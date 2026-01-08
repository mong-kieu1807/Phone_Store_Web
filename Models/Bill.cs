// Models/Bill.cs
using System;
using System.ComponentModel.DataAnnotations;
namespace PhoneStore.Models
{
	public class Bill
	{
		[Key]
		public int order_id { get; set; }                         // Khóa chính
		public int user_id { get; set; }                         // Mã người dùng (FK -> Users)
		public double total_amount { get; set; }             //Tổng tiền
		public string shipping_status { get; set; } = " ";          //  Trạng thái giao hàng
		public int payment_method_id { get; set; } // Phương thức thanh toán
		public DateTime created_at { get; set; } // Ngày tạo	
		public DateTime updated_at { get; set; } // Ngày cập nhật	
        public bool status { get; set; } = true;                // Trạng thái hóa đơn (hiển thị/ẩn)
    }
}