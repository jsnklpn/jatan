using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JatanWebApp.Controllers
{
    /// <summary>
    /// Base controller that checks for user credentials.
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // get controller & action names
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            // Ensure user is logged in & not trying to visit login page
            //string user = context.HttpContext.Session["User"] as string;
            //if (user == null && actionName != "Login")
            //{
            //    // not logged in; redirect to login page
            //    context.Result = RedirectToAction("Login", "User", new { url = Request.RawUrl });
            //    return;
            //}
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }
    }
}