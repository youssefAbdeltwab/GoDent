using GoDent.DAL.Entities;
using GoDent.DAL.Enums;

namespace GoDent.BLL.Service.Abstraction
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetAllExpensesAsync();
        Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(ExpenseCategory category);
        Task<Expense?> GetExpenseByIdAsync(int id);
        Task CreateExpenseAsync(Expense expense);
        Task UpdateExpenseAsync(Expense expense);
        Task DeleteExpenseAsync(int id);
        Task<decimal> GetTotalExpensesAsync(DateTime from, DateTime to);
    }
}
