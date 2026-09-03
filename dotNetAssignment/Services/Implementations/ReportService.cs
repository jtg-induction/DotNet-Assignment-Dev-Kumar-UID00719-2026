using dotNetAssignment.Constants;
using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Enums;
using dotNetAssignment.Reports;
using dotNetAssignment.Repositories.OrderRepository;
using dotNetAssignment.Repositories.RestaurantRepo;
using dotNetAssignment.Repositories.UserRepo;
using dotNetAssignment.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Telerik.Reporting;
using Telerik.Reporting.Processing;

namespace dotNetAssignment.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IUserRepository _userRepository;
        public ReportService(
            IRestaurantRepository restaurantRepository,
            IUserRepository userRepository)
        {
            _restaurantRepository = restaurantRepository;
            _userRepository = userRepository;
        }

        public async Task<byte[]> GetTopTenItemsReport(TopTenMostOrderedRequestDto request)
        {
            var reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "Top10MostOrderedItems.trdp");
            var reportPackager = new ReportPackager();

            Telerik.Reporting.Report report;

            using (var stream = File.OpenRead(reportPath))
            {
                report = (Telerik.Reporting.Report) reportPackager.UnpackageDocument(stream);
            }

            report.ReportParameters["ExcludeItemIds"].Value = request?.ExcludeItemIds ?? "";
            var reportProcessor = new ReportProcessor();
            var reportSource = new InstanceReportSource();
            reportSource.ReportDocument = report;

            var result = reportProcessor.RenderReport("PDF", reportSource, new System.Collections.Hashtable());
            return result.DocumentBytes;
        }


        public async Task<ApiResponseDto<byte[]>> FrequentlyBoughtTogether(FrequentlyBoughtTogetherRequestDto request, Guid ownerId) 
        {
            var role = await _userRepository.GetUserRoleByIdAsync(ownerId);
            if(role == UserRole.Owner)
            {
                var isRestaurantOwner = await _restaurantRepository.IsRestaurantOwnerAsync(Guid.Parse(request.RestaurantId), ownerId);
            
                if (!isRestaurantOwner)
                {
                    return new ApiResponseDto<byte[]>
                    {
                        Success = false,
                        Message = ExceptionMessages.YouCantPerformThisAction
                    };
                }
            }

            var report = new FrequentlyBoughtTogetherReport();

            report.ReportParameters["RestaurantId"].Value = request?.RestaurantId ?? "";
            var reportProcessor = new ReportProcessor();
            var reportSource = new InstanceReportSource();
            reportSource.ReportDocument = report;

            var result = reportProcessor.RenderReport("PDF", reportSource, new System.Collections.Hashtable());
            return new ApiResponseDto<byte[]>
            {
                Success = true,
                Message = SuccessMessages.ReportGenerated,
                Data = result.DocumentBytes
            };
        }

    }
}
