using System.Security.Cryptography;
using System.Text;

namespace MedicalDashboardAPI.Helpers
{
    public static class PasswordHelper
    {
        public static void CreatePasswordHash(
            string password,
            out byte[] passwordHash,
            out byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty");

            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPasswordHash(
            string password,
            byte[] storedHash,
            byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (storedHash.Length != 64) return false; // 512-bit hash
            if (storedSalt.Length != 128) return false; // 1024-bit salt

            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) return false;
            }

            return true;
        }
    }
}