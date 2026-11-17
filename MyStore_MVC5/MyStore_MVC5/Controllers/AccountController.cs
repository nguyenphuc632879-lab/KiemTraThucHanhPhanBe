using System.Linq;
using System.Web.Mvc;
using MyStore_MVC5.Models;

namespace MyStore_MVC5.Controllers
{
    public class AccountController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: Account/Login
        public ActionResult Login()
        {
            ViewBag.Customers = db.Customers.ToList();
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var cust = db.Customers.FirstOrDefault(c => c.Username == username && c.Password == password);
            if (cust != null)
            {
                Session["CustomerID"] = cust.CustomerID;
                Session["CustomerName"] = cust.FullName;
                // Đăng nhập thành công chuyển về trang Home (hoặc MyOrder)
                return RedirectToAction("Index", "Home");
            }
            // Đăng nhập thất bại: hiển thị thông báo
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu.";
            ViewBag.Customers = db.Customers.ToList();
            return View();
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public ActionResult Register(Customers model, string confirmPassword)
        {
            // Kiểm tra dữ liệu nhập (Data Annotation)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra trùng thông tin với tài khoản đã có (Username hoặc Email)
            if (db.Customers.Any(c => c.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }
            if (db.Customers.Any(c => c.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }

            // Kiểm tra mật khẩu xác nhận
            if (model.Password != confirmPassword)
            {
                ModelState.AddModelError("Password", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            // Đăng ký thành công: lưu vào DB
            db.Customers.Add(model);
            db.SaveChanges();

            // Redirect về trang Đăng nhập
            TempData["RegisterSuccess"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // Chuyển nhanh tài khoản khách hàng để test quyền xem đơn hàng của từng khách
        public ActionResult QuickSwitch(int id)
        {
            var cust = db.Customers.Find(id);
            if (cust != null)
            {
                Session["CustomerID"] = cust.CustomerID;
                Session["CustomerName"] = cust.FullName;
            }
            return RedirectToAction("MyOrder", "MyOrder");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
