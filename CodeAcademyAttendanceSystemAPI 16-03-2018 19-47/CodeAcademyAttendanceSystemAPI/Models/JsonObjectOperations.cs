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
    public class JsonObjectOperations : ApiController
    {
        private string JsonString;

        //Json object'i yaradıb return edir.
        public HttpResponseMessage JsonGenerator(HttpResponseMessage response, bool success_status, string SuccessMessage, string ErrorMessage, bool SetNewPassword)
        {
            //Əgər SuccessMessage null gəlibsə dəyərinidə string tipində null yazdır...
            if(SuccessMessage == null)
            {
                SuccessMessage = "null";
            }
            //Əgər ErrorMessage null gəlibsə dəyərinidə string tipində null yazdır...
            if (ErrorMessage == null)
            {
                ErrorMessage = "null";
            }
            //Json obyektini string formatında düzəlt...
            JsonString = "{" +
                                    "\"Success\" : " + success_status.ToString().ToLower() + "," +
                                    "\"SuccessMessage\" : " + SuccessMessage + "," +
                                    "\"ErrorMessage\" : " + ErrorMessage + "," +
                                    "\"SetNewPassword\" : " + SetNewPassword.ToString().ToLower() + " " +
                                "}";
            //String tipindəki Json'u Json formatında return et...
            response.Content = new StringContent(JsonString, Encoding.UTF8, "application/json");
            return response;
        }



        //1. OVERLOAD
        //Login action'da SuccessMessage əvəzinə StudentId göndərə bilmək üçün overload yazıldı...
        public HttpResponseMessage JsonGenerator(HttpResponseMessage response, bool success_status, int StudentId, string ErrorMessage, bool SetNewPassword)
        {
            //Əgər ErrorMessage null gəlibsə dəyərinidə string tipində null yazdır...
            if (ErrorMessage == null)
            {
                ErrorMessage = "null";
            }
            //Json obyektini string formatında düzəlt...
            JsonString = "{" +
                                    "\"Success\" : " + success_status.ToString().ToLower() + "," +
                                    "\"StudentId\" : " + StudentId + "," +
                                    "\"ErrorMessage\" : " + ErrorMessage + "," +
                                    "\"SetNewPassword\" : " + SetNewPassword.ToString().ToLower() + " " +
                                "}";
            //String tipindəki Json'u Json formatında return et...
            response.Content = new StringContent(JsonString, Encoding.UTF8, "application/json");
            return response;
        }
    }
}