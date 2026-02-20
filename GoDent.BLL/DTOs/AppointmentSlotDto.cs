namespace GoDent.BLL.DTOs
{
    public class AppointmentSlotDto
    {
        public TimeSpan Time { get; set; }
        public string DisplayTime { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }
}
