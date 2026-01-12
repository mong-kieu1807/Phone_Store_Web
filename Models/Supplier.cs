using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class Supplier
	{
		[Key]
		public int supplier_id { get; set; }                                                
	public string? supplier_name { get; set; }
	public string? phone { get; set; }
	public string? address { get; set; }
	public string? email { get; set; }
	public byte status { get; set; }
    }
}