using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GoDent.Controllers
{
    public class TreatmentsController : Controller
    {
        private readonly ITreatmentService _treatmentService;
        private readonly IVisitService _visitService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public TreatmentsController(
            ITreatmentService treatmentService,
            IVisitService visitService,
            IPatientService patientService,
            IDoctorService doctorService)
        {
            _treatmentService = treatmentService;
            _visitService = visitService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        private async Task PopulateDoctorsDropdown(int? selectedDoctorId = null)
        {
            var doctors = await _doctorService.GetActiveDoctorsAsync();
            ViewBag.Doctors = new SelectList(doctors, "Id", "FullName", selectedDoctorId);
        }

        // GET: Treatments/Create?visitId=5
        public async Task<IActionResult> Create(int visitId)
        {
            var visit = await _visitService.GetVisitByIdAsync(visitId);
            if (visit == null)
                return NotFound();

            ViewBag.VisitId = visitId;
            ViewBag.PatientId = visit.PatientId;
            ViewBag.PatientName = visit.Patient?.FullName;
            ViewBag.VisitDate = visit.VisitDate;
            await PopulateDoctorsDropdown();

            var treatment = new Treatment
            {
                VisitId = visitId,
                PatientId = visit.PatientId,
                TreatmentDate = visit.VisitDate
            };

            return View(treatment);
        }

        // POST: Treatments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Treatment treatment)
        {
            // Remove navigation property validation
            ModelState.Remove("Patient");
            ModelState.Remove("Visit");
            ModelState.Remove("Doctor");

            // Validate ToothNumber range
            if (treatment.ToothNumber.HasValue && (treatment.ToothNumber < 1 || treatment.ToothNumber > 32))
            {
                ModelState.AddModelError("ToothNumber", "رقم السن يجب أن يكون بين 1 و 32");
            }

            // Clear lab cost if no lab cost flag
            if (!treatment.HasLabCost)
            {
                treatment.LabCost = null;
            }

            if (ModelState.IsValid)
            {
                await _treatmentService.CreateTreatmentAsync(treatment);
                TempData["Success"] = "تم إضافة العلاج بنجاح";
                return RedirectToAction("Details", "Patients", new { id = treatment.PatientId });
            }

            var visit = await _visitService.GetVisitByIdAsync(treatment.VisitId);
            ViewBag.VisitId = treatment.VisitId;
            ViewBag.PatientId = treatment.PatientId;
            ViewBag.PatientName = visit?.Patient?.FullName;
            ViewBag.VisitDate = visit?.VisitDate;
            await PopulateDoctorsDropdown(treatment.DoctorId);

            return View(treatment);
        }

        // GET: Treatments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var treatment = await _treatmentService.GetTreatmentByIdAsync(id);
            if (treatment == null)
                return NotFound();

            ViewBag.VisitId = treatment.VisitId;
            ViewBag.PatientId = treatment.PatientId;
            ViewBag.PatientName = treatment.Patient?.FullName;
            ViewBag.VisitDate = treatment.Visit?.VisitDate;
            await PopulateDoctorsDropdown(treatment.DoctorId);

            return View(treatment);
        }

        // POST: Treatments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Treatment treatment)
        {
            if (id != treatment.Id)
                return BadRequest();

            // Remove navigation property validation
            ModelState.Remove("Patient");
            ModelState.Remove("Visit");
            ModelState.Remove("Doctor");

            // Validate ToothNumber range
            if (treatment.ToothNumber.HasValue && (treatment.ToothNumber < 1 || treatment.ToothNumber > 32))
            {
                ModelState.AddModelError("ToothNumber", "رقم السن يجب أن يكون بين 1 و 32");
            }

            // Clear lab cost if no lab cost flag
            if (!treatment.HasLabCost)
            {
                treatment.LabCost = null;
            }

            if (ModelState.IsValid)
            {
                await _treatmentService.UpdateTreatmentAsync(treatment);
                TempData["Success"] = "تم تحديث العلاج بنجاح";
                return RedirectToAction("Details", "Patients", new { id = treatment.PatientId });
            }

            var visit = await _visitService.GetVisitByIdAsync(treatment.VisitId);
            ViewBag.VisitId = treatment.VisitId;
            ViewBag.PatientId = treatment.PatientId;
            ViewBag.PatientName = treatment.Patient?.FullName;
            ViewBag.VisitDate = visit?.VisitDate;
            await PopulateDoctorsDropdown(treatment.DoctorId);

            return View(treatment);
        }

        // POST: Treatments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int patientId)
        {
            await _treatmentService.DeleteTreatmentAsync(id);
            TempData["Success"] = "تم حذف العلاج بنجاح";
            return RedirectToAction("Details", "Patients", new { id = patientId });
        }
    }
}
