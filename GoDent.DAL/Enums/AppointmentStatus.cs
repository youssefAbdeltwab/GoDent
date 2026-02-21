using System.ComponentModel.DataAnnotations;

namespace GoDent.DAL.Enums
{
    /// <summary>
    /// Tracks the lifecycle of an appointment.
    /// Why enum? — It ensures only valid statuses are stored in the DB,
    /// and EF Core stores them as integers (fast queries).
    /// </summary>
    public enum AppointmentStatus
    {
        [Display(Name = "مجدول")]
        Scheduled = 0,
        
        [Display(Name = "مكتمل")]
        Completed = 1,
        
        [Display(Name = "ملغي")]
        Cancelled = 2,
        
        [Display(Name = "لم يحضر")]
        NoShow = 3
    }
}
