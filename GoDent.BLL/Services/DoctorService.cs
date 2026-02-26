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
                .OrderByDescending(d => d.CreatedAt)
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

        public async Task CreateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            var existing = await _context.Doctors.FindAsync(doctor.Id);
            if (existing == null) return;

            existing.FullName = doctor.FullName;
            existing.PhoneNumber = doctor.PhoneNumber;
            existing.Specialty = doctor.Specialty;
            existing.Notes = doctor.Notes;
            existing.IsActive = doctor.IsActive;

            _context.Doctors.Update(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Treatments)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null) return false;

            // Don't delete if doctor has treatments
            if (doctor.Treatments.Any())
                return false;

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DoctorEarningsDto> GetDoctorEarningsAsync(int doctorId, DateTime from, DateTime to)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
                return new DoctorEarningsDto();

            // Client-side evaluation for SQLite decimal Sum compatibility
            var treatments = await _context.Treatments
                .Where(t => t.DoctorId == doctorId
                         && t.TreatmentDate >= from
                         && t.TreatmentDate <= to)
                .ToListAsync();

            var totalCost = treatments.Sum(t => t.Cost);
            var totalLabCost = treatments.Where(t => t.HasLabCost).Sum(t => t.LabCost);
            var netCost = totalCost - totalLabCost;

            return new DoctorEarningsDto
            {
                DoctorId = doctorId,
                DoctorName = doctor.FullName,
                TotalCost = totalCost,
                TotalLabCost = totalLabCost,
                NetCost = netCost,
                DoctorShare = netCost * 0.40m,
                ClinicShare = netCost * 0.60m,
                TreatmentCount = treatments.Count
            };
        }
    }
}
