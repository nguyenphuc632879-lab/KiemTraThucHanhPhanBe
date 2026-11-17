using System;
using System.Collections.Generic;
using PagedList;

namespace MyStore_MVC5.Models
{
    // Hằng số trạng thái đơn hàng (lưu ở cột Orders.OrderStatus)
    public static class OrderStatuses
    {
        public const string ChoThanhToan  = "Chờ thanh toán";
        public const string DangXuLy      = "Đang xử lý";
        public const string DangVanChuyen = "Đang vận chuyển";
        public const string DaGiao        = "Đã giao";
        public const string DaHuy         = "Đã hủy";

        public static readonly string[] All =
            { ChoThanhToan, DangXuLy, DangVanChuyen, DaGiao, DaHuy };
    }

    // Hằng số trạng thái thanh toán (lưu ở cột Orders.PaymentStatus)
    public static class PaymentStatuses
    {
        public const string ChuaThanhToan = "Chưa thanh toán";
        public const string TienMat       = "Thanh toán tiền mặt";
        public const string ChuyenKhoan   = "Chuyển khoản";

        public static readonly string[] All = { ChuaThanhToan, TienMat, ChuyenKhoan };
    }

    // ---------- ADMIN ----------
    public class OrderSearchVM
    {
        public string CustomerName { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public IPagedList<Orders> Orders { get; set; }
    }

    // ---------- CUSTOMER ----------
    public class MyOrderGroupVM
    {
        public string Status { get; set; }
        public List<Orders> Orders { get; set; }   // 2 đơn gần nhất hoặc tối đa 50
        public int TotalInStatus { get; set; }
        public bool Expanded { get; set; }          // true khi đã bấm "Xem thêm"
    }

    public class MyOrderVM
    {
        public string Keyword { get; set; }
        public string Tab { get; set; }            // "all" hoặc tên trạng thái
        public bool HasAnyOrder { get; set; }      // true nếu khách có ít nhất 1 đơn hàng
        public List<MyOrderGroupVM> Groups { get; set; }
    }
}
