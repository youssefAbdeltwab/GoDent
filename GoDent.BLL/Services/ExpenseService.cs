using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Entities;
using GoDent.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoDent.BLL.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
        {
            return await _context.Expenses
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Expenses
                .Where(e => e.ExpenseDate >= from && e.ExpenseDate <= to)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(ExpenseCategory category)
        {
            return await _context.Expenses
                .Where(e => e.Category == category)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<Expense?> GetExpenseByIdAsync(int id)
        {
            return await _context.Expenses.FindAsync(id);
        }

        public async Task CreateExpenseAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExpenseAsync(Expense expense)
        {
            var existing = await _context.Expenses.FindAsync(expense.Id);
            if (existing == null) return;

            existing.Description = expense.Description;
            existing.Amount = expense.Amount;
            existing.ExpenseDate = expense.ExpenseDate;
            existing.Category = expense.Category;
            existing.Notes = expense.Notes;

            _context.Expenses.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalExpensesAsync(DateTime from, DateTime to)
        {
            var amounts = await _context.Expenses
                .Where(e => e.ExpenseDate >= from && e.ExpenseDate <= to)
                .Select(e => e.Amount)
                .ToListAsync();
            return amounts.Sum();
        }
    }
}
