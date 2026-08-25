using System;
using System.Data.Entity;
using System.Threading.Tasks;

using dotNetAssignment.Data;
using dotNetAssignment.Models.Entities;

namespace dotNetAssignment.Repositories.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly RestaurantDbContext _context;

        public UserRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Checks if a user with the specified email exists in the database.
        /// </summary>
        /// <param name="email">The email to check.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        /// <summary>
        /// Checks if a user with the specified phone number exists in the database.
        /// </summary>
        /// <param name="phoneNumber">The phone number to check.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return false;
            }
            return await _context.Users.AnyAsync(x => x.PhoneNumber == phoneNumber);
        }

        /// <summary>
        /// Adds a new user to the database context.
        /// </summary>
        /// <param name="user">The user to add.</param>
        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        /// <summary>
        /// Adds a new user address to the database context.
        /// </summary>
        /// <param name="address">The user address to add.</param>
        public void AddAddress(UserAddress address)
        {
            _context.UserAddresses.Add(address);
        }

        /// <summary>
        /// Retrieves a user from the database by their email address.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email.ToLower());
        }

        /// <summary>
        /// Retrieves a user from the database by their unique identifier (UserId).
        /// </summary>
        /// <param name="userId">The unique id of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        /// <summary>
        /// Retrieves a user's Address from the database by addressId.
        /// </summary>
        /// <param name="addressId">The unique id of the address to retrieve.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<UserAddress> GetAddressByIdAsync(Guid addressId)
        {
            return await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == addressId);
        }

        /// <summary>
        /// Saves all changes made in the context to the database asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
