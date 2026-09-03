using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Threading;
using System.Web.Http.Results;
using System.Net.Http;
using System.Net;

using log4net;

using dotNetAssignment.Models.DTO;

namespace dotNetAssignment.Handlers
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(GlobalExceptionHandler));

        /// <summary>
        /// Handles exceptions globally and returns a standardized error response.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>HTTP response with standardized error message</returns>
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            _logger.Error("Unhandled exception occurred", context.Exception);
            var response = new ApiResponseDto<object>
            {
                Success = false,
                Message = context.Exception.Message,
                Data = null
            };

            context.Result = new ResponseMessageResult(context.Request.CreateResponse(HttpStatusCode.InternalServerError, response));
            return Task.CompletedTask;
        }
    }
}
