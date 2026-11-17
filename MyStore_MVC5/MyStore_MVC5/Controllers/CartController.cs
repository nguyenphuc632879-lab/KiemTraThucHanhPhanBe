using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStore_MVC5.Models;

namespace MyStore_MVC5.Controllers
{
    public class CartController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: Cart/Index - Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            return View(cart);
        }

        // GET: Cart/Add - Thêm sản phẩm vào giỏ hàng
        public ActionResult Add(int productId, int quantity = 1)
        {
            var p = db.Products.Find(productId);
            if (p == null || p.Quantity <= 0)
            {
                TempData["Message"] = "Sản phẩm không tồn tại hoặc đã hết hàng.";
                return RedirectToAction("Index");
            }

            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductID == productId);
            if (item == null)
            {
                // Không cho vượt quá số lượng tồn
                int qty = Math.Min(quantity, p.Quantity);
                cart.Add(new CartItem
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Image = p.Image,
                    Price = p.Price,
                    Quantity = qty
                });
            }
            else
            {
                // Tăng số lượng nhưng không vượt quá tồn kho
                if (item.Quantity + quantity <= p.Quantity)
                    item.Quantity += quantity;
                else
                    item.Quantity = p.Quantity;
            }
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // GET: Cart/Increase - Tăng số lượng (không vượt quá tồn kho)
        public ActionResult Increase(int productId)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductID == productId);
                if (item != null)
                {
                    var prod = db.Products.Find(productId);
                    if (prod != null && item.Quantity < prod.Quantity)
                        item.Quantity++;
                }
                Session["Cart"] = cart;
            }
            return RedirectToAction("Index");
        }

        // GET: Cart/Decrease - Giảm số lượng (không cho < 1, nếu = 1 thì remove)
        public ActionResult Decrease(int productId)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductID == productId);
                if (item != null)
                {
                    if (item.Quantity > 1)
                        item.Quantity--;
                    else
                        cart.Remove(item);
                }
                Session["Cart"] = cart;
            }
            return RedirectToAction("Index");
        }

        // GET: Cart/Remove - Xóa sản phẩm khỏi giỏ hàng
        public ActionResult Remove(int productId)
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                cart.RemoveAll(c => c.ProductID == productId);
                Session["Cart"] = cart;
            }
            return RedirectToAction("Index");
        }

        // POST: Cart/Checkout - Thanh toán và tạo đơn hàng
        [HttpPost]
        public ActionResult Checkout(string shippingAddress, string paymentMethod, string shippingMethod)
        {
            if (Session["CustomerID"] == null)
                return RedirectToAction("Login", "Account");

            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null || !cart.Any())
                return RedirectToAction("Index");

            int customerId = (int)Session["CustomerID"];
            var cust = db.Customers.Find(customerId);

            var order = new Orders
            {
                CustomerID = customerId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(c => c.SubTotal),
                PaymentStatus = paymentMethod == "Chuyển khoản ngân hàng" ? PaymentStatuses.ChuyenKhoan : PaymentStatuses.ChuaThanhToan,
                PaymentMethod = string.IsNullOrEmpty(paymentMethod) ? "Thanh toán khi nhận hàng (COD)" : paymentMethod,
                ShippingMethod = string.IsNullOrEmpty(shippingMethod) ? "Giao hàng nhanh" : shippingMethod,
                ShippingAddress = string.IsNullOrWhiteSpace(shippingAddress) ? (cust?.Address ?? "TP. Hồ Chí Minh") : shippingAddress,
                OrderStatus = OrderStatuses.ChoThanhToan,
                OrderDetails = new List<OrderDetails>()
            };

            foreach (var item in cart)
            {
                var prod = db.Products.Find(item.ProductID);
                if (prod != null)
                {
                    int qty = Math.Min(item.Quantity, prod.Quantity);
                    if (qty > 0)
                    {
                        prod.Quantity -= qty;
                        order.OrderDetails.Add(new OrderDetails
                        {
                            ProductID = prod.ProductID,
                            Quantity = qty,
                            UnitPrice = prod.Price
                        });
                    }
                }
            }

            db.Orders.Add(order);
            db.SaveChanges();
            Session["Cart"] = null;

            return RedirectToAction("MyOrder", "MyOrder");
        }

        // Helper: Đếm tổng số lượng hàng trong giỏ (dùng cho _MasterLayout)
        public static int GetCartCount(System.Web.HttpSessionStateBase session)
        {
            var cart = session["Cart"] as List<CartItem>;
            return cart?.Sum(c => c.Quantity) ?? 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
