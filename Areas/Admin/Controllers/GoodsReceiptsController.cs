using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GoodsReceiptsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GoodsReceiptsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Dữ liệu tĩnh mẫu
            var receipts = new List<GoodsReceipts>
            {
                new GoodsReceipts { import_id = 1, supplier_id = 1, user_id = 1, total_cost = 50000000, status = 1, created_at = DateTime.Now.AddDays(-10) },
                new GoodsReceipts { import_id = 2, supplier_id = 2, user_id = 1, total_cost = 35000000, status = 1, created_at = DateTime.Now.AddDays(-7) },
                new GoodsReceipts { import_id = 3, supplier_id = 1, user_id = 2, total_cost = 28000000, status = 1, created_at = DateTime.Now.AddDays(-5) },
                new GoodsReceipts { import_id = 4, supplier_id = 3, user_id = 1, total_cost = 42000000, status = 1, created_at = DateTime.Now.AddDays(-3) },
                new GoodsReceipts { import_id = 5, supplier_id = 2, user_id = 2, total_cost = 31000000, status = 1, created_at = DateTime.Now.AddDays(-1) }
            };
            
            return View(receipts);
        }

        // Xóa phiếu nhập (Chức năng tạm thời - Dữ liệu tĩnh)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Không thực hiện xóa thật sự - chỉ trả về thông báo
            return Json(new { success = true, message = "Chức năng đang phát triển - Dữ liệu tĩnh" });
        }
    }
}
