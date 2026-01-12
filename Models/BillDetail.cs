// Models/Bill.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class BillDetail
	{
		public int order_id { get; set; }                         // Khóa chính (FK -> Bills)
		public int product_id { get; set; }                       // Khóa chính (FK -> Products)
		public int quantity { get; set; }                         // Số lượng
	public decimal price { get; set; }                         // Giá tại thời điểm mua
    }
}