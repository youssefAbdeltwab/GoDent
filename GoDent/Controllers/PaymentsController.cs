using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Entities;
using GoDent.DAL.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GoDent.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IPatientService _patientService;

        public PaymentsController(IPaymentService paymentService, IPatientService patientService)
        {
            _paymentService = paymentService;
            _patientService = patientService;
        }

        // GET: Payments/Create?patientId=5
        public async Task<IActionResult> Create(int patientId)
        {
            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
                return NotFound();

            ViewBag.PatientId = patientId;
            ViewBag.PatientName = patient.FullName;
            ViewBag.CurrentDebt = patient.CurrentDebt;

            var payment = new Payment
            {
                PatientId = patientId,
                PaymentDate = DateTime.Today,
                PaymentMethod = PaymentMethod.Cash
            };

            return View(payment);
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            // Remove navigation property validation
            ModelState.Remove("Patient");

            // Ensure cash payment
            payment.PaymentMethod = PaymentMethod.Cash;

            if (ModelState.IsValid)
            {
                await _paymentService.CreatePaymentAsync(payment);
                TempData["Success"] = "تم تسجيل الدفعة بنجاح";
                return RedirectToAction("Details", "Patients", new { id = payment.PatientId });
            }

            var patient = await _patientService.GetPatientByIdAsync(payment.PatientId);
            ViewBag.PatientId = payment.PatientId;
            ViewBag.PatientName = patient?.FullName;
            ViewBag.CurrentDebt = patient?.CurrentDebt ?? 0;

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int patientId)
        {
            await _paymentService.DeletePaymentAsync(id);
            TempData["Success"] = "تم حذف الدفعة بنجاح";
            return RedirectToAction("Details", "Patients", new { id = patientId });
        }
    }
}
