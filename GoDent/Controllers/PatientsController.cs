using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GoDent.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IVisitService _visitService;
        private readonly IPaymentService _paymentService;

        public PatientsController(IPatientService patientService, IVisitService visitService, IPaymentService paymentService)
        {
            _patientService = patientService;
            _visitService = visitService;
            _paymentService = paymentService;
        }


        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return View(patients);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound();

            var visits = await _visitService.GetAllVisitsByPatientIdAsync(id);
            var payments = await _paymentService.GetPaymentsByPatientIdAsync(id);
            
            ViewBag.Visits = visits;
            ViewBag.Payments = payments;
            ViewBag.TotalTreatmentCost = visits.SelectMany(v => v.Treatments).Sum(t => t.Cost);
            ViewBag.TotalPayments = payments.Sum(p => p.Amount);

            return View(patient);
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                await _patientService.CreatePatientAsync(patient);
                TempData["Success"] = "تم إضافة المريض بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return BadRequest();

            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Patient patient)
        {
            if (patient is null)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _patientService.UpdatePatientAsync(patient);
                TempData["Success"] = "تم تحديث بيانات المريض بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return BadRequest();

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _patientService.DeletePatientAsync(id);
            TempData["Success"] = "تم حذف المريض بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}