using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Entities;
using GoDent.DAL.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GoDent.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        // GET: Expenses
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, ExpenseCategory? category)
        {
            IEnumerable<Expense> expenses;

            if (from.HasValue && to.HasValue)
            {
                expenses = await _expenseService.GetExpensesByDateRangeAsync(from.Value, to.Value);
                ViewBag.FromDate = from.Value.ToString("yyyy-MM-dd");
                ViewBag.ToDate = to.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                expenses = await _expenseService.GetAllExpensesAsync();
            }

            if (category.HasValue)
            {
                expenses = expenses.Where(e => e.Category == category.Value);
                ViewBag.SelectedCategory = category.Value;
            }

            ViewBag.TotalExpenses = expenses.Sum(e => e.Amount);

            return View(expenses);
        }

        // GET: Expenses/Create
        public IActionResult Create()
        {
            var expense = new Expense
            {
                ExpenseDate = DateTime.Today
            };
            return View(expense);
        }

        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expense expense)
        {
            if (ModelState.IsValid)
            {
                await _expenseService.CreateExpenseAsync(expense);
                TempData["Success"] = "تم إضافة المصروف بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
                return NotFound();

            return View(expense);
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Expense expense)
        {
            if (id != expense.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _expenseService.UpdateExpenseAsync(expense);
                TempData["Success"] = "تم تحديث المصروف بنجاح";
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _expenseService.DeleteExpenseAsync(id);
            TempData["Success"] = "تم حذف المصروف بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
