using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoDent.BLL.Services
{
    public class TreatmentService : ITreatmentService
    {
        private readonly AppDbContext _context;
        private readonly IPatientService _patientService;

        public TreatmentService(AppDbContext context, IPatientService patientService)
        {
            _context = context;
            _patientService = patientService;
        }

        public async Task<IEnumerable<Treatment>> GetTreatmentsByVisitIdAsync(int visitId)
        {
            return await _context.Treatments
                .Where(t => t.VisitId == visitId)
                .OrderByDescending(t => t.TreatmentDate)
                .ToListAsync();
        }

        public async Task<Treatment?> GetTreatmentByIdAsync(int id)
        {
            return await _context.Treatments
                .Include(t => t.Visit)
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CreateTreatmentAsync(Treatment treatment)
        {
            _context.Treatments.Add(treatment);
            await _context.SaveChangesAsync();

            // Update patient debt after adding treatment
            await _patientService.UpdatePatientDebtAsync(treatment.PatientId);
        }

        public async Task UpdateTreatmentAsync(Treatment treatment)
        {
            var existingTreatment = await _context.Treatments.FindAsync(treatment.Id);
            if (existingTreatment == null) return;

            existingTreatment.ToothNumber = treatment.ToothNumber;
            existingTreatment.Description = treatment.Description;
            existingTreatment.Cost = treatment.Cost;
            existingTreatment.TreatmentDate = treatment.TreatmentDate;
            existingTreatment.Notes = treatment.Notes;

            _context.Treatments.Update(existingTreatment);
            await _context.SaveChangesAsync();

            // Update patient debt after modifying treatment
            await _patientService.UpdatePatientDebtAsync(treatment.PatientId);
        }

        public async Task DeleteTreatmentAsync(int id)
        {
            var treatment = await _context.Treatments.FindAsync(id);
            if (treatment != null)
            {
                var patientId = treatment.PatientId;
                _context.Treatments.Remove(treatment);
                await _context.SaveChangesAsync();

                // Update patient debt after deleting treatment
                await _patientService.UpdatePatientDebtAsync(patientId);
            }
        }
    }
}
