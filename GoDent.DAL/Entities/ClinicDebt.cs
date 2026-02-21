using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GoDent.DAL.Enums;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// Tracks a debt the clinic owes (rent, lab bills, suppliers, etc.).
    /// Supports recurring debts that auto-generate monthly.
    /// </summary>
    public class ClinicDebt
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الدائن مطلوب")]
        [MaxLength(200, ErrorMessage = "الاسم لا يمكن أن يتجاوز 200 حرف")]
        [Display(Name = "الدائن/المورد")]
        public string CreditorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "الوصف مطلوب")]
        [MaxLength(300, ErrorMessage = "الوصف لا يمكن أن يتجاوز 300 حرف")]
        [Display(Name = "الوصف")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        [Display(Name = "المبلغ (ج.م)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "تاريخ الاستحقاق مطلوب")]
        [Display(Name = "تاريخ الاستحقاق")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "الحالة")]
        public DebtStatus Status { get; set; } = DebtStatus.Pending;

        [Display(Name = "التصنيف")]
        public ExpenseCategory Category { get; set; }

        [Display(Name = "دين متكرر؟")]
        public bool IsRecurring { get; set; } = false;

        [Display(Name = "يوم الاستحقاق الشهري")]
        [Range(1, 31, ErrorMessage = "يوم الاستحقاق يجب أن يكون بين 1 و 31")]
        public int? RecurrenceDay { get; set; }

        [MaxLength(500)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
