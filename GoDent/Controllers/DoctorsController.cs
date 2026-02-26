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

            if (ModelState.IsValid)
            {
                await _doctorService.UpdateDoctorAsync(doctor);
                TempData["Success"] = "تم تحديث بيانات الطبيب بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int id, DateTime? from, DateTime? to)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            // Default: current month
            var fromDate = from ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var toDate = to ?? DateTime.Now;

            var earnings = await _doctorService.GetDoctorEarningsAsync(id, fromDate, toDate);

            ViewBag.Earnings = earnings;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _doctorService.DeleteDoctorAsync(id);
            if (deleted)
            {
                TempData["Success"] = "تم حذف الطبيب بنجاح";
            }
            else
            {
                TempData["Error"] = "لا يمكن حذف الطبيب لأن لديه علاجات مسجلة";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
