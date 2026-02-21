using GoDent.DAL.Entities;

namespace GoDent.BLL.Service.Abstraction
{
    public interface IClinicDebtService
    {
        Task<IEnumerable<ClinicDebt>> GetAllDebtsAsync();
        Task<IEnumerable<ClinicDebt>> GetPendingDebtsAsync();
        Task<IEnumerable<ClinicDebt>> GetOverdueDebtsAsync();
        Task<ClinicDebt?> GetDebtByIdAsync(int id);
        Task CreateDebtAsync(ClinicDebt debt);
        Task UpdateDebtAsync(ClinicDebt debt);
        Task MarkAsPaidAsync(int debtId);
        Task DeleteDebtAsync(int id);
        Task GenerateRecurringDebtsAsync();
        Task<decimal> GetTotalPendingDebtsAsync();
    }
}
