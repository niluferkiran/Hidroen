using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SusisProject_V6.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            // Oturum kontrolü
            if (Session["UserID"] == null &&
                !filterContext.ActionDescriptor.ActionName.Equals("Login", StringComparison.OrdinalIgnoreCase))
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }
        }
    }
}