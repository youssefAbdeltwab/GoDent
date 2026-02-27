using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoDent.BLL.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _context;

        public DoctorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors
                .OrderBy(d => d.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
        {
            return await _context.Doctors
                .Where(d => d.IsActive)
                .OrderBy(d => d.FullName)
                .ToListAsync();
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.Treatments)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Doctor?> GetDoctorWithDetailsAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.Treatments)
                    .ThenInclude(t => t.Patient)
                .Include(d => d.Treatments)
                    .ThenInclude(t => t.Visit)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task CreateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            var existingDoctor = await _context.Doctors.FindAsync(doctor.Id);
            if (existingDoctor == null) return;

            existingDoctor.FullName = doctor.FullName;
            existingDoctor.PhoneNumber = doctor.PhoneNumber;
            existingDoctor.Specialization = doctor.Specialization;
            existingDoctor.IsActive = doctor.IsActive;

            _context.Doctors.Update(existingDoctor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoctorAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ToggleActiveAsync(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                doctor.IsActive = !doctor.IsActive;
                await _context.SaveChangesAsync();
            }
        }
    }
}
