using System;
using System.Linq;
using System.Net;
using System.Data.Entity;
using System.Web.Mvc;
using PagedList;
using MyStore_MVC5.Models;

namespace MyStore_MVC5.Controllers
{
  
    public class OrdersController : Controller
    {
        private DBEntities db = new DBEntities();   
        private const int PageSize = 15;

       
        public ActionResult Index(string customerName, string paymentStatus, DateTime? orderDate, int? page)
        {
            var query = db.Orders.Include(o => o.Customers).AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerName))
                query = query.Where(o => o.Customers.FullName.Contains(customerName.Trim()));

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(o => o.PaymentStatus == paymentStatus);

            if (orderDate.HasValue)
            {
                var d = orderDate.Value.Date;
                var next = d.AddDays(1);
                query = query.Where(o => o.OrderDate >= d && o.OrderDate < next);
            }

            var model = new OrderSearchVM
            {
                CustomerName = customerName,
                PaymentStatus = paymentStatus,
                OrderDate = orderDate,
                Orders = query.OrderByDescending(o => o.OrderDate)
                              .ToPagedList(page ?? 1, PageSize)
            };

            ViewBag.PaymentStatuses = new SelectList(PaymentStatuses.All, paymentStatus);
            return View(model);
        }

       
        public ActionResult DetailedOrder(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.Orders
                          .Include(o => o.Customers)
                          .Include(o => o.OrderDetails.Select(d => d.Products))
                          .FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound();
            return View(order);
        }

       
        public ActionResult UpdateStatus(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order = db.Orders.Include(o => o.Customers).FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound();

            ViewBag.OrderStatuses   = new SelectList(OrderStatuses.All, order.OrderStatus);
            ViewBag.PaymentStatuses = new SelectList(PaymentStatuses.All, order.PaymentStatus);
            return View(order);
        }

       
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, string orderStatus, string paymentStatus)
        {
            var order = db.Orders.Include(o => o.Customers).FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound();

            if (!OrderStatuses.All.Contains(orderStatus) || !PaymentStatuses.All.Contains(paymentStatus))
            {
                ModelState.AddModelError("", "Trạng thái không hợp lệ.");
                ViewBag.OrderStatuses   = new SelectList(OrderStatuses.All, order.OrderStatus);
                ViewBag.PaymentStatuses = new SelectList(PaymentStatuses.All, order.PaymentStatus);
                return View(order);
            }

          
            if (orderStatus == OrderStatuses.DaHuy && order.OrderStatus != OrderStatuses.DaHuy)
            {
                foreach (var d in db.OrderDetails.Include(x => x.Products).Where(x => x.OrderID == id))
                    d.Products.Quantity += d.Quantity;
            }

            order.OrderStatus   = orderStatus;
            order.PaymentStatus = paymentStatus;
            db.SaveChanges();
            return RedirectToAction("DetailedOrder", new { id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
