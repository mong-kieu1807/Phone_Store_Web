// CHNhu - 20/10/2025 - Tạo ApiController trả về chuỗi đơn giản
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace core_23webc_gr6.Areas.Api
{
	[ApiController]
	[Route("api/[controller]")]
	public class ApiController : ControllerBase
	{
		[HttpGet]
		public string Get()
		{
			return "API is running";
		}
	}
}