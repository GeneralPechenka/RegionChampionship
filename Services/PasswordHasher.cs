using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IPasswordHasher
    {
        public string Encrypt(string password);
        public bool VerifyPassword(string password, string hashedPassword);

    }
    public class PasswordHasher : IPasswordHasher
    {

        public string Encrypt(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
        }

    }
}
