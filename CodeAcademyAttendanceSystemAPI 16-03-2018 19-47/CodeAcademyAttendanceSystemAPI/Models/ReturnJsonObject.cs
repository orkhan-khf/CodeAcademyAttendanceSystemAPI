using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace CodeAcademyAttendanceSystemAPI.Models
{
    public class ReturnJsonObject : ApiController
    {
        //string tipindəki JSON'u JSON tipində return edir
        public HttpResponseMessage StringToJson(string JsonString, HttpResponseMessage response)
        {
            response.Content = new StringContent(JsonString, Encoding.UTF8, "application/json");
            return response;
        }
    }
}