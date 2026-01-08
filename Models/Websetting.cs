using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneStore.Models
{
	public class Websetting
	{
		[Key]
		public int setting_id { get; set; }                         
		public string config_key { get; set; } = " ";
        public string config_value { get; set; } = " ";                  
		public string description { get; set; } = " ";
    }
}