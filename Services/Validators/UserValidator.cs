using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services.Validators
{
    public class UserValidator
    {
        public bool ValidateEmail(string email)
        {
            if(string.IsNullOrWhiteSpace(email)) return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        public bool ValidatePassword(string password) 
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;
            return true;
            //var PasswordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[^\w\s]).{8,}$";
            //return Regex.IsMatch(password, PasswordPattern);
        }
        public bool ValidateFullname(string fullname)
        {
            var regex = new Regex(@"^[а-яА-Яa-zA-Z\-]{2,}\s+[а-яА-Яa-zA-Z\-]{2,}(\s+[а-яА-Яa-zA-Z\-]{2,})?$");
            return !string.IsNullOrWhiteSpace(fullname) &&
              regex.IsMatch(Regex.Replace(fullname.Trim(), @"\s+", " "));
        }
        
    }
}
