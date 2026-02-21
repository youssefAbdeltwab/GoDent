using GoDent.BLL.DTOs;

namespace GoDent.BLL.Service.Abstraction
{
    public interface IFinanceDashboardService
    {
        Task<FinanceDashboardDto> GetDashboardAsync(DateTime from, DateTime to);
    }
}
