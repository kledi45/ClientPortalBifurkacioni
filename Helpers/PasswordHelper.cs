using System.Security.Cryptography;
using System.Text;

namespace ClientPortalBifurkacioni.Helpers
{
    public class PasswordHelper
    {
        public static string GenerateSalt()
        {
            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPassword(string password, string salt)
        {
            var combined = password + salt;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(bytes);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedSalt, string storedHash)
        {
            string hashOfInput = HashPassword(enteredPassword, storedSalt);
            return hashOfInput == storedHash;
        }
    }
}
