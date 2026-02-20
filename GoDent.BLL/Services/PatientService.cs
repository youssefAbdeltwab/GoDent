using GoDent.BLL.DTOs;
using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoDent.BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<Patient?> SearchPatientAsync(string term)
        {
            return await _context.Patients
                .Include(p => p.Appointments)
                .Include(p => p.Treatments)
                .FirstOrDefaultAsync(p => p.PhoneNumber.Contains(term) || p.FullName.Contains(term));
        }


        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _context.Patients
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.Appointments)
                .Include(p => p.Treatments)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Patient> CreatePatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient?> UpdatePatientAsync(Patient patient)
        {
            var existingPatient = await _context.Patients.FindAsync(patient.Id);
            if (existingPatient == null) return null;

            existingPatient.FullName = patient.FullName;
            existingPatient.PhoneNumber = patient.PhoneNumber;
            existingPatient.Gender = patient.Gender;
            existingPatient.DateOfBirth = patient.DateOfBirth;
            existingPatient.Address = patient.Address;
            existingPatient.MedicalHistory = patient.MedicalHistory;
            existingPatient.Notes = patient.Notes;

            _context.Patients.Update(existingPatient);
            await _context.SaveChangesAsync();
            return existingPatient;
        }

        public async Task<bool> DeletePatientAsync(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }

       
    }
}
