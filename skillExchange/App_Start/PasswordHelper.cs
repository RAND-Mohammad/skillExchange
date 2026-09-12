using System;
using System.Security.Cryptography;

namespace skillExchange.App_Start
{
    public static class PasswordHelper
    {
        private const int Iterations = 100000;
        public static string Hash(string password)
        {
            byte[] salt = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return "PBKDF2$" + Iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(pbkdf2.GetBytes(32));
            }
        }

        public static bool Verify(string password, string storedValue)
        {
            if (String.IsNullOrWhiteSpace(storedValue) || !storedValue.StartsWith("PBKDF2$")) return false;
            string[] parts = storedValue.Split('$');
            if (parts.Length != 4) return false;
            try
            {
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expected = Convert.FromBase64String(parts[3]);
                using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, Convert.ToInt32(parts[1]))) return FixedTimeEquals(expected, pbkdf2.GetBytes(expected.Length));
            }
            catch { return false; }
        }

        public static bool IsLegacyPlainText(string value) { return !String.IsNullOrWhiteSpace(value) && !value.StartsWith("PBKDF2$"); }
        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left.Length != right.Length) return false;
            int difference = 0;
            for (int i = 0; i < left.Length; i++) difference |= left[i] ^ right[i];
            return difference == 0;
        }
    }
}
