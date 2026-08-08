using System;
using System.Data.Entity;
using System.Threading.Tasks;

using dotNetAssignment.Data;
using dotNetAssignment.Models.Entities;

namespace dotNetAssignment.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly RestaurantDbContext _context;

        public UserRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public void AddUser(Users user)
        {
            _context.Users.Add(user);
        }

        public void AddAddress(UserAddresses address)
        {
            _context.UserAddresses.Add(address);
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Users> GetUserByIdAsync(Guid UserId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId);
        }

        public async Task<UserAddresses> GetAddressByIdAsync(Guid addressId)
        {
            return await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == addressId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
