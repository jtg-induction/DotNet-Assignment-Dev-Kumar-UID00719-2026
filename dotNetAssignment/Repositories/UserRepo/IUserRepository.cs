using System;
using System.Threading.Tasks;

using dotNetAssignment.Models.Entities;

namespace dotNetAssignment.Repositories.UserRepo
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);

        void AddUser(User user);

        void AddAddress(UserAddress address);

        Task<User> GetUserByEmailAsync(String email);

        Task<User> GetUserByIdAsync(Guid UserId);

        Task SaveChangesAsync();
    }
}
