using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, string? sort)
        {
            var query = _context.Categories
                .Where(c => c.status == 1);   // chỉ lấy danh mục đang hiển thị

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.category_name.Contains(search));
            }

            // Sắp xếp
            query = sort == "asc"
                ? query.OrderBy(c => c.created_at)
                : query.OrderByDescending(c => c.created_at);

                var categories = await query.ToListAsync();


            ViewBag.Search  = search;           // giữ lại từ khóa tìm kiếm trên view
            ViewBag.Sort    = sort ?? "desc";   // giữ lại kiểu sắp xếp trên view

            return View(categories);
        }

         // Hiển thị form tạo danh mục
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        // Xử lý tạo danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            //Kiểm tra dữ liệu hợp lệ
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ!";
                return View(model);
            }

            // Kiểm tra tên danh mục đã tồn tại
            bool isExist = await _context.Categories
                .AnyAsync(c => c.category_name == model.category_name);

            if (isExist)
            {
                ModelState.AddModelError("category_name", "Tên danh mục đã tồn tại!");
                return View(model);
            }

            // Thiết lập các thuộc tính còn lại
            model.status = 1; // Mặc định là hoạt động
            model.created_at = DateTime.Now;
            model.updated_at = DateTime.Now;

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            // Hiển thị thông báo thành công
            TempData["SuccessMessage"] = "Thêm danh mục thành công!";

            return RedirectToAction("Index");
        }

        // Hiển thị form cập nhật danh mục
        [HttpGet]
         public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category); // giữ form cũ
        }
        // Xử lý cập nhật danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category model)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            category.category_name = model.category_name;
            category.description   = model.description;

            category.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
            return RedirectToAction("Index");
        }


        //Xóa danh mục
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            // Kiểm tra nếu không tìm thấy danh mục
            if (category == null)
            {
                return Json(new { success = false, message = "Không tìm thấy danh mục" });
            }
            // Kiểm tra nếu danh mục có sản phẩm liên kết
            bool hasProduct = await _context.Products
                .AnyAsync(p => p.category_id == id && p.status == 1);

            if (hasProduct)
            {
                return Json(new
                {
                    success = false,
                    message = "Danh mục đang có sản phẩm, không thể xóa"
                });
            }

            category.status = 0;           
            category.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã xóa danh mục" });
        }

    }
}
