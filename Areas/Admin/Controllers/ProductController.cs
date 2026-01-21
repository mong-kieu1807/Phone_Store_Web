using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models;
using PhoneStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace PhoneStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _env;

		public ProductController(ApplicationDbContext context, IWebHostEnvironment env)
		{
			_context = context;
			_env = env;
		}

		// Hiển thị danh sách sản phẩm với phân trang
		public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
		{
			int totalProducts = await _context.Products.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

			if (totalPages == 0) totalPages = 1;
			if (page < 1) page = 1;
			if (page > totalPages) page = totalPages;

			var pagedProducts = await _context.Products
			.OrderBy(p => p.product_id)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
			ViewBag.Page = page;
			ViewBag.TotalPages = totalPages;
			return View(pagedProducts);
		}

		// Hiển thị form thêm sản phẩm
		[HttpGet]
		public async Task<IActionResult> AddProduct()
		{
		ViewBag.Categories = await _context.Set<Category>().Where(c => c.status == 1).ToListAsync();
		return View();
		}

	// Xử lý thêm sản phẩm mới
	[HttpPost]
	public async Task<IActionResult> AddProduct(Product product, IFormFile? imageFile)
	{
		if (!ModelState.IsValid)
		{
			ViewBag.Categories = await _context.Set<Category>().Where(c => c.status == 1).ToListAsync();
			TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin bắt buộc!";
			return View(product);
		}

		try
		{
			// Xử lý upload ảnh
			if (imageFile != null && imageFile.Length > 0)
			{
				var uploadDir = Path.Combine(_env.WebRootPath, "img");
				if (!Directory.Exists(uploadDir))
				{
					Directory.CreateDirectory(uploadDir);
				}

				var ext = Path.GetExtension(imageFile.FileName);
				var fileName = $"{Guid.NewGuid()}{ext}";
				var filePath = Path.Combine(uploadDir, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(stream);
				}
				
				product.image = fileName;
			}
			

			product.created_at = DateTime.Now;
			product.updated_at = DateTime.Now;
			product.status = 1;

			_context.Products.Add(product);
			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
			return RedirectToAction("Index");
		}
		catch (Exception ex)
		{
			TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
			ViewBag.Categories = await _context.Set<Category>().Where(c => c.status == 1).ToListAsync();
			return View(product);
		}
	}

	// Hiển thị form chỉnh sửa sản phẩm
	[HttpGet]
	public async Task<IActionResult> EditProduct(int id)
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null)
		{
			TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
			return RedirectToAction("Index");
		}

		ViewBag.Categories = await _context.Set<Category>().Where(c => c.status == 1).ToListAsync();
		return View(product);
	}

	// Xử lý cập nhật sản phẩm
	[HttpPost]
	public async Task<IActionResult> EditProduct(Product product, IFormFile? imageFile)
	{
		if (!ModelState.IsValid)
		{
			ViewBag.Categories = await _context.Set<Category>().Where(c => c.status == 1).ToListAsync();
			return View(product);
		}

		try
		{
			var existingProduct = await _context.Products.FindAsync(product.product_id);
			if (existingProduct == null)
			{
				TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
				return RedirectToAction("Index");
			}

			// Xử lý upload ảnh mới
			if (imageFile != null && imageFile.Length > 0)
			{
				var uploadDir = Path.Combine(_env.WebRootPath, "img");
				if (!Directory.Exists(uploadDir))
				{
					Directory.CreateDirectory(uploadDir);
				}

				// Xóa ảnh cũ
				if (!string.IsNullOrEmpty(existingProduct.image))
				{
					var oldFilePath = Path.Combine(uploadDir, existingProduct.image);
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath);
					}
				}

				var ext = Path.GetExtension(imageFile.FileName);
				var fileName = $"{Guid.NewGuid()}{ext}";
				var filePath = Path.Combine(uploadDir, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(stream);
				}
				
				existingProduct.image = fileName;
			}

			// Cập nhật các trường
			existingProduct.product_name = product.product_name;
			existingProduct.category_id = product.category_id;
			existingProduct.brand = product.brand;
			existingProduct.price = product.price;
			existingProduct.discount_percent = product.discount_percent;
			existingProduct.stock_quantity = product.stock_quantity;
			existingProduct.description = product.description;
			existingProduct.ram = product.ram;
			existingProduct.storage = product.storage;
			existingProduct.color = product.color;
			existingProduct.os = product.os;
			existingProduct.screen_size = product.screen_size;
			existingProduct.status = product.status;
			existingProduct.updated_at = DateTime.Now;

			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
			return RedirectToAction("Index");
		}
		catch (Exception ex)
		{
			TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
			ViewBag.Categories = await _context.Set<Category>().Where(c => c.status == 1).ToListAsync();
		return View(product);
	}
}

// Xóa sản phẩm
[HttpPost]
public async Task<IActionResult> DeleteProduct(int id)
{
	try
	{
		var product = await _context.Products.FindAsync(id);
		if (product == null)
		{
			return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
		}

		// Xóa ảnh
		if (!string.IsNullOrEmpty(product.image))
		{
			var uploadDir = Path.Combine(_env.WebRootPath, "img", "Products");
			var filePath = Path.Combine(uploadDir, product.image);
			if (System.IO.File.Exists(filePath))
			{
				System.IO.File.Delete(filePath);
			}
		}

		_context.Products.Remove(product);
		await _context.SaveChangesAsync();

		return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
	}
	catch (Exception ex)
	{
		return Json(new { success = false, message = "Lỗi: " + ex.Message });
	}
}
}
}
