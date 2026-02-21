using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Data;
using GoDent.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GoDent.Controllers
{
    public class ToolsController : Controller
    {
        private readonly IToolService _toolService;
        private readonly IWhatsAppService _whatsAppService;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ToolsController(IToolService toolService, IWhatsAppService whatsAppService, AppDbContext context, IConfiguration configuration)
        {
            _toolService = toolService;
            _whatsAppService = whatsAppService;
            _context = context;
            _configuration = configuration;
        }

        // GET: Tools
        public async Task<IActionResult> Index()
        {
            var tools = await _toolService.GetAllToolsAsync();
            ViewData["Departments"] = await _context.Departments.ToListAsync();
            return View(tools);
        }

        // GET: Tools/ByDepartment/5
        public async Task<IActionResult> ByDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return BadRequest();

            var tools = await _toolService.GetToolsByDepartmentIdAsync(id);
            ViewData["DepartmentName"] = department.Name;
            ViewData["Departments"] = await _context.Departments.ToListAsync();
            return View(tools);
        }

        // GET: Tools/Create
        public async Task<IActionResult> Create()
        {
            var departments = await _context.Departments.ToListAsync();
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "Name");
            return View();
        }

        // POST: Tools/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tool tool)
        {
            // Remove validation errors for navigation properties
            ModelState.Remove("Department");

            if (ModelState.IsValid)
            {
                await _toolService.CreateToolAsync(tool);
                TempData["Success"] = "تم إضافة الأداة بنجاح";
                return RedirectToAction(nameof(Index));
            }

            var departments = await _context.Departments.ToListAsync();
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "Name", tool.DepartmentId);
            return View(tool);
        }

        // GET: Tools/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tool = await _toolService.GetToolByIdAsync(id);
            if (tool == null)
                return BadRequest();

            var departments = await _context.Departments.ToListAsync();
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "Name", tool.DepartmentId);
            return View(tool);
        }

        // POST: Tools/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tool tool)
        {
            if (tool is null)
                return BadRequest();

            // Remove validation errors for navigation properties
            ModelState.Remove("Department");

            if (ModelState.IsValid)
            {
                var updatedTool = await _toolService.UpdateToolAsync(tool);
                
                // Check for shortage and generate WhatsApp alert
                if (updatedTool != null && updatedTool.Quantity <= updatedTool.MinQuantity)
                {
                    var doctorPhone = _configuration["WhatsApp:DoctorPhone"];
                    if (!string.IsNullOrEmpty(doctorPhone))
                    {
                        var whatsappUrl = _whatsAppService.GenerateShortageAlertUrl(updatedTool);
                        TempData["WhatsAppAlert"] = whatsappUrl;
                        TempData["ShortageToolName"] = updatedTool.Name;
                    }
                }
                
                TempData["Success"] = "تم تحديث الأداة بنجاح";
                return RedirectToAction(nameof(Index));
            }

            var departments = await _context.Departments.ToListAsync();
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "Name", tool.DepartmentId);
            return View(tool);
        }

        // GET: Tools/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var tool = await _toolService.GetToolByIdAsync(id);
            if (tool == null)
                return BadRequest();

            return View(tool);
        }

        // POST: Tools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _toolService.DeleteToolAsync(id);
            TempData["Success"] = "تم حذف الأداة بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
