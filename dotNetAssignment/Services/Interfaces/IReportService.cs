using dotNetAssignment.Models.DTO;
using dotNetAssignment.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotNetAssignment.Services.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GetTopTenItemsReport(TopTenMostOrderedRequestDto request);

        Task<ApiResponseDto<byte[]>> FrequentlyBoughtTogether(FrequentlyBoughtTogetherRequestDto request, Guid ownerId);
    }
}
