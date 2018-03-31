using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeAcademyAttendanceSystemAPI.Models;

namespace CodeAcademyAttendanceSystemAPI.Models
{
    public class StudentIPAddress
    {
        CodeAcademyAttendanceSystem_dbEntities db = new CodeAcademyAttendanceSystem_dbEntities();

        //Tələbənin giriş etdiyi IP adresi ilə databazadakı (System_Settings > system_settings_academy_ip) Code Academynin IP adresi ilə qarşılaşdırır
        public bool Check()
        {
            //Code Academynin IP adresini databazadan çək...
            string CodeAcademyIpAddress = db.System_Settings.Select(s=>s.system_settings_academy_ip).FirstOrDefault();

            //Request gələn IP adresini ipAddress dəyişəninə bərabər et...
            var ipAddress = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            else if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null && HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"].Length != 0)
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
                ipAddress = HttpContext.Current.Request.UserHostName;
            int index = ipAddress.IndexOf(":");
            if (index > 0)
                ipAddress = ipAddress.Substring(0, index);

            //Əgər request gələn IP adress Code Academynin IP adresinə bərabərdirsə (deyilsə else'ə gir)...
            if(ipAddress == CodeAcademyIpAddress)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Tələbənin IP adresini string kimi return et (Databazaya yazdırmaq və s. üçün lazımdır)...
        public string Get()
        {
            var ipAddress = string.Empty;
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            else if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null && HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"].Length != 0)
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
            else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
                ipAddress = HttpContext.Current.Request.UserHostName;
            int index = ipAddress.IndexOf(":");
            if (index > 0)
                ipAddress = ipAddress.Substring(0, index);
                return ipAddress;
        }
    }
}