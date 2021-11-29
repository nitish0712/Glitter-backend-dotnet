using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlitterTweeting.Business
{
    public class PasswordHasher
    {
        private static string GetRandomSalt() => BCrypt.Net.BCrypt.GenerateSalt(12);

        
        public static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password, GetRandomSalt());

        
        public static bool ValidatePassword(string password, string correctHash) => BCrypt.Net.BCrypt.Verify(password, correctHash);

    }
}
