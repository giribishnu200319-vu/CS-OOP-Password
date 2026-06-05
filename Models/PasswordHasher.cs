using System;
using System.Security.Cryptography;
using System.Text;

namespace password.Models
{
    /// <summary>
    /// Handles password hashing using SHA256 with a constant static salt.
    /// This is Stage 2 of the development - Core hashing functionality.
    /// </summary>
    public class PasswordHasher
    {
        // Static salt - constant value defined in the application
        private static readonly string StaticSalt = "BruteForceApp2026";
        
        /// <summary>
        /// Generates a SHA256 hash of the given password with the static salt.
        /// </summary>
        /// <param name="password">The plain text password to hash</param>
        /// <returns>Hexadecimal string representation of the hash</returns>
        /// from Teachers
        public string GenerateHash(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            // Combine password with static salt
            string passwordWithSalt = password + StaticSalt;
            
            // Convert to bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordWithSalt);
            
            // Generate SHA256 hash
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                
                // Convert hash bytes to hexadecimal string
                StringBuilder hashBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashBuilder.Append(b.ToString("x2"));
                }
                
                return hashBuilder.ToString();
            }
        }

        /// <summary>
        /// Verifies if a given password matches the provided hash.
        /// </summary>
        /// <param name="password">The plain text password to verify</param>
        /// <param name="hash">The hash to compare against</param>
        /// <returns>True if password matches the hash, false otherwise</returns>
        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            // Generate hash of the provided password
            string generatedHash = GenerateHash(password);
            
            // Compare hashes using constant-time comparison to prevent timing attacks
            return ConstantTimeEquals(generatedHash, hash);
        }

        /// <summary>
        /// Performs constant-time string comparison to prevent timing attacks.
        /// </summary>
        private bool ConstantTimeEquals(string a, string b)
        {
            if (a == null || b == null)
                return a == b;

            if (a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }

            return diff == 0;
        }

        /// <summary>
        /// Gets the static salt used for hashing (for testing purposes).
        /// </summary>
        public static string GetStaticSalt() => StaticSalt;
    }
}