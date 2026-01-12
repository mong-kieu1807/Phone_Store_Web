using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _context.Suppliers
                .OrderBy(s => s.supplier_id)
                .ToListAsync();
            
            return View(suppliers);
        }

        // Xóa nhà cung cấp
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var supplier = await _context.Suppliers.FindAsync(id);
                if (supplier == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhà cung cấp!" });
                }

                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa nhà cung cấp thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
