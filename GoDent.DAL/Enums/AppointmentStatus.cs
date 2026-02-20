namespace GoDent.DAL.Enums
{
    /// <summary>
    /// Tracks the lifecycle of an appointment.
    /// Why enum? — It ensures only valid statuses are stored in the DB,
    /// and EF Core stores them as integers (fast queries).
    /// </summary>
    public enum AppointmentStatus
    {
        Scheduled = 0,
        Completed = 1,
        Cancelled = 2,
        NoShow = 3
    }
}
