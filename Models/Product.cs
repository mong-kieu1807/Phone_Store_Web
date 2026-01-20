using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneStore.Models
{
	public class Product
	{
		[Key]
		public int product_id { get; set; }             // Khóa chính
		[Required]
		public string product_name { get; set; } = string.Empty;      // Tên sản phẩm
		
		public int category_id { get; set; }            // Mã danh mục (có thể null)
		
        public string brand { get; set; } = string.Empty;
		
		public decimal price { get; set; }              // Giá
		
		public int discount_percent { get; set; }     // Phần trăm giảm giá
		
		public int stock_quantity { get; set; }                  // Số lượng tồn kho
		public int sold_count { get; set; }                  // Số lượng đã bán
		public int view_count { get; set; }                  // Số lượt xem
		public decimal rating { get; set; }
	public string image { get; set; } = string.Empty;           // Ảnh
	
	public string description { get; set; } = string.Empty;     // Mô tả
	
	public string ram { get; set; } = string.Empty;
	
	public string storage { get; set; } = string.Empty;
	
	public string color { get; set; } = string.Empty;
	
	public string os { get; set; } = string.Empty;
	
	public string screen_size { get; set; } = string.Empty;
	
	public byte status { get; set; }                // Trạng thái (1 = hoạt động, 0 = ẩn)
	
	public DateTime created_at { get; set; }         // Ngày tạo
	
	public DateTime updated_at { get; set; }         // Ngày cập nhật
    }
}
