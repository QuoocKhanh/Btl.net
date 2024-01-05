using BTL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BTL.Controllers
{
    public class ContactController : Controller
    {
        MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("customer_email")))
            {
                List<ItemCustomers> cus = db.Customers.OrderByDescending(item => item.Id).ToList();
                foreach (var item in cus)
                {
                    if (item.Email == HttpContext.Session.GetString("customer_email"))
                    {
                       
                        ViewBag.email = item.Email;
                        ViewBag.name = item.Name;
                        
                    }
                }
                return View("Index", cus);
            }
            return View();
        }
    }
}
