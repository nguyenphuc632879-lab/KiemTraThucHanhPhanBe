using System.Linq;
using System.Web.Mvc;
using MyStore_MVC5.Models;

namespace MyStore_MVC5.Controllers
{
    public class HomeController : Controller
    {
        private DBEntities db = new DBEntities();

        public ActionResult Index()
        {
            // Tự động đăng nhập sẵn tài khoản mẫu (CustomerID = 1) nếu chưa chọn để test nhanh
            if (Session["CustomerID"] == null)
            {
                var defaultCust = db.Customers.FirstOrDefault();
                if (defaultCust != null)
                {
                    Session["CustomerID"] = defaultCust.CustomerID;
                    Session["CustomerName"] = defaultCust.FullName;
                }
            }

            ViewBag.Customers = db.Customers.ToList();

            // Chỉ hiển thị sản phẩm có số lượng tồn > 0 (theo yêu cầu checklist)
            var products = db.Products.Where(p => p.Quantity > 0).ToList();
            return View(products);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
