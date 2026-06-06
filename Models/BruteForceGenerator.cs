using System;
using System.Collections.Generic;

namespace password.Models
{
    /// <summary>
    /// Generates all possible character combinations for brute-force attacks.
    /// This is Stage 3 of the development - Brute force generation logic.
    /// 
    /// Key requirement: The generator does NOT know the password length in advance.
    /// It generates all combinations from length 1 to length 4.
    /// </summary>
    public class BruteForceGenerator
    {
        private const string Characters = "abcdefghijklmnopqrstuvwxyz";
        private const int MinimumLength = 1;
        private const int MaximumLength = 4;

        /// <summary>
        /// Generates ALL possible combinations of characters from length 1 to 4.
        /// This is lazy-evaluated using yield return for memory efficiency.
        /// </summary>
        /// <remarks>
        /// Enumeration order:
        /// Length 1: a, b, c, ..., z, 0, 
        /// Length 2: aa, ab, ac, ..., zz, 
        /// ... up to length 4
        /// 
        /// Total combinations: ~300,000 
        /// </remarks>
        public IEnumerable<string> GenerateAllCombinations()
        {
            for (int length = MinimumLength; length <= MaximumLength; length++)
            {
                // Generate combinations for this specific length
                foreach (var combination in GenerateByLength(length))
                {
                    yield return combination;
                }
            }
        }

        /// <summary>
        /// Generates all combinations of a specific length.
        /// This is a recursive generator that uses backtracking.
        /// </summary>
        /// <param name="length">The length of combinations to generate</param>
        /// <remarks>
        /// This method uses a char array and recursion for efficiency.
        /// It modifies the array in place and yields complete combinations.
        /// </remarks>
        private IEnumerable<string> GenerateByLength(int length)
        {
            char[] combination = new char[length];
            foreach (var result in GenerateByLengthRecursive(combination, 0))
            {
                yield return result;
            }
        }

        /// <summary>
        /// Recursive helper method to generate combinations.
        /// </summary>
        /// <param name="combination">The current combination being built</param>
        /// <param name="position">The current position in the combination</param>
        private IEnumerable<string> GenerateByLengthRecursive(char[] combination, int position)
        {
            if (position == combination.Length)
            {
                // Base case: we've filled all positions
                yield return new string(combination);
            }
            else
            {
                // Recursive case: try each character at this position
                for (int i = 0; i < Characters.Length; i++)
                {
                    combination[position] = Characters[i];
                    foreach (var result in GenerateByLengthRecursive(combination, position + 1))
                    {
                        yield return result;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the character set used for generation.
        /// </summary>
        public static string GetCharacterSet() => Characters;

        /// <summary>
        /// Gets the minimum length of combinations.
        /// </summary>
        public static int GetMinimumLength() => MinimumLength;

        /// <summary>
        /// Gets the maximum length of combinations.
        /// </summary>
        public static int GetMaximumLength() => MaximumLength;

        /// <summary>
        /// Calculates the total number of possible combinations.
        /// Formula: sum of (36^i) for i from 1 to 6
        /// </summary>
        public static long CalculateTotalCombinations()
        {
            long total = 0;
            for (int length = MinimumLength; length <= MaximumLength; length++)
            {
                total += (long)Math.Pow(Characters.Length, length);
            }
            return total;
        }
    }
}