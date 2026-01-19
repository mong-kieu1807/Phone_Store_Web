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


         public async Task<IActionResult> Index(string? search, string? sort) 
        {
            var query = _context.Suppliers
                .Where(s => s.status == 1); // chỉ lấy NCC đang hoạt động

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.supplier_name.Contains(search));
            }

            //Sắp xếp
            query = sort == "asc"
                ? query.OrderBy(s => s.supplier_name)
                : query.OrderByDescending(s => s.supplier_id);

            var suppliers = await query.ToListAsync();

            //Giữ lại trạng thái
            ViewBag.Search = search;
            ViewBag.Sort   = sort ?? "desc";

            return View(suppliers);
        }

        //Hiển thị form tạo nhà cung cấp
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        //Xử lý tạo nhà cung cấp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ";
                return View(model);
            }

            model.status = 1;
            _context.Suppliers.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công!";
            return RedirectToAction("Index");
        }

        // Hiển thị form chỉnh sửa nhà cung cấp
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            return View(supplier);
        }
        // Xử lý chỉnh sửa nhà cung cấp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier model)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            supplier.supplier_name = model.supplier_name;
            supplier.phone         = model.phone;
            supplier.email         = model.email;
            supplier.address       = model.address;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật nhà cung cấp thành công!";
            return RedirectToAction("Index");
        }

        // Xử lý xóa nhà cung cấp (xóa mềm)
       [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);

            if (supplier == null)
            {
                return Json(new { success = false, message = "Không tìm thấy nhà cung cấp" });
            }

            supplier.status = 0; //xóa mềm
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã xóa nhà cung cấp" });
        }
    }
}
