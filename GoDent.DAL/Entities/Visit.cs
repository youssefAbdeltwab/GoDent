using System.ComponentModel.DataAnnotations;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// Represents a patient visit to the clinic.
    /// Each visit can contain multiple treatments (procedures).
    /// </summary>
    public class Visit
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب اختيار المريض")]
        [Display(Name = "المريض")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "تاريخ الزيارة مطلوب")]
        [Display(Name = "تاريخ الزيارة")]
        [DataType(DataType.Date)]
        public DateTime VisitDate { get; set; }

        [MaxLength(500, ErrorMessage = "سبب الزيارة لا يمكن أن يتجاوز 500 حرف")]
        [Display(Name = "سبب الزيارة")]
        public string? ChiefComplaint { get; set; }

        [MaxLength(1000, ErrorMessage = "التشخيص لا يمكن أن يتجاوز 1000 حرف")]
        [Display(Name = "التشخيص")]
        public string? Diagnosis { get; set; }

        [MaxLength(500)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Navigation Properties ──
        public virtual Patient Patient { get; set; } = null!;
        public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
    }
}
