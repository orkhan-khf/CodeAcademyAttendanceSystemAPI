using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using CodeAcademyAttendanceSystemAPI.Models;
using CodeAcademyAttendanceSystem.Models.PasswordSecurity;
using System.Text;

namespace CodeAcademyAttendanceSystemAPI.Controllers
{
    public class StudentController : ApiController
    {
        CodeAcademyAttendanceSystem_dbEntities db = new CodeAcademyAttendanceSystem_dbEntities();

        //ApproveAttendance Action'da istifadə edilən property'lər
        DateTime today = Convert.ToDateTime(DateTime.Now.ToShortDateString());
        //CheckStudentLogin Action'da istifadə edilən property'lər
        string returnObject;
        //CheckStudentIpAddress Action'da istifadə edilən property'lər
        string CodeAcademyIp = "82.194.17.75";



        //Tələbə mobil application ilə giriş edəndə
        [HttpGet]
        public HttpResponseMessage CheckStudentLogin(string student_email, string student_password, string student_device_id)
        {
            //URL'dən gələn email və password dəyərləri boşdursa (və ya null)...
            if (student_email == null || student_email == "" || student_password == null || student_password == "")
            {
                returnObject = "{\"message\" : \"true\" }";
            }

            //Əgər tələbənin cihazının ID adresi oxunmadısa...
            //if(student_device_id == null || student_device_id == "")
            //{
            //    return "Cihazın ID adresi oxunmadı!";
            //}

            ////Tələbə Code Academy'dən kənarda hesaba daxil olmağa cəhd edirsə...
            //if (CheckStudentIpAddress() != CodeAcademyIp)
            //{
            //    return "Xahiş edirik Code Academy'nin Wifi'ı ilə daxil olun";
            //}

            ////Databazadan form'dan gələn email'ə uyğun emailı seç
            //Students check_logged_student_informations = db.Students.Where(s => s.student_email == student_email).FirstOrDefault();

            ////Əgər Form'dan gələn email'ə uyğun nəticə tapılmadısa...
            //if (check_logged_student_informations == null)
            //{
            //    return "Email düzgün daxil edilməyib!";
            //}

            ////Əgər email'in şifrəsi düzgün daxil edilməyibsə...
            //if (!PasswordStorage.VerifyPassword(student_password, check_logged_student_informations.student_password))
            //{
            //    return "Şifrə düzgün daxil edilməyib!";
            //}

            ////Əgər tələbə hesabına ilk dəfə (və ya resetləndikdən sonra ilk dəfə) daxil olursa...
            //if (check_logged_student_informations.student_first_login == true)
            //{
            //    return "SetNewPassword";
            //}

            ////Əgər tələbənin giriş etməyə çalışdığı cihaz ilk dəfə giriş etdiyi cihaz deyilsə...
            //if (check_logged_student_informations.student_device_id != student_device_id)
            //{
            //    return "Xahiş edirik öz cihazınızla daxil olun (Əgər cihazınızı dəyişmisinizsə, zəhmət olmasa Reseptiona bildirin.)";
            //}

            ////Əgər tələbə müvəffəqiyyətlə giriş edibsə...
            //if (check_logged_student_informations.student_id > 0)
            //{
            //    return check_logged_student_informations.student_name + " " + check_logged_student_informations.student_surname;
            //}

            ////Əgər yuxarıdakı şərtlərin heç biri ödənmirsə...

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(returnObject, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpGet]
        public string ApproveAttendance(int student_id, string qr_code)
        {
            if (CheckStudentIpAddress() != CodeAcademyIp)
            {
                return "Student is away from Academy!";
            }
            try
            {
                int exiting_user_id = (from s in db.Students
                                   join g in db.Groups on s.student_group_id equals g.group_id
                                   join q in db.Qr_Codes on s.student_group_id equals q.qr_codes_group_id
                                   where s.student_id == student_id
                                   && q.qr_codes_status == true
                                   && q.qr_codes_date == today
                                   && q.qr_codes_value == qr_code
                                   select new
                                   {
                                       s.student_id
                                   }).First().student_id;
                if (exiting_user_id > 0)
                {
                    Students_Attendance student_attendance = db.Students_Attendance.Where(a => a.students_attendance_date == today && a.students_attendance_status == false && a.students_attendance_student_id == exiting_user_id).First();
                    student_attendance.students_attendance_status = true;
                    student_attendance.students_attendance_sender_ip = CheckStudentIpAddress();
                    db.SaveChanges();
                }
                return "Student is in Academy.";
            }
            catch
            {

            }
            return "Error";
        }
        public string CheckStudentIpAddress()
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
            if (ipAddress.Length < 5)
            {
                ipAddress = "82.194.17.75";
            }
            return ipAddress;
        }
    }
}
