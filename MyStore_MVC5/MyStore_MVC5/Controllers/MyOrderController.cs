using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using MyStore_MVC5.Models;

namespace MyStore_MVC5.Controllers
{
    public class MyOrderController : Controller
    {
        private DBEntities db = new DBEntities();
        private const int PreviewCount = 2;   // Mặc định chỉ hiện 2 đơn gần nhất
        private const int MaxCount = 50;      // Khi bấm "Xem thêm" thì hiện tối đa 50

        // GET: MyOrder/MyOrder
        public ActionResult MyOrder(string keyword, string tab, string expandStatus)
        {
            // Chưa đăng nhập thì chuyển về trang Login
            if (Session["CustomerID"] == null)
                return RedirectToAction("Login", "Account");

            int customerId = (int)Session["CustomerID"];
            tab = string.IsNullOrEmpty(tab) ? "all" : tab;

            // Lấy danh sách đơn hàng của khách đang đăng nhập, kèm chi tiết & sản phẩm
            var query = db.Orders
                          .Include(o => o.OrderDetails.Select(d => d.Products))
                          .Where(o => o.CustomerID == customerId);

            // Tìm kiếm theo Mã đơn hàng hoặc Tên sản phẩm
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim();
                int id;
                bool isId = int.TryParse(k, out id);
                query = query.Where(o => (isId && o.OrderID == id)
                                      || o.OrderDetails.Any(d => d.Products.ProductName.Contains(k)));
            }

            // Xác định danh sách trạng thái cần hiển thị theo tab
            var statuses = (tab == "all") ? OrderStatuses.All
                                          : OrderStatuses.All.Where(s => s == tab).ToArray();

            // Lấy tất cả đơn hàng thỏa điều kiện tìm kiếm
            var allFilteredOrders = query.OrderByDescending(o => o.OrderDate).ToList();

            // Kiểm tra khách hàng có đơn hàng nào không (không tính keyword filter)
            bool hasAnyOrder = db.Orders.Any(o => o.CustomerID == customerId);

            // Tạo groups theo trạng thái
            var groups = new List<MyOrderGroupVM>();
            foreach (var s in statuses)
            {
                var statusOrders = allFilteredOrders
                    .Where(o => o.OrderStatus == s)
                    .ToList();

                if (statusOrders.Count == 0 && tab != "all") continue;
                if (statusOrders.Count == 0) continue;

                // Tab "all": mặc định hiện 2 đơn gần nhất, trừ khi bấm "Xem thêm" cho nhóm đó
                // Tab trạng thái cụ thể: hiện tối đa 50 đơn
                bool isExpanded = (tab == s) || (s == expandStatus);
                int take = isExpanded ? MaxCount : PreviewCount;

                groups.Add(new MyOrderGroupVM
                {
                    Status = s,
                    TotalInStatus = statusOrders.Count,
                    Orders = statusOrders.Take(take).ToList(),
                    Expanded = isExpanded
                });
            }

            var vm = new MyOrderVM
            {
                Keyword = keyword,
                Tab = tab,
                Groups = groups,
                HasAnyOrder = hasAnyOrder
            };
            return View("MyOrder", vm);
        }

        // GET: MyOrder/Reorder - Thêm sản phẩm vào giỏ hàng với số lượng = 1
        public ActionResult Reorder(int productId)
        {
            var p = db.Products.Find(productId);
            if (p == null || p.Quantity <= 0)
            {
                TempData["Message"] = "Sản phẩm hiện đã hết hàng.";
                return RedirectToAction("MyOrder");
            }

            var cart = Session["Cart"] as List<CartItem>
                       ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductID == productId);
            if (item == null)
                cart.Add(new CartItem
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Image = p.Image,
                    Price = p.Price,
                    Quantity = 1
                });
            else if (item.Quantity < p.Quantity)
                item.Quantity += 1;

            Session["Cart"] = cart;
            return RedirectToAction("Index", "Cart");
        }

        // GET: MyOrder/DetailedOrder - Xem chi tiết đơn hàng (hóa đơn)
        public ActionResult DetailedOrder(int? id)
        {
            if (Session["CustomerID"] == null)
                return RedirectToAction("Login", "Account");
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            int customerId = (int)Session["CustomerID"];
            var order = db.Orders
                          .Include(o => o.Customers)
                          .Include(o => o.OrderDetails.Select(d => d.Products))
                          .FirstOrDefault(o => o.OrderID == id && o.CustomerID == customerId);
            if (order == null) return HttpNotFound();

            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
