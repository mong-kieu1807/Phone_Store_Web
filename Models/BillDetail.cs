// Models/Bill.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class BillDetail
	{
		[Key]
		public int order_id { get; set; }                         // Khóa chính
		[Key]
		public int product_id { get; set; }                         // Mã người dùng (FK -> Users)
		public int quantity { get; set; }               //Tổng tiền
		public double price { get; set; }               // Số điện thoại liên hệ
    }
}