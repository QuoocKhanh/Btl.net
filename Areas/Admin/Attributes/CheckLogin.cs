using Microsoft.AspNetCore.Mvc.Filters;

namespace BTL.Areas.Admin.Attributes
{
    public class CheckLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //--
            //lay gtri cua bien session
            string _email = context.HttpContext.Session.GetString("admin_email");

            if (string.IsNullOrEmpty(_email))
            {
                context.HttpContext.Response.Redirect("/Admin/Account/Login");
            }

            //--
            base.OnActionExecuting(context);
        }

    }
}
