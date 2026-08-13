using dotNetAssignment.Models.DTO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using dotNetAssignment.Constants;

namespace dotNetAssignment.Handlers
{
    public class ModelStateHandler : ActionFilterAttribute
    {
        public override void OnActionExecuting(
            HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                var errors = actionContext.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key.Contains(".")
                            ? x.Key.Substring(x.Key.LastIndexOf('.') + 1)
                            : x.Key,
                        x => x.Value.Errors
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    );

                var response = new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ExceptionMessages.InvalidRequest,
                    Data = null,
                    Error = errors
                };

                actionContext.Response =
                    actionContext.Request.CreateResponse(
                        HttpStatusCode.BadRequest,
                        response);

                return;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
