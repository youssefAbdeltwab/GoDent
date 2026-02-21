using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoDent.BLL.Services
{
    public class VisitService : IVisitService
    {
        private readonly AppDbContext _context;

        public VisitService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Visit>> GetAllVisitsByPatientIdAsync(int patientId)
        {
            return await _context.Visits
                .Include(v => v.Treatments)
                .Where(v => v.PatientId == patientId)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync();
        }

        public async Task<Visit?> GetVisitByIdAsync(int id)
        {
            return await _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Treatments)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task CreateVisitAsync(Visit visit)
        {
            _context.Visits.Add(visit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVisitAsync(Visit visit)
        {
            var existingVisit = await _context.Visits.FindAsync(visit.Id);
            if (existingVisit == null) return;

            existingVisit.VisitDate = visit.VisitDate;
            existingVisit.ChiefComplaint = visit.ChiefComplaint;
            existingVisit.Diagnosis = visit.Diagnosis;
            existingVisit.Notes = visit.Notes;

            _context.Visits.Update(existingVisit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVisitAsync(int id)
        {
            var visit = await _context.Visits.FindAsync(id);
            if (visit != null)
            {
                _context.Visits.Remove(visit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasTreatmentsAsync(int visitId)
        {
            return await _context.Treatments.AnyAsync(t => t.VisitId == visitId);
        }
    }
}
