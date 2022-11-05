using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace JatanWebApp.Controllers
{
    /// <summary>
    /// Base controller that checks for user credentials.
    /// </summary>
    public abstract class BaseController : Microsoft.AspNetCore.Mvc.Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        protected ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
        }

        protected ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        protected IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        protected override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
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

        protected Microsoft.AspNetCore.Mvc.ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}