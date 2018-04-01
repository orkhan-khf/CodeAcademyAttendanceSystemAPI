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
        private const string specificKey = "fdh2tr5h12xfg65j12gc89h1j98gc6j2g8j2cgh";

        public bool Verify(int random_number, string token_hash)
        {
            string input = specificKey + (random_number / 2);
            
            var bytes = Encoding.UTF8.GetBytes(input);

            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);
                var hashedInputStringBuilder = new StringBuilder(128);
                foreach (var b in hashedInputBytes)
                {
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                }

                if (hashedInputStringBuilder.ToString() == token_hash)
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