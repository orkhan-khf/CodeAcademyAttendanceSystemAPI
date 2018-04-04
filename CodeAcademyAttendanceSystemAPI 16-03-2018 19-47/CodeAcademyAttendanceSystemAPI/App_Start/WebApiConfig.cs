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

            //Tələbənin giriş url'si...
            config.Routes.MapHttpRoute(
                name: "Login",
                routeTemplate: "api/{controller}/Login/{student_email}/{student_password}/{student_device_id}/{token}"
            );

            //Tələbənin şifrə təyin etmə url'si...
            config.Routes.MapHttpRoute(
                name: "SetNewPassword",
                routeTemplate: "api/{controller}/SetNewPassword/{student_id}/{student_new_password}/{student_device_id}/{token}"
            );

            //Tələbənin profil məlumatlarının çəkildiyi url...
            config.Routes.MapHttpRoute(
                name: "StudentProfile",
                routeTemplate: "api/{controller}/StudentProfile/{student_id}/{student_device_id}/{token}"
            );

            //Tələbənin davamiyyəti haqqda ətraflı məlumatın çəkildiyi url...
            config.Routes.MapHttpRoute(
                name: "StudentAttendanceList",
                routeTemplate: "api/{controller}/StudentAttendanceList/{token}/{device_id}/{student_id}"
            );

            //Tələbənin Qr kod təsdiq etmə url'si...
            config.Routes.MapHttpRoute(
                name: "ApproveAttendance",
                routeTemplate: "api/{controller}/ApproveAttendance/{student_id}/{student_device_id}/{qr_code}/{token}"
            );

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }

    }
}
