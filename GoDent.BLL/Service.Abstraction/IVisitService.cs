using GoDent.DAL.Entities;

namespace GoDent.BLL.Service.Abstraction
{
    public interface IVisitService
    {
        Task<IEnumerable<Visit>> GetAllVisitsByPatientIdAsync(int patientId);
        Task<Visit?> GetVisitByIdAsync(int id);
        Task CreateVisitAsync(Visit visit);
        Task UpdateVisitAsync(Visit visit);
        Task DeleteVisitAsync(int id);
        Task<bool> HasTreatmentsAsync(int visitId);
    }
}
