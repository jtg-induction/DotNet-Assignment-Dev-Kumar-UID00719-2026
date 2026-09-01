using dotNetAssignment.Models.Entities;
using dotNetAssignment.Models.Enums;
using System;
using System.Threading.Tasks;

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

        Task<UserRole?> GetUserRoleByIdAsync(Guid userId);

        Task<User> GetUserByIdAsync(Guid userId);

        Task<User> GetUserForUpdateAsync(Guid userId);

        Task<UserAddress> GetAddressByIdAsync(Guid UserId);

        Task SaveChangesAsync();
    }
}
