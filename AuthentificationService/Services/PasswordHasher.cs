using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IPasswordHasher
    {
        public string Encrypt(string password);
        public bool VerifyPassword(string password, string hashedPassword);

    }
    public interface IRsaCipher : IDisposable
    {
        string DecodeRSA(string encryptedBase64);
        string EncodeRSA(string plainText);
        
    }
    public class PasswordHasher : IPasswordHasher
    {
        private readonly RSACryptoServiceProvider _rsa;
        public string PublicKeyRSA { get; private set; }
        public PasswordHasher()
        {
            
        }
        public PasswordHasher(RSACryptoServiceProvider rsa)
        {
            _rsa = rsa ?? throw new ArgumentNullException(nameof(rsa));
            PublicKeyRSA = Convert.ToBase64String(_rsa.ExportRSAPublicKey());
        }

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
