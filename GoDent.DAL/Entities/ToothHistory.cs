using System.ComponentModel.DataAnnotations;
using GoDent.DAL.Enums;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// Records the history/status of a specific tooth for a patient.
    /// This powers the Dental Chart (Teeth Map) in Step 4.
    /// Each row = one event in a tooth's life.
    /// ToothNumber uses the Universal Numbering System (1-32 for adults).
    /// </summary>
    public class ToothHistory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب اختيار المريض")]
        [Display(Name = "المريض")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "رقم السن مطلوب")]
        [Range(1, 32, ErrorMessage = "رقم السن يجب أن يكون بين 1 و 32")]
        [Display(Name = "رقم السن")]
        public int ToothNumber { get; set; }

        [Display(Name = "حالة السن")]
        public ToothStatus Status { get; set; }

        [MaxLength(500)]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "تاريخ العلاج مطلوب")]
        [Display(Name = "تاريخ العلاج")]
        [DataType(DataType.Date)]
        public DateTime TreatmentDate { get; set; }

        [MaxLength(500)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual Patient Patient { get; set; } = null!;
    }
}
