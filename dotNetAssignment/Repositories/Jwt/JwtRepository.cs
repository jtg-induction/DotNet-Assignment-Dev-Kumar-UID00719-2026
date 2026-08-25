using System;
using System.Data.Entity;
using System.Threading.Tasks;

using dotNetAssignment.Data;
using dotNetAssignment.Models.Entities;

namespace dotNetAssignment.Repositories.Jwt
{
    public class JwtRepository : IJwtRepository
    {
        private readonly RestaurantDbContext _context;

        public JwtRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new JWT ID to the database with the current UTC timestamp.
        /// </summary>
        /// <param name="jwtId">The JWT ID to add.</param>
        public void AddJwtId(Guid jwtId)
        {
            _context.RefreshTokens.Add(new RefreshToken
            {
                JwtId = jwtId,
                CreatedAt = DateTime.UtcNow
            });
        }

        /// <summary>   
        /// Checks if a JWT ID exists in the database.
        /// </summary>
        /// <param name="jwtId">The JWT ID to check.</param>
        /// <returns>
        /// True if the JWT ID exists, otherwise false
        /// </returns>
        public async Task<bool> JwtIdExistsAsync(Guid jwtId)
        {
            return await _context.RefreshTokens
                .AnyAsync(x => x.JwtId == jwtId);
        }

        /// <summary>
        /// Removes a JWT ID from the database if it exists.
        /// </summary>
        /// <param name="jwtId">The JWT ID to remove.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public async Task RemoveJwtIdAsync(Guid jwtId)
        {
            var token = await _context.RefreshTokens.FindAsync(jwtId);

            if (token != null)
            {
                _context.RefreshTokens.Remove(token);
            }
        }

        /// <summary>
        /// Saves changes made in the context to the database asynchronously.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
