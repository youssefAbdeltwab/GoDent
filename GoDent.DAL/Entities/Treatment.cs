using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// Records a dental treatment/procedure for a patient.
    /// The Cost here feeds into the financial engine:
    ///   CurrentBalance = SUM(Treatment.Cost) - SUM(Payment.Amount)
    ///
    /// Revenue split (after lab costs):
    ///   NetCost = Cost - LabCost
    ///   DoctorShare = NetCost * 40%
    ///   ClinicShare = NetCost * 60%
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

        [Required(ErrorMessage = "يجب اختيار الطبيب")]
        [Display(Name = "الطبيب")]
        public int DoctorId { get; set; }

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

        [Display(Name = "يوجد تكلفة معمل؟")]
        public bool HasLabCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "تكلفة المعمل يجب أن تكون قيمة موجبة")]
        [Display(Name = "تكلفة المعمل (ج.م)")]
        public decimal? LabCost { get; set; }

        [Required(ErrorMessage = "تاريخ العلاج مطلوب")]
        [Display(Name = "تاريخ العلاج")]
        [DataType(DataType.Date)]
        public DateTime TreatmentDate { get; set; }

        [MaxLength(500)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "يجب اختيار الزيارة")]
        [Display(Name = "الزيارة")]
        public int VisitId { get; set; }

        // ── Computed Properties (not stored in DB) ──

        /// <summary>Net cost after subtracting lab costs</summary>
        [NotMapped]
        [Display(Name = "الصافي")]
        public decimal NetCost => Cost - (HasLabCost ? (LabCost ?? 0) : 0);

        /// <summary>Doctor's 40% share of net cost</summary>
        [NotMapped]
        [Display(Name = "نصيب الطبيب (40%)")]
        public decimal DoctorShare => NetCost * 0.40m;

        /// <summary>Clinic's 60% share of net cost</summary>
        [NotMapped]
        [Display(Name = "نصيب العيادة (60%)")]
        public decimal ClinicShare => NetCost * 0.60m;

        // ── Navigation ──
        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;
        public virtual Visit Visit { get; set; } = null!;
    }
}
