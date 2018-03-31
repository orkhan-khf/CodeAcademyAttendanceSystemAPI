using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CodeAcademyAttendanceSystemAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "Login",
                routeTemplate: "api/{controller}/Login/{student_email}/{student_password}/{student_device_id}"
            );
            config.Routes.MapHttpRoute(
                name: "SetNewPassword",
                routeTemplate: "api/{controller}/SetNewPassword/{student_email}/{student_password}/{student_new_password}/{student_device_id}"
            );
            config.Routes.MapHttpRoute(
                name: "ApproveAttendance",
                routeTemplate: "api/{controller}/ApproveAttendance/{student_email}/{student_password}/{student_device_id}/{qr_code}"
            );
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }

    }
}
