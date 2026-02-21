using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GoDent.Controllers
{
    public class VisitsController : Controller
    {
        private readonly IVisitService _visitService;
        private readonly IPatientService _patientService;

        public VisitsController(IVisitService visitService, IPatientService patientService)
        {
            _visitService = visitService;
            _patientService = patientService;
        }

        // GET: Visits/Create?patientId=5
        public async Task<IActionResult> Create(int patientId)
        {
            var patient = await _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
                return NotFound();

            ViewBag.PatientId = patientId;
            ViewBag.PatientName = patient.FullName;

            var visit = new Visit
            {
                PatientId = patientId,
                VisitDate = DateTime.Today
            };

            return View(visit);
        }

        // POST: Visits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visit visit)
        {
            // Remove navigation property validation
            ModelState.Remove("Patient");
            ModelState.Remove("Treatments");

            if (ModelState.IsValid)
            {
                await _visitService.CreateVisitAsync(visit);
                TempData["Success"] = "تم إضافة الزيارة بنجاح";
                return RedirectToAction("Details", "Patients", new { id = visit.PatientId });
            }

            var patient = await _patientService.GetPatientByIdAsync(visit.PatientId);
            ViewBag.PatientId = visit.PatientId;
            ViewBag.PatientName = patient?.FullName;

            return View(visit);
        }

        // GET: Visits/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var visit = await _visitService.GetVisitByIdAsync(id);
            if (visit == null)
                return NotFound();

            ViewBag.PatientId = visit.PatientId;
            ViewBag.PatientName = visit.Patient?.FullName;

            return View(visit);
        }

        // POST: Visits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Visit visit)
        {
            if (id != visit.Id)
                return BadRequest();

            // Remove navigation property validation
            ModelState.Remove("Patient");
            ModelState.Remove("Treatments");

            if (ModelState.IsValid)
            {
                await _visitService.UpdateVisitAsync(visit);
                TempData["Success"] = "تم تحديث الزيارة بنجاح";
                return RedirectToAction("Details", "Patients", new { id = visit.PatientId });
            }

            var patient = await _patientService.GetPatientByIdAsync(visit.PatientId);
            ViewBag.PatientId = visit.PatientId;
            ViewBag.PatientName = patient?.FullName;

            return View(visit);
        }

        // POST: Visits/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int patientId)
        {
            // Check if visit has treatments
            var hasTreatments = await _visitService.HasTreatmentsAsync(id);
            if (hasTreatments)
            {
                TempData["Error"] = "لا يمكن حذف زيارة تحتوي على علاجات. قم بحذف العلاجات أولاً";
                return RedirectToAction("Details", "Patients", new { id = patientId });
            }

            await _visitService.DeleteVisitAsync(id);
            TempData["Success"] = "تم حذف الزيارة بنجاح";
            return RedirectToAction("Details", "Patients", new { id = patientId });
        }
    }
}
