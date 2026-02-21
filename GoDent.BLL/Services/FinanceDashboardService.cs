using GoDent.BLL.DTOs;
using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoDent.BLL.Services
{
    public class FinanceDashboardService : IFinanceDashboardService
    {
        private readonly AppDbContext _context;

        public FinanceDashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FinanceDashboardDto> GetDashboardAsync(DateTime from, DateTime to)
        {
            var toEnd = to.Date.AddDays(1); // inclusive upper bound

            // ── Period-scoped totals (client-side evaluation for SQLite decimal compatibility) ──
            var payments = await _context.Payments
                .Where(p => p.PaymentDate >= from.Date && p.PaymentDate < toEnd)
                .Select(p => p.Amount)
                .ToListAsync();
            var totalIncome = payments.Sum();

            var expenses = await _context.Expenses
                .Where(e => e.ExpenseDate >= from.Date && e.ExpenseDate < toEnd)
                .Select(e => e.Amount)
                .ToListAsync();
            var totalExpenses = expenses.Sum();

            // ── Current debts (not period-filtered) ──
            var patientDebts = await _context.Patients
                .Select(p => p.CurrentDebt)
                .ToListAsync();
            var totalPatientDebts = patientDebts.Sum();

            var pendingDebts = await _context.ClinicDebts
                .Where(d => d.Status == DebtStatus.Pending)
                .CountAsync();

            var overdueDebts = await _context.ClinicDebts
                .Where(d => d.Status == DebtStatus.Overdue)
                .CountAsync();

            var clinicDebts = await _context.ClinicDebts
                .Where(d => d.Status == DebtStatus.Pending || d.Status == DebtStatus.Overdue)
                .Select(d => d.Amount)
                .ToListAsync();
            var totalClinicDebts = clinicDebts.Sum();

            // ── Counts ──
            var totalPatients = await _context.Patients.CountAsync();
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var todayAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow)
                .CountAsync();

            // ── Last 6 months breakdown ──
            var sixMonthsAgo = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5);

            // Client-side grouping for SQLite decimal compatibility
            var allPayments = await _context.Payments
                .Where(p => p.PaymentDate >= sixMonthsAgo)
                .Select(p => new { p.PaymentDate.Year, p.PaymentDate.Month, p.Amount })
                .ToListAsync();

            var monthlyIncome = allPayments
                .GroupBy(p => new { p.Year, p.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(x => x.Amount)
                })
                .ToList();

            var allExpenses = await _context.Expenses
                .Where(e => e.ExpenseDate >= sixMonthsAgo)
                .Select(e => new { e.ExpenseDate.Year, e.ExpenseDate.Month, e.Amount })
                .ToListAsync();

            var monthlyExpenses = allExpenses
                .GroupBy(e => new { e.Year, e.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(x => x.Amount)
                })
                .ToList();

            var breakdown = new List<MonthlyFinanceDto>();
            for (int i = 0; i < 6; i++)
            {
                var date = sixMonthsAgo.AddMonths(i);
                var key = $"{date.Year}-{date.Month:D2}";
                breakdown.Add(new MonthlyFinanceDto
                {
                    Month = key,
                    Income = monthlyIncome.FirstOrDefault(m => m.Year == date.Year && m.Month == date.Month)?.Total ?? 0,
                    Expenses = monthlyExpenses.FirstOrDefault(m => m.Year == date.Year && m.Month == date.Month)?.Total ?? 0
                });
            }

            return new FinanceDashboardDto
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                TotalPatientDebts = totalPatientDebts,
                TotalClinicDebts = totalClinicDebts,
                TotalPatients = totalPatients,
                TodayAppointments = todayAppointments,
                PendingClinicDebtsCount = pendingDebts,
                OverdueClinicDebtsCount = overdueDebts,
                MonthlyBreakdown = breakdown
            };
        }
    }
}
