using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BraintreeASPExample
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "DropIn-New",
                url: "DropIn/new",
                defaults: new { controller = "DropIn", action = "New" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[]{"GET"}) }
            );

             routes.MapRoute(
                name: "DropIn",
                url: "DropIn",
                defaults: new { controller = "DropIn", action = "New" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[]{"GET"}) }
            );

            routes.MapRoute(
                name: "DropIn-Create",
                url: "DropIn",
                defaults: new { controller = "DropIn", action = "Create" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[] { "POST" }) }
            );

            routes.MapRoute(
                name: "DropIn-Show",
                url: "DropIn/{id}",
                defaults: new { controller = "DropIn", action = "Show" }
            );

            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}