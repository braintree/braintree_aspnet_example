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

            // Drop-In Routes
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

            //routes.MapRoute(
            //    name: "DropIn-Show",
            //    url: "DropIn/{id}",
            //    defaults: new { controller = "DropIn", action = "Show" }
            //);

            // Hosted Fields Routes
            routes.MapRoute(
                name: "Hosted-New",
                url: "Hosted/new",
                defaults: new { controller = "Hosted", action = "New" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[] { "GET" }) }
            );

            routes.MapRoute(
                name: "Hosted",
                url: "Hosted",
                defaults: new { controller = "Hosted", action = "New" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[] { "GET" }) }
            );

            routes.MapRoute(
                name: "Hosted-Create",
                url: "Hosted",
                defaults: new { controller = "Hosted", action = "Create" },
                constraints: new { httpMethod = new HttpMethodConstraint(new string[] { "POST" }) }
            );

            // Show Transaction retults
            routes.MapRoute(
                name: "Show",
                url: "Show/{id}",
                defaults: new { controller = "Show", action = "Index" }
            );

            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}