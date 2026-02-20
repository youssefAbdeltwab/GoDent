using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// Records a dental treatment/procedure for a patient.
    /// The Cost here feeds into the financial engine:
    ///   CurrentBalance = SUM(Treatment.Cost) - SUM(Payment.Amount)
    ///
    /// ToothNumber is nullable because some treatments (e.g., cleaning, X-ray)
    /// are not specific to one tooth.
    /// </summary>
    public class Treatment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب اختيار المريض")]
        [Display(Name = "المريض")]
        public int PatientId { get; set; }

        [Display(Name = "رقم السن")]
        [Range(1, 32, ErrorMessage = "رقم السن يجب أن يكون بين 1 و 32")]
        public int? ToothNumber { get; set; }

        [Required(ErrorMessage = "وصف العلاج مطلوب")]
        [MaxLength(500, ErrorMessage = "الوصف لا يمكن أن يتجاوز 500 حرف")]
        [Display(Name = "وصف العلاج")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "تكلفة العلاج مطلوبة")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "التكلفة يجب أن تكون قيمة موجبة")]
        [Display(Name = "التكلفة (ج.م)")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "تاريخ العلاج مطلوب")]
        [Display(Name = "تاريخ العلاج")]
        [DataType(DataType.Date)]
        public DateTime TreatmentDate { get; set; }

        [MaxLength(500)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Navigation ──
        public virtual Patient Patient { get; set; } = null!;
    }
}
