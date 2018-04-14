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

        JsonObjectOperations JsonObjectOperations = new JsonObjectOperations();

        StudentIPAddress StudentIPAddress = new StudentIPAddress();

        AntiForgeryToken AntiForgeryToken = new AntiForgeryToken();

        //ApproveAttendance Action'da istifadə edilən property'lər
        DateTime today = Convert.ToDateTime(DateTime.Now.ToShortDateString());

        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------

        //Tələbə mobil application ilə giriş edəndə
        [HttpGet]
        public HttpResponseMessage Login(string student_email, string student_password, string student_device_id, string token)
        {
            // Aşağıda ReturnJsonObject methoduna parametr kimi ötürmək üçün HttpResponseMessage tipində bir cavab yaradılır...
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Tokeni yoxlayır (AntiForgeryToken Class'da ətraflı yazılıb)
            if (!AntiForgeryToken.Verify(student_email, token))
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Giriş icazəsi verilmədi!\"", false);
            }

            //URL'dən gələn email və password dəyərləri boşdursa (və ya null)...
            if (student_email == null || student_email == "" || student_password == null || student_password == "")
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Email və ya şifrə daxil edilməyib!\"", false);
            }

            //Əgər tələbənin cihazının ID adresi oxunmadısa...
            if (student_device_id == null || student_device_id == "")
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Cihazın ID adresi oxunmadı!\"", false);
            }

            //Databazadan form'dan gələn email'ə uyğun emailı seç
            Students check_logged_student_informations = db.Students.Where(s => s.student_email == student_email).FirstOrDefault();

            //Əgər Form'dan gələn email'ə uyğun nəticə tapılmadısa...
            if (check_logged_student_informations == null)
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Email düzgün daxil edilməyib!\"", false);
            }

            //Əgər email'in şifrəsi düzgün daxil edilməyibsə...
            if (!PasswordStorage.VerifyPassword(student_password, check_logged_student_informations.student_password))
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Şifrə düzgün daxil edilməyib!\"", false);
            }

            //Əgər tələbə hesabına ilk dəfə (və ya resetləndikdən sonra ilk dəfə) daxil olursa...
            if (check_logged_student_informations.student_first_login == true)
            {
                int student_id = check_logged_student_informations.student_id;
                return JsonObjectOperations.JsonGenerator(response, true, student_id, null, true);
            }

            //Əgər tələbənin giriş etməyə çalışdığı cihaz ilk dəfə giriş etdiyi cihaz deyilsə...
            if (check_logged_student_informations.student_device_id != student_device_id)
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Xahiş edirik öz cihazınızla daxil olun (Əgər cihazınızı dəyişmisinizsə, zəhmət olmasa müəlliminizə bildirin.)\"", false);
            }

            //Tələbənin hesabı bağlıdırsa
            if(check_logged_student_informations.student_status != true)
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Sizin hesabınız bağlıdır!\"", false);
            }

            //Əgər tələbə müvəffəqiyyətlə giriş edibsə...
            if (check_logged_student_informations.student_id > 0)
            {
                int student_id = check_logged_student_informations.student_id;
                
                return JsonObjectOperations.JsonGenerator(response, true, student_id, null, false);
            }

            //Əgər yuxarıdakı şərtlərin heç biri ödənmirsə...
            return JsonObjectOperations.JsonGenerator(response, false, null, "\"CA216587: Xəta baş verdi!\"", false);
        }


        [HttpGet]
        public HttpResponseMessage SetNewPassword(int student_id, string student_new_password, string student_device_id, string token)
        {
            // Aşağıda ReturnJsonObject methoduna parametr kimi ötürmək üçün HttpResponseMessage tipində bir cavab yaradılır...
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Tokeni yoxlayır (AntiForgeryToken Class'da ətraflı yazılıb)
            if (!AntiForgeryToken.Verify(student_id, token))
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Giriş icazəsi verilmədi!\"", false);
            }

            //URL'dən student_id cihazın ID dəyərləri boşdursa (və ya null)...
            if (student_id < 0 || student_device_id == null || student_device_id == "")
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Xahiş edirik programı bağlayıb təkrar giriş edərək yenidən cəhd edin\"", false);
            }

            //Form'dan gələn id'ə uyğun tələbəni seç...
            Students select_student_information_for_reset_password = db.Students.Where(s => s.student_id == student_id).FirstOrDefault();

            //Əgər Form'dan gələn Id'ə uyğun nəticə tapılmadısa...
            if (select_student_information_for_reset_password == null)
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Məlumatlar düzgün deyil! Xahiş edirik programı bağlayıb təkrar giriş edərək yenidən cəhd edin\"", false);
            }

            //Əgər tələbə şifrəsini daha öncədən təyin edibsə (Şifrəsini unudan tələbələrin hesabında admin tərəfindən şifrə resetlənəcək şifrə bərpa etmək üçün)...
            if (select_student_information_for_reset_password.student_first_login == false)
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Sizin şifrəniz artıq təyin edilib! Şifrənizi sıfırlamaq üçün müəlliminizə bildirin\"", false);
            }

            //Tələbənin şifrəsini update et...
            try
            {
                select_student_information_for_reset_password.student_password = PasswordStorage.CreateHash(student_new_password);
                select_student_information_for_reset_password.student_device_id = student_device_id;
                select_student_information_for_reset_password.student_first_login = false;
                db.SaveChanges();

                return JsonObjectOperations.JsonGenerator(response, true, "\"Şifrəniz müvəffəqiyyətlə yeniləndi. Zəhmət olmasa yeni şifrənizlə hesabınıza daxil olun\"", null, false);
            }
            catch
            {
                //Əgər yuxarıdakı şərtlərin heç biri ödənmirsə...
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"CA127953: Xəta baş verdi!\"", false);
            }
        }


        [HttpGet]
        public HttpResponseMessage StudentProfile(int student_id, string student_device_id, string token)
        {
            // Aşağıda ReturnJsonObject methoduna parametr kimi ötürmək üçün HttpResponseMessage tipində bir cavab yaradılır...
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Tokeni yoxlayır (AntiForgeryToken Class'da ətraflı yazılıb)...
            if (!AntiForgeryToken.Verify(student_id, token))
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Giriş icazəsi verilmədi!\"", false);
            }

            //Default null olur, əgər tələbənin grupunda dərs keçirilibsə aşağıda tələbənin davamiyyət nisbətini hesablayır...
            string AttendanceRatio = "null";
            
            //Databazadan, form'dan gələn student_id'ə uyğun tələbəni seç...
            Students check_logged_student_informations = db.Students.Where(s => s.student_id == student_id).FirstOrDefault();

            //Əgər Id'ə uyğun tələbə tapılmadısa...
            if(check_logged_student_informations == null)
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Tələbə məlumatları düzgün deyil!\"", false);
            }

            //Əgər tələbənin grupunda dərs keçirilibsə...
            if (check_logged_student_informations.Students_Attendance.Count() > 0)
            {
                //Tələbənin dərslərdə iştirak nisbətini 100 üzərindən hesabla...
                AttendanceRatio = "\"100 / " + (((double)check_logged_student_informations.Students_Attendance.Where(a => a.students_attendance_status == true).Count() * 100) / (double)check_logged_student_informations.Students_Attendance.Count()).ToString("#.##") + "\"";
            }

            //JsonGenerator method'na göndərmək üçün string tipində Json strukturu hazırla...
            string StudentProfile = "{" +
                                         "\"Id\" : " + check_logged_student_informations.student_id + "," +
                                         "\"Name\" : \"" + check_logged_student_informations.student_name + "\"," +
                                         "\"Surname\" : \"" + check_logged_student_informations.student_surname + "\"," +
                                         "\"FatherName\" : \"" + check_logged_student_informations.student_father_name + "\"," +
                                         "\"Phone\" : \"" + check_logged_student_informations.student_phone + "\"," +
                                         "\"Gender\" : \"" + check_logged_student_informations.Genders.gender_name + "\"," +
                                         "\"Grup\" : \"" + check_logged_student_informations.Groups.group_name + "\"," +
                                         "\"Group_Schedule\" : \"" + check_logged_student_informations.Groups.Lesson_Times.Group_Schedule.group_schedule_name + "\"," +
                                         "\"LessonTime\" : \"" + check_logged_student_informations.Groups.Lesson_Times.lesson_times_name + "\"," +
                                         "\"LessonBeginTime\" : \"" + check_logged_student_informations.Groups.Lesson_Times.lesson_times_start_time.Value.ToString("hh\\:mm") + "\"," +
                                         "\"LessonEndTime\" : \"" + check_logged_student_informations.Groups.Lesson_Times.lesson_times_end_time.Value.ToString("hh\\:mm") + "\"," +
                                         "\"AttendanceRatio\" : " + AttendanceRatio +
                                    "}";

            return JsonObjectOperations.JsonGenerator(response, true, StudentProfile, null, false);
        }


        [HttpGet]
        public HttpResponseMessage StudentAttendanceList(string token, string device_id, int student_id)
        {
            // Aşağıda ReturnJsonObject methoduna parametr kimi ötürmək üçün HttpResponseMessage tipində bir cavab yaradılır...
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            //Tokeni yoxlayır (AntiForgeryToken Class'da ətraflı yazılıb)...
            if (!AntiForgeryToken.Verify(student_id, token))
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Giriş icazəsi verilmədi!\"", false);
            }

            //Databazadan, form'dan gələn student_id'ə uyğun tələbəni seç...
            Students check_logged_student_informations = db.Students.Where(s => s.student_id == student_id).FirstOrDefault();

            //Əgər Id'ə uyğun tələbə tapılmadısa...
            if (check_logged_student_informations == null)
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Tələbə məlumatları düzgün deyil!\"", false);
            }

            string StudentAttendance, AttendanceInfo;
            //Əgər tələbənin grupunda dərs keçirilibsə...
            if (check_logged_student_informations.Students_Attendance.Count() > 0)
            {
                //Tələbənin gəldiyi/gəlmədiyi günləri tarix:status formatında çıxartmaq üçün Json strukturu qur......
                StudentAttendance = "[";
                //Tələbənin gəldiyi/gəlmədiyi günlərin tarixlərini və statusunu foreach ilə Json obyektinə doldur...
                foreach (var item in check_logged_student_informations.Students_Attendance.OrderByDescending(d => d.students_attendance_date))
                {
                    StudentAttendance += "{\"AttendanceDate\" : " + "\"" + item.students_attendance_date.Value.ToShortDateString() + "\"" + ",";
                    StudentAttendance += "\"AttendanceStatus\" : " + item.students_attendance_status.ToString().ToLower() + "},";
                }
                //Json obyektindən ən sonuncu vergülü götür (error çıxartmasın deyə)...
                StudentAttendance = StudentAttendance.Substring(0, StudentAttendance.Length - 1);
                //Json obyektini bağla...
                StudentAttendance += "]";

                //Tələbənin neçə dərsin neçəsində iştirak etdiyini hesabla...
                AttendanceInfo = "\"Siz ümumi " + check_logged_student_informations.Students_Attendance.Count() + " dərsdən " + check_logged_student_informations.Students_Attendance.Where(s => s.students_attendance_status == true).Count() + " dərsdə iştirak etmisiniz.\"";
            }
            else
            {
                StudentAttendance = "[{\"AttendanceDate\": null}]";
                AttendanceInfo = "null";
            }
            StudentAttendance = "{" +
                                             "\"AttendanceTable\" : " + StudentAttendance + "," +
                                             "\"AttendanceInfo\" : " + AttendanceInfo +
                                        "}";
            return JsonObjectOperations.JsonGenerator(response, true, StudentAttendance, null, false);

        }


        [HttpGet]
        public HttpResponseMessage ApproveAttendance(int student_id, string student_device_id, string qr_code, string token)
        {
            // Aşağıda ReturnJsonObject methoduna parametr kimi ötürmək üçün HttpResponseMessage tipində bir cavab yaradılır...
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            TimeSpan time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));

            //Tokeni yoxlayır (AntiForgeryToken Class'da ətraflı yazılıb)
            if (!AntiForgeryToken.Verify(student_id, token))
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Giriş icazəsi verilmədi!\"", false);
            }

            //Tələbə Code Academy'dən kənarda QR kodu təsdiqləmək istəyirsə...
            if (!StudentIPAddress.Check())
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Siz Code Academy'nin Wifi'na qoşulmamısınız!\"", false);
            }

            //URL'dən gələn student_id, cihazın ID və ya token parametrləri boşdursa (və ya null)...
            if (student_id < 0 || student_device_id == null || student_device_id == "" || token == null || token == "")
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Xahiş edirik programı bağlayıb təkrar giriş edərək yenidən cəhd edin\"", false);
            }

            //URL'dən gələn Qr kod dəyəri boşdursa (və ya null)...
            if (qr_code == null || qr_code == "")
            {
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Qr Kod oxunmadı!\"", false);
            }

            try
            {
                int std_id = Convert.ToInt32(student_id);

                //URL'dən gələn məlumatlara əsasən tələbənin məlumatlarını al...
                var student_info = (from s in db.Students
                                   join g in db.Groups on s.student_group_id equals g.group_id
                                   join q in db.Qr_Codes on s.student_group_id equals q.qr_codes_group_id
                                   where s.student_id == std_id && s.student_device_id == student_device_id && q.qr_codes_date == today
                                   select new
                                   {
                                       s.student_id,
                                       g.group_name,
                                       q.qr_codes_status,
                                       q.qr_codes_value,
                                       q.qr_codes_date,
                                       q.qr_code_deadline_time
                                   }).FirstOrDefault();
                
                //Əgər yuxarıdakı sorğu null gəlibsə, deməli bu gün tələbənin olduğu grup üçün Qr kod generate olunmayıb
                if (student_info == null)
                {
                    return JsonObjectOperations.JsonGenerator(response, false, null, "\"Sizin grupda bu günə aid Qr kod tapılmadı\"", false);
                }

                //Əgər tələbənin kamera ilə oxuduğu Qr kod səhvdirsə
                if(student_info.qr_codes_value != qr_code)
                {
                    return JsonObjectOperations.JsonGenerator(response, false, null, "\"Qr kod səhvdir!\"", false);
                }

                //Əgər Qr kodun generate olunan anda databazaya yazılan deadline vaxtından sonra tələbə təsdiq edərsə...
                if (student_info.qr_code_deadline_time < time)
                {
                    Qr_Codes ChangeQrCodeStatus = db.Qr_Codes.Where(q => q.qr_codes_date == today && q.qr_codes_value == student_info.qr_codes_value).FirstOrDefault();
                    ChangeQrCodeStatus.qr_codes_status = false;

                    return JsonObjectOperations.JsonGenerator(response, false, null, "\"Bu Qr kod artıq etibarlı deyil!\"", false);
                }

                //Əgər tələbənin oxuduğu Qr kod varsa, lakin artıq etibarlı deyilsə (dərsə gecikibsə)...
                if (student_info.qr_codes_status != true)
                {
                    return JsonObjectOperations.JsonGenerator(response, false, null, "\"Bu Qr kod artıq etibarlı deyil!\"", false);
                }

                //Əgər bu günə aid Qr kod varsa...
                if (student_info != null && student_info.qr_codes_status == true && student_info.qr_codes_value == qr_code && student_info.qr_codes_date == today)
                {
                    //Students_Attendance table'dan bu günə və bu tələbəyə aid olan sətiri seç...
                    Students_Attendance student_attendance = db.Students_Attendance.Where(a => a.students_attendance_date == today && a.students_attendance_student_id == student_info.student_id).First();
                    
                    //Əgər tələbə bu Qr kodu artıq bu gün bir dəfə təsdiq edibsə...
                    if (student_attendance.students_attendance_status == true)
                    {
                        return JsonObjectOperations.JsonGenerator(response, false, null, "\"Siz bu gün dərsdə iştirak etdiyinizi artıq təsdiq etmisiniz\"", false);
                    }

                    //Tələbənin Attendance'nın statusunu true et (yəni dərsdədir Qr kod oxunub) və Qr kodu oxuduğu İp adresini yaz DB ya
                    student_attendance.students_attendance_status = true;
                    student_attendance.students_attendance_sender_ip = StudentIPAddress.Get();
                    db.SaveChanges();
                    
                    return JsonObjectOperations.JsonGenerator(response, true, "\"Sizin dərsdə iştirak etdiyiniz müvəffəqiyyətlə təsdiqləndi\"", null, false);
                }
                //Əgər yuxarıdakı şərtlərin heç biri ödənmirsə...
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Xahiş edirik programı bağlayıb təkrar giriş edərək yenidən cəhd edin\"", false);
            }
            catch
            {
                //Əgər yuxarıdakı şərtlərin heç biri ödənmirsə...
                return JsonObjectOperations.JsonGenerator(response, false, null, "\"Xahiş edirik programı bağlayıb təkrar giriş edərək yenidən cəhd edin!\"", false);
            }
        }
    }
}
