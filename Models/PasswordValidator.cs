using System;

namespace password.Models
{
    /// <summary>
    /// Validates passwords against a target hash.
    /// This is Stage 2 of the development - Core validation functionality.
    /// </summary>
    public class PasswordValidator
    {
        private readonly PasswordHasher _hasher;
        private string? _targetHash;

        public PasswordValidator()
        {
            _hasher = new PasswordHasher();
        }

        /// <summary>
        /// Sets the target hash that passwords will be validated against.
        /// </summary>
        /// <param name="hash">The target hash to match</param>
        public void SetTargetHash(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentException("Hash cannot be null or empty", nameof(hash));

            _targetHash = hash;
        }

        /// <summary>
        /// Checks if the given password is correct by comparing its hash with the target hash.
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <returns>True if the password matches the target hash, false otherwise</returns>
        /// <remarks>
        /// This method is thread-safe as it only reads from _targetHash and uses 
        /// the stateless PasswordHasher methods.
        /// </remarks>
        public bool IsPasswordCorrect(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (_targetHash == null)
                throw new InvalidOperationException("Target hash not set. Call SetTargetHash first.");

            // Verify password against the target hash
            return _hasher.VerifyPassword(password, _targetHash);
        }

        /// <summary>
        /// Gets the current target hash.
        /// </summary>
        public string? GetTargetHash() => _targetHash;

        /// <summary>
        /// Resets the validator state.
        /// </summary>
        public void Reset()
        {
            _targetHash = null;
        }
    }
}