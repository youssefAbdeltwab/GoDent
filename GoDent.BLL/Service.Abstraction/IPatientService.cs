using GoDent.BLL.DTOs;
using GoDent.DAL.Entities;

namespace GoDent.BLL.Service.Abstraction
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> SearchPatientAsync(string term);
        Task<Patient?> GetPatientByIdAsync(int id);

        Task<Patient> CreatePatientAsync(Patient patient);
        Task<Patient?> UpdatePatientAsync(Patient patient);
        Task<bool> DeletePatientAsync(int id);

    }
}
