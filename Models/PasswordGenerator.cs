using System;
using System.Text;

namespace password.Models
{
    /// <summary>
    /// Generates random passwords for testing the brute-force algorithm.
    /// This is Stage 2 of the development - Core generation functionality.
    /// Passwords are 4-6 characters long as required.
    /// </summary>
    public class PasswordGenerator
    {
        // Character set for password generation (lowercase and digits for simplicity)
        private const string CharacterSet = "abcdefghijklmnopqrstuvwxyz0123456789";
        
        // Minimum password length (inclusive)
        private const int MinimumLength = 1;
        
        // Maximum password length (exclusive) - so actual max is 4
        private const int MaximumLength = 4;
        
        private readonly Random _random = new Random();
        
        /// <summary>
        /// Generates a random password with length between 1 and 3 (inclusive).
        /// </summary>
        /// <returns>A randomly generated password</returns>
        public string GeneratePassword()
        {
            int length = GetRandomLength();
            StringBuilder password = new StringBuilder();
            
            for (int i = 0; i < length; i++)
            {
                int randomIndex = _random.Next(CharacterSet.Length);
                password.Append(CharacterSet[randomIndex]);
            }
            
            return password.ToString();
        }

        /// <summary>
        /// Gets a random password length between 1 and 3 (inclusive).
        /// Range is [1, 4) as specified in requirements.
        /// </summary>
        /// <returns>A random integer between 1 and 3</returns>
        public int GetRandomLength()
        {
            return _random.Next(MinimumLength, MaximumLength);
        }

        /// <summary>
        /// Gets the character set used for password generation.
        /// </summary>
        public static string GetCharacterSet() => CharacterSet;

        /// <summary>
        /// Gets the minimum password length.
        /// </summary>
        public static int GetMinimumLength() => MinimumLength;

        /// <summary>
        /// Gets the maximum password length (exclusive).
        /// </summary>
        public static int GetMaximumLength() => MaximumLength;
    }
}