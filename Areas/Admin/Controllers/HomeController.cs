using Microsoft.AspNetCore.Mvc;
using BTL.Areas.Admin.Attributes;
namespace BTL.Areas.Admin.Controllers
{
	[Area("Admin")]

	[CheckLogin]
	
	public class HomeController : Controller
	{
		
		public IActionResult Index()
		{
			return View();
		}
	}
}
