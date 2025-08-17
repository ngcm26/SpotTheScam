using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SpotTheScam.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // MVC route for the chatbot
            routes.MapRoute(
                name: "ChatBot",
                url: "chat/{action}/{id}",
                defaults: new { controller = "Chat", action = "Index", id = UrlParameter.Optional }
            );
        }

    }
}