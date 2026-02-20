using GoDent.BLL.DTOs;
using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Entities;
using GoDent.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GoDent.BLL.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _context;

    public AppointmentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
    }

    //public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
    //{
    //    return await _context.Appointments
    //        .Include(a => a.Patient)
    //        .Where(a => a.AppointmentDate.Date == date.Date)
    //        .OrderBy(a => a.AppointmentDate)
    //        .ToListAsync();
    //}

    public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(string term)
    {
        return await _context.Appointments
            .Include(a => a.Patient)
            .Where(a => a.Patient.FullName.Contains(term) || a.Patient.PhoneNumber.Contains(term))
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<Appointment> GetAppointmentByIdAsync(int id)
    {
        
        return await _context.Appointments.FindAsync(id);
    }

    // CRITICAL: Conflict Checking Logic
    //public async Task<bool> HasConflictAsync(DateTime appointmentDate, int? excludeAppointmentId = null)
    //{
    //    // 30-minute window for conflict detection
    //    var startWindow = appointmentDate.AddMinutes(-30);
    //    var endWindow = appointmentDate.AddMinutes(30);

    //    if (excludeAppointmentId.HasValue)
    //    {
    //        return await _context.Appointments
    //            .AnyAsync(a =>
    //                a.Id != excludeAppointmentId.Value &&
    //                a.AppointmentDate >= startWindow &&
    //                a.AppointmentDate <= endWindow &&
    //                a.Status != "ملغي");
    //    }

    //    return await _context.Appointments
    //        .AnyAsync(a =>
    //            a.AppointmentDate >= startWindow &&
    //            a.AppointmentDate <= endWindow &&
    //            a.Status != "ملغي");
    //}

    public async Task AddAppointmentAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAppointmentAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AppointmentSlotDto>> GetDailySlotsAsync(DateTime date)
    {
        // Reset time component to ensure strict date match
        var targetDate = date.Date;

        // Fetch booked slots for this day
        var bookedTimes = await _context.Appointments
            .Where(a => a.AppointmentDate == targetDate && a.Status != AppointmentStatus.Cancelled)
            .Select(a => a.StartTime)
            .ToListAsync();

        var slots = new List<AppointmentSlotDto>();

        // Configuration: Work from 9:00 AM to 5:00 PM (17:00)
        var currentTime = new TimeSpan(9, 0, 0);
        var endTime = new TimeSpan(17, 0, 0);
        var slotDuration = TimeSpan.FromMinutes(30);

        while (currentTime < endTime)
        {
            bool isTaken = bookedTimes.Contains(currentTime);

            slots.Add(new AppointmentSlotDto
            {
                Time = currentTime,
                // Format explicitly using invariant culture or a specific format
                DisplayTime = DateTime.Today.Add(currentTime).ToString("hh:mm tt", CultureInfo.InvariantCulture),
                IsAvailable = !isTaken
            });

            currentTime = currentTime.Add(slotDuration);
        }

        return slots;
    }
}

 
