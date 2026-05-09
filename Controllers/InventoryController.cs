using Microsoft.AspNetCore.Mvc;
using StallFruitsManagement.Models;

namespace StallFruitsManagement.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InventoryController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        private static readonly List<InventoryItem> _items = new()
        {
            new InventoryItem { Id = 1, Name = "Apples", Category = CategoryType.Fruit, Quantity = 32, Unit = UnitType.Kilogram, Price = 85m },
            new InventoryItem { Id = 2, Name = "Carrots", Category = CategoryType.Vegetable, Quantity = 18, Unit = UnitType.Kilogram, Price = 50m },
            new InventoryItem { Id = 3, Name = "Bananas", Category = CategoryType.Fruit, Quantity = 24, Unit = UnitType.Kilogram, Price = 40m }
        };

        private static int _nextId = 4;

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
            var model = _items.AsEnumerable();
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

            var item = _items.FirstOrDefault(i => i.Id == id);
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

            item.Id = _nextId++;
            _items.Add(item);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = _items.FirstOrDefault(i => i.Id == id);
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

            var storedItem = _items.FirstOrDefault(i => i.Id == id);
            if (storedItem is null)
            {
                return NotFound();
            }

            storedItem.Name = item.Name;
            storedItem.Category = item.Category;
            storedItem.Quantity = item.Quantity;
            storedItem.Unit = item.Unit;
            storedItem.Price = item.Price;

            // Handle file upload
            if (imageFile is not null)
            {
                storedItem.ImagePath = await SaveUploadedFile(imageFile);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item is null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item is not null)
            {
                _items.Remove(item);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
