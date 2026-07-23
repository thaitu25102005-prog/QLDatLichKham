using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace QLBV.Filters
{
    public class AdminAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["Role"] == null || filterContext.HttpContext.Session["Role"].ToString() != "Admin")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Login", action = "Login" })
                );
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class BSAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["Role"] == null || filterContext.HttpContext.Session["Role"].ToString() != "BS")
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Login", action = "Login" })
                );
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
