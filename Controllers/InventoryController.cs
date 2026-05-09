using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StallFruitsManagement.Models;
using StallFruitsManagement.Hubs;
using StallFruitsManagement.Services;

namespace StallFruitsManagement.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IInventoryService _inventoryService;
        private readonly IHubContext<InventoryHub> _hubContext;

        public InventoryController(IWebHostEnvironment webHostEnvironment, IInventoryService inventoryService, IHubContext<InventoryHub> hubContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _inventoryService = inventoryService;
            _hubContext = hubContext;
        }

        private async Task<string?> SaveUploadedFile(IFormFile? file)
        {
            if (file is null || file.Length == 0)
                return null;

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
                return null;

            // Create uploads directory if it doesn't exist
            var uploadsDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDirectory))
                Directory.CreateDirectory(uploadsDirectory);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsDirectory, fileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{fileName}";
        }

        public IActionResult Index(string? category)
        {
            var model = _inventoryService.GetAll();
            if (!string.IsNullOrWhiteSpace(category) && Enum.TryParse<CategoryType>(category, out var activeCategory))
            {
                model = model.Where(i => i.Category == activeCategory);
            }

            ViewData["SelectedCategory"] = category;
            return View(model);
        }

        public IActionResult Details(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = _inventoryService.GetById(id.Value);
            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryItem item, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }

            // Handle file upload
            if (imageFile is not null)
            {
                item.ImagePath = await SaveUploadedFile(imageFile);
            }

            var added = _inventoryService.Add(item);
            // Notify connected clients about the change
            await _hubContext.Clients.All.SendAsync("InventoryChanged", _inventoryService.GetDashboard());

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = _inventoryService.GetById(id.Value);
            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InventoryItem item, IFormFile? imageFile)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(item);
            }

            // Handle file upload
            if (imageFile is not null)
            {
                item.ImagePath = await SaveUploadedFile(imageFile);
            }

            _inventoryService.Update(item);
            await _hubContext.Clients.All.SendAsync("InventoryChanged", _inventoryService.GetDashboard());

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = _inventoryService.GetById(id.Value);
            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var removed = _inventoryService.Delete(id);
            if (removed)
            {
                await _hubContext.Clients.All.SendAsync("InventoryChanged", _inventoryService.GetDashboard());
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
