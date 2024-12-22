﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteBanQuanAo.Filters
{
    public class UserAuthorization : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.IsInRole("admin") == true)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { area = "Admin", controller = "Home", action = "Index" }
                    )
                );
            }
        }
    }
}