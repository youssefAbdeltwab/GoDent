using GoDent.DAL.Entities;

namespace GoDent.BLL.Service.Abstraction
{
    public interface ITreatmentService
    {
        Task<IEnumerable<Treatment>> GetTreatmentsByVisitIdAsync(int visitId);
        Task<Treatment?> GetTreatmentByIdAsync(int id);
        Task CreateTreatmentAsync(Treatment treatment);
        Task UpdateTreatmentAsync(Treatment treatment);
        Task DeleteTreatmentAsync(int id);
    }
}
