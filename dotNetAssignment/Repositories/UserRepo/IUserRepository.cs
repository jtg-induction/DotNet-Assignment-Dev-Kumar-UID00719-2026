using System;
using System.Threading.Tasks;

using dotNetAssignment.Models.Entities;

namespace dotNetAssignment.Repositories.UserRepo
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);

        Task<bool> PhoneNumberExistsAsync(string phoneNumber);

        Task<bool> UserExistsAsync(Guid userId);

        void AddUser(User user);

        void AddAddress(UserAddress address);

        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByIdAsync(Guid userId);

        Task<UserAddress> GetAddressByIdAsync(Guid UserId);

        Task SaveChangesAsync();
    }
}
