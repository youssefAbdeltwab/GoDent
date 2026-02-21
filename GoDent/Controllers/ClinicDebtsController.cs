using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GoDent.Controllers
{
    public class ClinicDebtsController : Controller
    {
        private readonly IClinicDebtService _debtService;

        public ClinicDebtsController(IClinicDebtService debtService)
        {
            _debtService = debtService;
        }

        // GET: ClinicDebts
        public async Task<IActionResult> Index()
        {
            // Auto-detect overdue debts
            await _debtService.GetOverdueDebtsAsync();

            var debts = await _debtService.GetAllDebtsAsync();
            ViewBag.TotalPending = await _debtService.GetTotalPendingDebtsAsync();

            return View(debts);
        }

        // GET: ClinicDebts/Create
        public IActionResult Create()
        {
            var debt = new ClinicDebt
            {
                DueDate = DateTime.Today
            };
            return View(debt);
        }

        // POST: ClinicDebts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClinicDebt debt)
        {
            if (ModelState.IsValid)
            {
                await _debtService.CreateDebtAsync(debt);
                TempData["Success"] = "تم إضافة الدين بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(debt);
        }

        // GET: ClinicDebts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var debt = await _debtService.GetDebtByIdAsync(id);
            if (debt == null)
                return NotFound();

            return View(debt);
        }

        // POST: ClinicDebts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClinicDebt debt)
        {
            if (id != debt.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _debtService.UpdateDebtAsync(debt);
                TempData["Success"] = "تم تحديث الدين بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(debt);
        }

        // POST: ClinicDebts/MarkAsPaid/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            await _debtService.MarkAsPaidAsync(id);
            TempData["Success"] = "تم تسديد الدين بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // POST: ClinicDebts/GenerateRecurring
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecurring()
        {
            await _debtService.GenerateRecurringDebtsAsync();
            TempData["Success"] = "تم توليد الديون المتكررة لهذا الشهر";
            return RedirectToAction(nameof(Index));
        }

        // POST: ClinicDebts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _debtService.DeleteDebtAsync(id);
            TempData["Success"] = "تم حذف الدين بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
