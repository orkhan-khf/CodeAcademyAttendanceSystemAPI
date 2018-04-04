using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CodeAcademyAttendanceSystemAPI.Models
{
    public class AntiForgeryToken
    {
        //Android application'da da eyni dəyər şifrələmə zamanı istifadə olunub DƏYİŞDİRİLMƏMƏLİDİR !!!
        private const string specificKey = "5sda01bg350cx1g2b5gv1x32fs4d5f21gfsd1f25as6d1f56as1dgv651f23v15df31vb65df4153dsad1v54sv18v54zs35b4135zd4b15";

        //Tələbənin url'dəki id parametri ilə specifickeyi birləşdirib hash'layır
        public bool Verify(int student_id, string token_hash)
        {
            string input = specificKey + student_id.ToString();

            var bytes = Encoding.UTF8.GetBytes(input);

            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                {
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                }

                if (hashedInputStringBuilder.ToString().ToLower() == token_hash)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        //2. Overload 
        //Tələbənin url'dəki email parametri ilə specifickeyi birləşdirib hash'layır (sadəcə logində istifadə olunur)
        public bool Verify(string student_email, string token_hash)
        {
            string input = specificKey + student_email.ToString();

            var bytes = Encoding.UTF8.GetBytes(input);

            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                {
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                }

                if (hashedInputStringBuilder.ToString().ToLower() == token_hash)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}