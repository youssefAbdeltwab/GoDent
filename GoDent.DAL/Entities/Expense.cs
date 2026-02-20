using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GoDent.DAL.Enums;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// Tracks clinic expenses (rent, materials, utilities, etc.).
    /// This is the "cost" side of the Profit & Loss dashboard:
    ///   Net Profit = Total Income (Payments) - Total Expenses
    /// 
    /// Note: Expenses are NOT linked to patients — they are clinic-level costs.
    /// </summary>
    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "وصف المصروف مطلوب")]
        [MaxLength(300, ErrorMessage = "الوصف لا يمكن أن يتجاوز 300 حرف")]
        [Display(Name = "الوصف")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        [Display(Name = "المبلغ (ج.م)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "تاريخ المصروف مطلوب")]
        [Display(Name = "التاريخ")]
        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; }

        [Display(Name = "التصنيف")]
        public ExpenseCategory Category { get; set; }

        [MaxLength(500)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
