using GoDent.DAL.Entities;

namespace GoDent.BLL.Service.Abstraction
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetPaymentsByPatientIdAsync(int patientId);
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task CreatePaymentAsync(Payment payment);
        Task DeletePaymentAsync(int id);
    }
}
