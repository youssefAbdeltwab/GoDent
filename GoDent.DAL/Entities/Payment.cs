using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GoDent.DAL.Enums;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// Records a payment (full or partial / installment) from a patient.
    /// This is the other side of the financial equation:
    ///   CurrentBalance = SUM(Treatment.Cost) - SUM(Payment.Amount)
    /// 
    /// Why separate from Treatment?
    /// — A patient might pay for multiple treatments at once,
    ///   or pay in installments over time. Keeping payments separate
    ///   gives us maximum flexibility for the "Installment Logs" feature.
    /// </summary>
    public class Payment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب اختيار المريض")]
        [Display(Name = "المريض")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        [Display(Name = "المبلغ (ج.م)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "تاريخ الدفع مطلوب")]
        [Display(Name = "تاريخ الدفع")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }
        
        [Display(Name = "طريقة الدفع")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        [MaxLength(500)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Navigation ──
        public virtual Patient Patient { get; set; } = null!;
    }
}
