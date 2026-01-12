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

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Bills
                .OrderBy(b => b.order_id)
                .Take(50)
                .ToListAsync();

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
