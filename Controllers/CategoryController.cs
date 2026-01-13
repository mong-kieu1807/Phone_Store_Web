using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models;
using PhoneStore.Data;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Controllers
{
    public class CategoryController : Controller
    {
        // GET: CategoryController
        private readonly ApplicationDbContext _context;

        public CategoryController( ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult Index(int? categoryId, int page = 1)
        {
            const int pageSize = 9; // Hiển thị 9 sản phẩm mỗi trang (3x3 grid)
            
            var categories =  _context.Categories
                .Where(c => c.status == 1)
                .ToList();
            
            // Lấy query sản phẩm theo danh mục
            var productsQuery = categoryId.HasValue 
                ? _context.Products.Where(p => p.category_id == categoryId.Value && p.status == 1)
                : _context.Products.Where(p => p.status == 1);
            
            // Đếm tổng số sản phẩm
            var totalProducts = productsQuery.Count();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            
            // Đảm bảo page hợp lệ
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;
            
            // Lấy sản phẩm theo trang
            var products = productsQuery
                .OrderByDescending(p => p.created_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.PageSize = pageSize;
            
            return View(products);
        }

    }
}
