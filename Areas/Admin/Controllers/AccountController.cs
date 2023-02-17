using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;
//doi tuong ma hoa pass -> co the gan vao 1 bien
using BC = BCrypt.Net.BCrypt;

using BTL.Models;

namespace BTL.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        public MyDbContext db = new MyDbContext();  
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public IActionResult LoginPost(IFormCollection fc) 
        {
            string _email = fc["email"].ToString().Trim();
            string _pass = fc["password"].ToString().Trim();

            ItemUser user = db.Users.Where(n => n.Email == _email).FirstOrDefault();
            if (user != null) 
            {
                if (BC.Verify(_pass, user.Password) == true)
                {
                    //khoi tao bien session
                    HttpContext.Session.SetString("admin_email", _email);

                    HttpContext.Session.SetString("admin_id", user.Id.ToString());

                    return Redirect("/Admin/Home");
                }
            }
            return Redirect("/Admin/Account/Login?notify=invalid");

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("admin_email");
			return Redirect("/Admin/Account/Login");	
        }

    }
}
