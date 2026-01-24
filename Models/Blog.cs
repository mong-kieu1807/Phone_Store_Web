using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class Blog
	{
		[Key]
		public int blog_id { get; set; }
		
		[Required]
		[StringLength(200)]
		public string title { get; set; } = string.Empty;
		
		[Required]
		public string content { get; set; } = string.Empty;
		
		[StringLength(100)]
		public string author { get; set; } = string.Empty;
		
		public string image { get; set; } = string.Empty;
		
		[StringLength(500)]
		public string summary { get; set; } = string.Empty;
		
		public int view_count { get; set; } = 0;
		
		public byte status { get; set; } = 1; // 1 = active, 0 = inactive
		
		public DateTime created_at { get; set; } = DateTime.Now;
		
		public DateTime updated_at { get; set; } = DateTime.Now;
	}
}
