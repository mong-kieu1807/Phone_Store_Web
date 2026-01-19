using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneStore.Models
{
	public class Contact
	{
		[Key]
		public int contact_id { get; set; }
		public int? user_id { get; set; }
		public string? full_name { get; set; }
		public string? email { get; set; }
		public string? phone { get; set; }
		public string? subject { get; set; }
		public string? message { get; set; }
		public string? status { get; set; } = "Chưa xử lý";
		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }
	}
}
