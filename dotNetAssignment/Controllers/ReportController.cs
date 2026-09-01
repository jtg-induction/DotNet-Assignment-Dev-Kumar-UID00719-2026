using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Services.Implementations;
using dotNetAssignment.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace dotNetAssignment.Controllers
{
    [RoutePrefix("api/report")]
    public class ReportController : ApiController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("top-items")]
        public async Task<IHttpActionResult> GetTopTenMostOrderedItems([FromUri] TopTenMostOrderedRequestDto request = null)
        {
            var response = await _reportService.GetTopTenItemsReport(request);

            var httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            httpResponse.Content = new ByteArrayContent(response);
            httpResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            httpResponse.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "TopTenMostOrderedItems.pdf"
            };

            return ResponseMessage(httpResponse);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpGet]
        [Route("frequently-bought")]
        public async Task<IHttpActionResult> FrequentlyBoughtTogether([FromUri] FrequentlyBoughtTogetherRequestDto request)
        {
            var ownerId = Guid.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value);
            var response = await _reportService.FrequentlyBoughtTogether(request, ownerId);
            if (!response.Success)
            {
                if(response.Message == ExceptionMessages.YouCantPerformThisAction)
                {
                    return Content(HttpStatusCode.Unauthorized, response);
                }
            }

            var httpResponse = Request.CreateResponse(System.Net.HttpStatusCode.OK);
            httpResponse.Content = new ByteArrayContent(response.Data);
            httpResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            httpResponse.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "FrequentlyBoughtTogether.pdf"
            };

            return ResponseMessage(httpResponse);
        }
    }
}
