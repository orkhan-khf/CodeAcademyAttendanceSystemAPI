using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAcademyAttendanceSystemAPI.Models
{
    public class AntiForgeryToken
    {
        public string GetToken()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            string token = "";

            return "";
        }
    }
}