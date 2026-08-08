using System;
using System.Threading.Tasks;

using dotNetAssignment.Models.Entities;

namespace dotNetAssignment.Repositories.User
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);

        void AddUser(Users user);

        void AddAddress(UserAddresses address);

        Task<Users> GetUserByEmailAsync(String email);

        Task<Users> GetUserByIdAsync(Guid UserId);

        Task<UserAddresses> GetAddressByIdAsync(Guid UserId);

        Task SaveChangesAsync();
    }
}
