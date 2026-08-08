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

        public void AddJwtId(Guid jwtId)
        {
            _context.RefreshTokens.Add(new RefreshTokens
            {
                JwtId = jwtId,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<bool> JwtIdExistsAsync(Guid jwtId)
        {
            return await _context.RefreshTokens
                .AnyAsync(x => x.JwtId == jwtId);
        }

        public async Task RemoveJwtIdAsync(Guid jwtId)
        {
            var token = await _context.RefreshTokens.FindAsync(jwtId);

            if (token != null)
            {
                _context.RefreshTokens.Remove(token);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
