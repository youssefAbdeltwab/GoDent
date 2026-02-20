using GoDent.BLL.DTOs;
using GoDent.DAL.Entities;

namespace GoDent.BLL.Service.Abstraction;

public interface IAppointmentService
{
    Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
    //Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
    Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(string term);
    //Task<bool> HasConflictAsync(DateTime appointmentDate, int? excludeAppointmentId = null);
    Task AddAppointmentAsync(Appointment appointment);
    Task UpdateAppointmentAsync(Appointment appointment);
    Task<Appointment> GetAppointmentByIdAsync(int id);

    /// <summary>
    /// Generates a list of time slots for a specific date, marking them as Available or Booked.
    /// </summary>
    Task<List<AppointmentSlotDto>> GetDailySlotsAsync(DateTime date);
}