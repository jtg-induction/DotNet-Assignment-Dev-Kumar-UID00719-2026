using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Repositories.UserRepo;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace dotNetAssignment.Filters
{
    public class ActiveUserFilter : AuthorizationFilterAttribute
    {
        public override async Task OnAuthorizationAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken)
        {
            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            var userId = Guid.Parse(principal?.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userRepository = (IUserRepository)actionContext.Request.GetDependencyScope().GetService(typeof(IUserRepository));

            var user = await userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                var response = new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ExceptionMessages.UserNotFound,
                    Data = null
                };

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.NotFound, response);
                return;
            }
            if (!user.IsActive)
            {
                var response = new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ExceptionMessages.UserInactive,
                    Data = null
                };

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, response);
                return;
            }
        }
    }
}
