namespace Hangfire.Dashboard.BasicAuthorization
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;

    public class BasicAuthAuthorizationUser
    {
        public bool Validate(string login, string password, bool loginCaseSensitive)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                throw new ArgumentNullException("login");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("password");
            }
            if (login.Equals(Login, loginCaseSensitive ? (StringComparison.CurrentCulture) :StringComparison.OrdinalIgnoreCase))
            {
                using (SHA1 sha = SHA1.Create())
                {
                    byte[] x = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return StructuralComparisons.StructuralEqualityComparer.Equals(x, Password);
                }
            }
            return false;
        }

        public string Login { get; set; }

        public byte[] Password { get; set; }

        public string PasswordClear
        {
            set
            {
                using (SHA1 sha = SHA1.Create())
                {
                    Password = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
                }
            }
        }
    }
}

