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

        public async Task<IActionResult> Index()
        {
            var receipts = await _context.GoodsReceipts
                .OrderBy(g => g.import_id)
                .ToListAsync();
            
            return View(receipts);
        }

        // Xóa phiếu nhập
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var receipt = await _context.GoodsReceipts.FindAsync(id);
                if (receipt == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phiếu nhập!" });
                }

                _context.GoodsReceipts.Remove(receipt);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa phiếu nhập thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
