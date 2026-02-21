namespace GoDent.BLL.DTOs
{
    /// <summary>
    /// Aggregated financial data for the clinic dashboard.
    /// </summary>
    public class FinanceDashboardDto
    {
        // ── Period Totals ──
        /// <summary>Total payments received from patients in the period.</summary>
        public decimal TotalIncome { get; set; }

        /// <summary>Total clinic expenses in the period.</summary>
        public decimal TotalExpenses { get; set; }

        /// <summary>Income − Expenses (positive = profit, negative = loss).</summary>
        public decimal ProfitOrLoss => TotalIncome - TotalExpenses;

        // ── Debts (always current, not period-filtered) ──
        /// <summary>Sum of all patients' CurrentDebt (money patients owe the clinic).</summary>
        public decimal TotalPatientDebts { get; set; }

        /// <summary>Sum of pending + overdue clinic debts (money the clinic owes).</summary>
        public decimal TotalClinicDebts { get; set; }

        // ── Counts ──
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingClinicDebtsCount { get; set; }
        public int OverdueClinicDebtsCount { get; set; }

        // ── Monthly breakdown (last 6 months for chart) ──
        public List<MonthlyFinanceDto> MonthlyBreakdown { get; set; } = new();
    }

    public class MonthlyFinanceDto
    {
        public string Month { get; set; } = string.Empty; // e.g. "2026-01"
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
    }
}
