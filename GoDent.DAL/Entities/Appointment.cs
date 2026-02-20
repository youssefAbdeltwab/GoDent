using System.ComponentModel.DataAnnotations;
using GoDent.DAL.Enums;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// Represents a single appointment slot.
    /// The combination of Date + StartTime is key for conflict detection.
    /// </summary>
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "يجب اختيار المريض")]
        [Display(Name = "المريض")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "تاريخ الحجز مطلوب")]
        [Display(Name = "تاريخ الحجز")]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "تاريخ الحجز يجب أن يكون في المستقبل")]
        public DateTime AppointmentDate { get; set; }


        //[Required(ErrorMessage = "توقيت الحجز مطلوب")]
        //[Display(Name = "توقيت الحجز")]
        //public AppointmentTime AppointmentTime { get; set; }


        [Required(ErrorMessage = "وقت البداية مطلوب")]
        [Display(Name = "وقت البداية")]
        [DataType(DataType.Time)]
        public TimeSpan? StartTime { get; set; }

       
        [Display(Name = "وقت النهاية")]
        [DataType(DataType.Time)]
        public TimeSpan? EndTime { get; set; }

        [Display(Name = "الحالة")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        [MaxLength(500)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Navigation ──
        public virtual Patient Patient { get; set; } = null!;
    }
}
