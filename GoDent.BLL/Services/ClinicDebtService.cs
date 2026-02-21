using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Entities;
using GoDent.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoDent.BLL.Services
{
    public class ClinicDebtService : IClinicDebtService
    {
        private readonly AppDbContext _context;
        private readonly IExpenseService _expenseService;

        public ClinicDebtService(AppDbContext context, IExpenseService expenseService)
        {
            _context = context;
            _expenseService = expenseService;
        }

        public async Task<IEnumerable<ClinicDebt>> GetAllDebtsAsync()
        {
            return await _context.ClinicDebts
                .OrderByDescending(d => d.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicDebt>> GetPendingDebtsAsync()
        {
            return await _context.ClinicDebts
                .Where(d => d.Status == DebtStatus.Pending || d.Status == DebtStatus.Overdue)
                .OrderBy(d => d.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicDebt>> GetOverdueDebtsAsync()
        {
            // Auto-detect overdue: pending + past due date
            var overdueDebts = await _context.ClinicDebts
                .Where(d => d.Status == DebtStatus.Pending && d.DueDate < DateTime.Today)
                .ToListAsync();

            // Update status to Overdue
            foreach (var debt in overdueDebts)
            {
                debt.Status = DebtStatus.Overdue;
            }

            if (overdueDebts.Any())
            {
                await _context.SaveChangesAsync();
            }

            return await _context.ClinicDebts
                .Where(d => d.Status == DebtStatus.Overdue)
                .OrderBy(d => d.DueDate)
                .ToListAsync();
        }

        public async Task<ClinicDebt?> GetDebtByIdAsync(int id)
        {
            return await _context.ClinicDebts.FindAsync(id);
        }

        public async Task CreateDebtAsync(ClinicDebt debt)
        {
            _context.ClinicDebts.Add(debt);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDebtAsync(ClinicDebt debt)
        {
            var existing = await _context.ClinicDebts.FindAsync(debt.Id);
            if (existing == null) return;

            existing.CreditorName = debt.CreditorName;
            existing.Description = debt.Description;
            existing.Amount = debt.Amount;
            existing.DueDate = debt.DueDate;
            existing.Category = debt.Category;
            existing.IsRecurring = debt.IsRecurring;
            existing.RecurrenceDay = debt.RecurrenceDay;
            existing.Notes = debt.Notes;

            _context.ClinicDebts.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsPaidAsync(int debtId)
        {
            var debt = await _context.ClinicDebts.FindAsync(debtId);
            if (debt == null) return;

            // Create corresponding expense entry
            var expense = new Expense
            {
                Description = $"تسديد دين: {debt.CreditorName} - {debt.Description}",
                Amount = debt.Amount,
                ExpenseDate = DateTime.Today,
                Category = debt.Category,
                Notes = $"تم التسديد تلقائياً من ديون العيادة (معرف الدين: {debt.Id})"
            };
            await _expenseService.CreateExpenseAsync(expense);

            debt.Status = DebtStatus.Paid;
            await _context.SaveChangesAsync();

            // If recurring, generate next month's debt
            if (debt.IsRecurring && debt.RecurrenceDay.HasValue)
            {
                var nextMonth = debt.DueDate.AddMonths(1);
                var day = Math.Min(debt.RecurrenceDay.Value, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month));
                var nextDueDate = new DateTime(nextMonth.Year, nextMonth.Month, day);

                // Check if next month's debt already exists
                var exists = await _context.ClinicDebts.AnyAsync(d =>
                    d.CreditorName == debt.CreditorName &&
                    d.Description == debt.Description &&
                    d.DueDate.Month == nextDueDate.Month &&
                    d.DueDate.Year == nextDueDate.Year &&
                    d.Status != DebtStatus.Paid);

                if (!exists)
                {
                    var newDebt = new ClinicDebt
                    {
                        CreditorName = debt.CreditorName,
                        Description = debt.Description,
                        Amount = debt.Amount,
                        DueDate = nextDueDate,
                        Status = DebtStatus.Pending,
                        Category = debt.Category,
                        IsRecurring = true,
                        RecurrenceDay = debt.RecurrenceDay,
                        Notes = debt.Notes
                    };

                    _context.ClinicDebts.Add(newDebt);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteDebtAsync(int id)
        {
            var debt = await _context.ClinicDebts.FindAsync(id);
            if (debt != null)
            {
                _context.ClinicDebts.Remove(debt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task GenerateRecurringDebtsAsync()
        {
            var currentMonth = DateTime.Today;
            var recurringDebts = await _context.ClinicDebts
                .Where(d => d.IsRecurring && d.Status == DebtStatus.Paid)
                .ToListAsync();

            foreach (var debt in recurringDebts)
            {
                if (!debt.RecurrenceDay.HasValue) continue;

                var day = Math.Min(debt.RecurrenceDay.Value, DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month));
                var dueDate = new DateTime(currentMonth.Year, currentMonth.Month, day);

                // Check if this month's debt already exists
                var exists = await _context.ClinicDebts.AnyAsync(d =>
                    d.CreditorName == debt.CreditorName &&
                    d.Description == debt.Description &&
                    d.DueDate.Month == dueDate.Month &&
                    d.DueDate.Year == dueDate.Year &&
                    d.Status != DebtStatus.Paid);

                if (!exists)
                {
                    var newDebt = new ClinicDebt
                    {
                        CreditorName = debt.CreditorName,
                        Description = debt.Description,
                        Amount = debt.Amount,
                        DueDate = dueDate,
                        Status = dueDate < DateTime.Today ? DebtStatus.Overdue : DebtStatus.Pending,
                        Category = debt.Category,
                        IsRecurring = true,
                        RecurrenceDay = debt.RecurrenceDay,
                        Notes = debt.Notes
                    };

                    _context.ClinicDebts.Add(newDebt);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalPendingDebtsAsync()
        {
            var amounts = await _context.ClinicDebts
                .Where(d => d.Status == DebtStatus.Pending || d.Status == DebtStatus.Overdue)
                .Select(d => d.Amount)
                .ToListAsync();
            return amounts.Sum();
        }
    }
}
