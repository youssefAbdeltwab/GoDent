using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GoDent.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;

        public AppointmentsController(IAppointmentService appointmentService, IPatientService patientService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return View(appointments);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            ViewData["PatientId"] = new SelectList(patients, "Id", "FullName");
            
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            // Remove validation errors for navigation properties
            // We only need PatientId, not the full Patient object
            ModelState.Remove("Patient");
            
            // Ensure EndTime is set correctly for the slot system (30 mins)
            // This prevents errors if the form doesn't send EndTime
            if (appointment.StartTime != TimeSpan.Zero)
            {
                appointment.EndTime = appointment.StartTime.Value.Add(TimeSpan.FromMinutes(30));
            }

            if (ModelState.IsValid)
            {
                try 
                {
                    await _appointmentService.AddAppointmentAsync(appointment);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            
            var patients = await _patientService.GetAllPatientsAsync();
            ViewData["PatientId"] = new SelectList(patients, "Id", "FullName", appointment.PatientId);
            return View(appointment);
        }

        // AJAX: Get Slots
        [HttpGet]
        public async Task<IActionResult> GetSlots(DateTime date)
        {
            var slots = await _appointmentService.GetDailySlotsAsync(date);
            return Json(slots);
        }

        // GET: Appointments/Update
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            var patients = await _patientService.GetAllPatientsAsync();
            ViewData["PatientId"] = new SelectList(patients, "Id", "FullName");
            ViewData["Date"] = appointment.AppointmentDate.ToString("yyyy-MM-dd");

            return View();
        }

    }
}
