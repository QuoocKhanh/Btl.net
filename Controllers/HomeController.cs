using Microsoft.AspNetCore.Mvc;

namespace BTL.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
