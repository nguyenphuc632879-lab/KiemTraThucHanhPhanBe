using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace MyStore_MVC5.Models
{
    public class DbInitializer : CreateDatabaseIfNotExists<DBEntities>
    {
        protected override void Seed(DBEntities context)
        {
            var categories = new List<Categories>
            {
                new Categories { CategoryName = "Điện thoại" },
                new Categories { CategoryName = "Laptop Gaming" },
                new Categories { CategoryName = "Màn hình & Phụ kiện" }
            };
            categories.ForEach(c => context.Categories.Add(c));
            context.SaveChanges();

            var products = new List<Products>
            {
                new Products
                {
                    CategoryID = categories[0].CategoryID,
                    ProductName = "iPhone 17 Air 256GB",
                    Description = "Thiết kế siêu mỏng nhẹ, chip A19 mạnh mẽ, màn hình OLED 120Hz.",
                    Image = "iphone17air.svg",
                    Price = 27990000m,
                    Quantity = 25
                },
                new Products
                {
                    CategoryID = categories[1].CategoryID,
                    ProductName = "Laptop HP Victus 15 (16GB DDR5)",
                    Description = "Laptop gaming hiệu năng cao, RAM 16GB DDR5 5600MHz, màn hình 144Hz.",
                    Image = "laptop_victus.svg",
                    Price = 21490000m,
                    Quantity = 15
                },
                new Products
                {
                    CategoryID = categories[2].CategoryID,
                    ProductName = "Màn hình AOC 24G50Z 24 inch",
                    Description = "Tấm nền IPS sắc nét, tần số quét cao chuyên game FPS.",
                    Image = "monitor_aoc.svg",
                    Price = 3490000m,
                    Quantity = 40
                },
                new Products
                {
                    CategoryID = categories[2].CategoryID,
                    ProductName = "Bàn phím cơ MEIJIAZHI RGB",
                    Description = "Bàn phím cơ layout gọn gàng, gõ êm, LED RGB nhiều chế độ.",
                    Image = "keyboard_mech.svg",
                    Price = 890000m,
                    Quantity = 60
                }
            };
            products.ForEach(p => context.Products.Add(p));
            context.SaveChanges();

            var customers = new List<Customers>
            {
                new Customers
                {
                    FullName = "Nguyễn Đức Phúc",
                    Email = "phuc.nd@huflit.edu.vn",
                    Phone = "0901234567",
                    Address = "Quận Tân Bình, TP. Hồ Chí Minh",
                    Username = "phucnd",
                    Password = "123"
                },
                new Customers
                {
                    FullName = "Hồ Thanh Phúc",
                    Email = "phuc.ht@huflit.edu.vn",
                    Phone = "0912345678",
                    Address = "Quận 10, TP. Hồ Chí Minh",
                    Username = "thanhphuc",
                    Password = "123"
                },
                new Customers
                {
                    FullName = "Trần Quốc Dũng",
                    Email = "dung.tq@huflit.edu.vn",
                    Phone = "0923456789",
                    Address = "TP. Thủ Đức, TP. Hồ Chí Minh",
                    Username = "quocdung",
                    Password = "123"
                }
            };
            customers.ForEach(c => context.Customers.Add(c));
            context.SaveChanges();

            // Tạo 22 đơn hàng mẫu trải đều các trạng thái để test phân trang Admin (15/trang) và tab Customer ("Xem thêm")
            var now = DateTime.Now;
            var orders = new List<Orders>();

            // 4 đơn "Chờ thanh toán" cho Nguyễn Đức Phúc (để test nút "Xem thêm" vì mặc định chỉ hiện 2 đơn)
            for (int i = 1; i <= 4; i++)
            {
                orders.Add(new Orders
                {
                    CustomerID = customers[0].CustomerID,
                    OrderDate = now.AddHours(-i * 3),
                    TotalAmount = products[0].Price,
                    PaymentStatus = PaymentStatuses.ChuaThanhToan,
                    PaymentMethod = "Thanh toán khi nhận hàng (COD)",
                    ShippingMethod = "Giao hàng nhanh",
                    ShippingAddress = customers[0].Address,
                    OrderStatus = OrderStatuses.ChoThanhToan,
                    OrderDetails = new List<OrderDetails>
                    {
                        new OrderDetails { ProductID = products[0].ProductID, Quantity = 1, UnitPrice = products[0].Price }
                    }
                });
            }

            // 3 đơn "Đang xử lý" cho Nguyễn Đức Phúc
            for (int i = 1; i <= 3; i++)
            {
                orders.Add(new Orders
                {
                    CustomerID = customers[0].CustomerID,
                    OrderDate = now.AddDays(-i),
                    TotalAmount = products[1].Price + products[3].Price,
                    PaymentStatus = PaymentStatuses.ChuyenKhoan,
                    PaymentMethod = "Chuyển khoản ngân hàng",
                    ShippingMethod = "Giao hàng tiêu chuẩn",
                    ShippingAddress = customers[0].Address,
                    OrderStatus = OrderStatuses.DangXuLy,
                    OrderDetails = new List<OrderDetails>
                    {
                        new OrderDetails { ProductID = products[1].ProductID, Quantity = 1, UnitPrice = products[1].Price },
                        new OrderDetails { ProductID = products[3].ProductID, Quantity = 1, UnitPrice = products[3].Price }
                    }
                });
            }

            // 2 đơn "Đang vận chuyển" cho Nguyễn Đức Phúc
            for (int i = 1; i <= 2; i++)
            {
                orders.Add(new Orders
                {
                    CustomerID = customers[0].CustomerID,
                    OrderDate = now.AddDays(-3 - i),
                    TotalAmount = products[2].Price,
                    PaymentStatus = PaymentStatuses.TienMat,
                    PaymentMethod = "Tiền mặt",
                    ShippingMethod = "Giao hàng hỏa tốc",
                    ShippingAddress = customers[0].Address,
                    OrderStatus = OrderStatuses.DangVanChuyen,
                    OrderDetails = new List<OrderDetails>
                    {
                        new OrderDetails { ProductID = products[2].ProductID, Quantity = 1, UnitPrice = products[2].Price }
                    }
                });
            }

            // 3 đơn "Đã giao" cho Nguyễn Đức Phúc
            for (int i = 1; i <= 3; i++)
            {
                orders.Add(new Orders
                {
                    CustomerID = customers[0].CustomerID,
                    OrderDate = now.AddDays(-7 - i),
                    TotalAmount = products[3].Price * 2,
                    PaymentStatus = PaymentStatuses.ChuyenKhoan,
                    PaymentMethod = "Chuyển khoản ngân hàng",
                    ShippingMethod = "Giao hàng nhanh",
                    ShippingAddress = customers[0].Address,
                    OrderStatus = OrderStatuses.DaGiao,
                    OrderDetails = new List<OrderDetails>
                    {
                        new OrderDetails { ProductID = products[3].ProductID, Quantity = 2, UnitPrice = products[3].Price }
                    }
                });
            }

            // 1 đơn "Đã hủy" cho Nguyễn Đức Phúc
            orders.Add(new Orders
            {
                CustomerID = customers[0].CustomerID,
                OrderDate = now.AddDays(-15),
                TotalAmount = products[0].Price,
                PaymentStatus = PaymentStatuses.ChuaThanhToan,
                PaymentMethod = "Chuyển khoản ngân hàng",
                ShippingMethod = "Giao hàng tiêu chuẩn",
                ShippingAddress = customers[0].Address,
                OrderStatus = OrderStatuses.DaHuy,
                OrderDetails = new List<OrderDetails>
                {
                    new OrderDetails { ProductID = products[0].ProductID, Quantity = 1, UnitPrice = products[0].Price }
                }
            });

            // Thêm 9 đơn cho 2 khách hàng còn lại để tổng = 22 đơn (> 15 đơn/trang -> có phân trang trang 1 & trang 2)
            for (int i = 1; i <= 9; i++)
            {
                var cust = (i % 2 == 0) ? customers[1] : customers[2];
                var prod = products[i % products.Count];
                var st = OrderStatuses.All[i % OrderStatuses.All.Length];
                var paySt = PaymentStatuses.All[i % PaymentStatuses.All.Length];

                orders.Add(new Orders
                {
                    CustomerID = cust.CustomerID,
                    OrderDate = now.AddDays(-i * 2),
                    TotalAmount = prod.Price,
                    PaymentStatus = paySt,
                    PaymentMethod = paySt == PaymentStatuses.ChuyenKhoan ? "Chuyển khoản ngân hàng" : "Tiền mặt",
                    ShippingMethod = "Giao hàng nhanh",
                    ShippingAddress = cust.Address,
                    OrderStatus = st,
                    OrderDetails = new List<OrderDetails>
                    {
                        new OrderDetails { ProductID = prod.ProductID, Quantity = 1, UnitPrice = prod.Price }
                    }
                });
            }

            orders.ForEach(o => context.Orders.Add(o));
            context.SaveChanges();
        }
    }
}
