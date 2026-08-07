using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotNetAssignment.Repositories.Jwt
{
    public interface IJwtRepository
    {
        void AddJwtId(Guid jwtId);

        Task<bool> JwtIdExistsAsync(Guid jwtId);

        Task RemoveJwtIdAsync(Guid jwtId);

        Task SaveChangesAsync();
    }
}