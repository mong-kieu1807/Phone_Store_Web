using System;

namespace PhoneStore.Models
{
    // ViewModel dành riêng cho trang Admin hiển thị
    public class ReviewAdminViewModel
    {
        public int ReviewId { get; set; }
        public string UserFullName { get; set; }
        public string ProductName { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte Status { get; set; } // Dùng byte để khớp với Controller
    }

    // ViewModel dành cho trang Khách hàng (để hiển thị bên Product/Details)
    public class ReviewViewModel
    {
        public Review Review { get; set; }
        public string UserFullName { get; set; }
    }
}