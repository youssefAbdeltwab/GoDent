using GoDent.DAL.Entities;

namespace GoDent.BLL.Service.Abstraction
{
    public interface IDoctorService
    {
        Task<IEnumerable<Doctor>> GetAllDoctorsAsync();
        Task<IEnumerable<Doctor>> GetActiveDoctorsAsync();
        Task<Doctor?> GetDoctorByIdAsync(int id);
        Task CreateDoctorAsync(Doctor doctor);
        Task UpdateDoctorAsync(Doctor doctor);
        Task<bool> DeleteDoctorAsync(int id);

        /// <summary>
        /// Returns doctor earnings summary: total cost, total lab cost, doctor share (40%), clinic share (60%)
        /// filtered by date range.
        /// </summary>
        Task<DoctorEarningsDto> GetDoctorEarningsAsync(int doctorId, DateTime from, DateTime to);
    }

    public class DoctorEarningsDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public decimal TotalLabCost { get; set; }
        public decimal NetCost { get; set; }
        public decimal DoctorShare { get; set; }
        public decimal ClinicShare { get; set; }
        public int TreatmentCount { get; set; }
    }
}
