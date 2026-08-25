using dotNetAssignment.Services.Interfaces;

namespace dotNetAssignment.Services.Implementations
{
    public class PasswordService : IPasswordService
    {
        /// <summary>
        /// Hashes the provided password using BCrypt algorithm.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>
        /// The hashed password
        /// </returns>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies if the provided password matches the hashed password using BCrypt algorithm.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="hashedPassword">The hashed password to compare against.</param>
        /// <returns>
        /// True if the password matches, otherwise false
        /// </returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
