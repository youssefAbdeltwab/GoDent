using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoDent.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IPatientService _patientService;

        public PaymentService(AppDbContext context, IPatientService patientService)
        {
            _context = context;
            _patientService = patientService;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByPatientIdAsync(int patientId)
        {
            return await _context.Payments
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreatePaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Update patient debt after adding payment
            await _patientService.UpdatePatientDebtAsync(payment.PatientId);
        }

        public async Task DeletePaymentAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                var patientId = payment.PatientId;
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();

                // Update patient debt after deleting payment
                await _patientService.UpdatePatientDebtAsync(patientId);
            }
        }
    }
}
