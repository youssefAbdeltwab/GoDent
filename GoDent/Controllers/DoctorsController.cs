using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GoDent.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return View(doctors);
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _doctorService.GetDoctorWithDetailsAsync(id);
            if (doctor == null)
                return NotFound();

            var treatments = doctor.Treatments.OrderByDescending(t => t.TreatmentDate).ToList();
            ViewBag.TotalCost = treatments.Sum(t => t.Cost);
            ViewBag.TotalLabCost = treatments.Where(t => t.HasLabCost).Sum(t => t.LabCost ?? 0);
            ViewBag.TotalNet = treatments.Sum(t => t.NetCost);
            ViewBag.TotalDoctorShare = treatments.Sum(t => t.DoctorShare);
            ViewBag.TotalClinicShare = treatments.Sum(t => t.ClinicShare);

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                await _doctorService.CreateDoctorAsync(doctor);
                TempData["Success"] = "تم إضافة الطبيب بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Doctor doctor)
        {
            if (id != doctor.Id)
                return BadRequest();

            // Remove navigation property validation
            ModelState.Remove("Treatments");

            if (ModelState.IsValid)
            {
                await _doctorService.UpdateDoctorAsync(doctor);
                TempData["Success"] = "تم تحديث بيانات الطبيب بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        // POST: Doctors/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            await _doctorService.ToggleActiveAsync(id);
            TempData["Success"] = "تم تحديث حالة الطبيب";
            return RedirectToAction(nameof(Index));
        }

        // POST: Doctors/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor != null && doctor.Treatments.Any())
            {
                TempData["Error"] = "لا يمكن حذف الطبيب لوجود علاجات مرتبطة به. يمكنك تعطيله بدلاً من ذلك.";
                return RedirectToAction(nameof(Index));
            }

            await _doctorService.DeleteDoctorAsync(id);
            TempData["Success"] = "تم حذف الطبيب بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
