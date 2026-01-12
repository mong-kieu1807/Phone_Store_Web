using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Dữ liệu tĩnh mẫu
            var orders = new List<Bill>
            {
                new Bill { order_id = 1, user_id = 1, total_amount = 15000000, shipping_status = "Completed", payment_method_id = 1, status = 1, created_at = DateTime.Now.AddDays(-5), updated_at = DateTime.Now.AddDays(-4) },
                new Bill { order_id = 2, user_id = 2, total_amount = 8500000, shipping_status = "Shipping", payment_method_id = 2, status = 1, created_at = DateTime.Now.AddDays(-3), updated_at = DateTime.Now.AddDays(-2) },
                new Bill { order_id = 3, user_id = 3, total_amount = 12000000, shipping_status = "Confirmed", payment_method_id = 1, status = 1, created_at = DateTime.Now.AddDays(-2), updated_at = DateTime.Now.AddDays(-1) },
                new Bill { order_id = 4, user_id = 1, total_amount = 25000000, shipping_status = "Pending", payment_method_id = 3, status = 1, created_at = DateTime.Now.AddDays(-1), updated_at = DateTime.Now.AddHours(-12) },
                new Bill { order_id = 5, user_id = 4, total_amount = 6500000, shipping_status = "Pending", payment_method_id = 2, status = 1, created_at = DateTime.Now, updated_at = DateTime.Now }
            };

            // Dictionary cho các trạng thái
            ViewBag.StatusInfo = new Dictionary<string, (string Label, string CssClass)>
            {
                { "Pending", ("Chờ xử lý", "label-warning") },
                { "Confirmed", ("Đã xác nhận", "label-info") },
                { "Shipping", ("Đang giao", "label-primary") },
                { "Completed", ("Hoàn thành", "label-success") },
                { "Cancelled", ("Đã hủy", "label-danger") }
            };
            
            return View(orders);
        }
    }
}
