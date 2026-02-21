using System.Diagnostics;
using GoDent.BLL.Service.Abstraction;
using GoDent.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoDent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFinanceDashboardService _dashboardService;

        public HomeController(ILogger<HomeController> logger, IFinanceDashboardService dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {
            // Default to current month
            var fromDate = from ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var toDate = to ?? DateTime.Today;

            ViewBag.FromDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.ToString("yyyy-MM-dd");

            var dashboard = await _dashboardService.GetDashboardAsync(fromDate, toDate);
            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
