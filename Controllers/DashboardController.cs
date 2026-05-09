using Microsoft.AspNetCore.Mvc;
using StallFruitsManagement.Models;
using StallFruitsManagement.Services;

namespace StallFruitsManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public DashboardController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public IActionResult Index()
        {
            var model = _inventoryService.GetDashboard();
            return View(model);
        }
    }
}
