using GoDent.DAL.Entities;

namespace GoDent.BLL.Service.Abstraction
{
    public interface IWhatsAppService
    {
        string GenerateShortageAlertUrl(Tool tool);
        string GeneratePostExtractionInstructionsUrl(Patient patient);
        string GenerateDebtReminderUrl(Patient patient, decimal debtAmount);
        string GenerateTodayAppointmentReminderUrl(Appointment appointment);
        string GenerateTomorrowAppointmentReminderUrl(Appointment appointment);
    }
}
