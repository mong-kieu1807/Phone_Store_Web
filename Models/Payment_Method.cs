using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class Payment_Method
	{
		[Key]
		public int method_id { get; set; }                         
		public string method_name { get; set; } = " ";              
		public byte status { get; set; } = 1;
    }
}