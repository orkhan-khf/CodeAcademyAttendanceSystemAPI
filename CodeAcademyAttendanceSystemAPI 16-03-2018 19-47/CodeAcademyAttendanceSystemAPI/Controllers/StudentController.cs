using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using CodeAcademyAttendanceSystemAPI.Models;
using CodeAcademyAttendanceSystem.Models.PasswordSecurity;

namespace CodeAcademyAttendanceSystemAPI.Controllers
{
    public class StudentController : ApiController
    {
        CodeAcademyAttendanceSystem_dbEntities db = new CodeAcademyAttendanceSystem_dbEntities();
        DateTime today = Convert.ToDateTime(DateTime.Now.ToShortDateString());
        string CodeAcademyIp = "82.194.17.75";

        [HttpGet]
        public string CheckStudentLogin(string student_email, string student_password)
        {
            if (CheckStudentIpAddress() != CodeAcademyIp)
            {
                return "Student is away from Academy!";
            }
            
            Students check_student_email = db.Students.Where(s => s.student_email == student_email).FirstOrDefault();

            if (check_student_email == null)
            {
                return "Email düzgün daxil edilməyib!";
            }

            if (!PasswordStorage.VerifyPassword(student_password, check_student_email.student_password))
            {
                return "Şifrə düzgün daxil edilməyib!";
            }
            if (check_student_email.student_id > 0)
            {
                return check_student_email.student_name + " " + check_student_email.student_surname;
            }
            return "Checking Error";
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
