using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class User
	{
		[Key]
		public int user_id { get; set; }                 // Khóa chính
	public string? username { get; set; }      // Tên đăng nhập
	public string? password { get; set; }      // Mật khẩu
	public string? full_name { get; set; }      // Họ và tên
        public string? email { get; set; }         // Địa chỉ email (duy nhất)
        public string? phone { get; set; }         // Số điện thoại
        public string? address { get; set; }         
        public string? role { get; set; } = "User";      // Vai trò (User/Admin)
		public byte status { get; set; } = 1;        // Trạng thái hoạt động
		public DateTime? created_at { get; set; } // Ngày tạo
		public DateTime? updated_at { get; set; } // Ngày cập nhật
    }
}