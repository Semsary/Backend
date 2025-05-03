using System.Security.Cryptography;

namespace semsary_backend.Service
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            // Generate a random salt (16 bytes)
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // Derive key using PBKDF2 (100,000 iterations)
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32); // 32 bytes = 256 bits

            // Combine salt + hash into 1 array
            byte[] result = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);

            // Return as Base64 string (store in DB)
            return Convert.ToBase64String(result);
        }
        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] storedBytes = Convert.FromBase64String(storedHash);

            // Extract salt (first 16 bytes)
            byte[] salt = new byte[16];
            Buffer.BlockCopy(storedBytes, 0, salt, 0, 16);

            // Extract original hash (next 32 bytes)
            byte[] originalHash = new byte[32];
            Buffer.BlockCopy(storedBytes, 16, originalHash, 0, 32);

            // Derive key from entered password using same salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            byte[] newHash = pbkdf2.GetBytes(32);

            // Compare both hashes
            return CryptographicOperations.FixedTimeEquals(originalHash, newHash);
        }
    }
}
