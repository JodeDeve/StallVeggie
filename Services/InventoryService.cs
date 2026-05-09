using System.Collections.Generic;
using System.Linq;
using StallFruitsManagement.Models;

namespace StallFruitsManagement.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly List<InventoryItem> _items = new();
        private int _nextId = 1;
        private readonly object _lock = new();

        public InventoryService()
        {
            // Seed demo data
            _items.Add(new InventoryItem { Id = _nextId++, Name = "Apples", Category = CategoryType.Fruit, Quantity = 50, Unit = UnitType.Kilogram, Price = 85m });
            _items.Add(new InventoryItem { Id = _nextId++, Name = "Carrots", Category = CategoryType.Vegetable, Quantity = 10, Unit = UnitType.Kilogram, Price = 50m });
            _items.Add(new InventoryItem { Id = _nextId++, Name = "Bananas", Category = CategoryType.Fruit, Quantity = 480, Unit = UnitType.Kilogram, Price = 40m });
        }

        public IEnumerable<InventoryItem> GetAll()
        {
            lock (_lock)
            {
                return _items.Select(i => Clone(i)).ToList();
            }
        }

        public InventoryItem? GetById(int id)
        {
            lock (_lock)
            {
                var item = _items.FirstOrDefault(i => i.Id == id);
                return item is null ? null : Clone(item);
            }
        }

        public InventoryItem Add(InventoryItem item)
        {
            lock (_lock)
            {
                var toAdd = Clone(item);
                toAdd.Id = _nextId++;
                _items.Add(toAdd);
                return Clone(toAdd);
            }
        }

        public void Update(InventoryItem item)
        {
            lock (_lock)
            {
                var stored = _items.FirstOrDefault(i => i.Id == item.Id);
                if (stored is null) return;
                stored.Name = item.Name;
                stored.Category = item.Category;
                stored.Quantity = item.Quantity;
                stored.Unit = item.Unit;
                stored.Price = item.Price;
                stored.ImagePath = item.ImagePath;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock)
            {
                var item = _items.FirstOrDefault(i => i.Id == id);
                if (item is null) return false;
                _items.Remove(item);
                return true;
            }
        }

        public DashboardViewModel GetDashboard()
        {
            lock (_lock)
            {
                var vm = new DashboardViewModel
                {
                    TotalItems = _items.Count,
                    FreshStock = _items.Sum(i => i.Quantity),
                    LowStockItems = _items.Count(i => i.Quantity < 20),
                    SpoiledItems = _items.Count(i => i.Quantity == 0),
                    RecentItems = _items.OrderByDescending(i => i.Id).Take(5).Select(i => Clone(i)).ToList()
                };
                return vm;
            }
        }

        private InventoryItem Clone(InventoryItem src)
        {
            return new InventoryItem
            {
                Id = src.Id,
                Name = src.Name,
                Category = src.Category,
                Quantity = src.Quantity,
                Unit = src.Unit,
                Price = src.Price,
                ImagePath = src.ImagePath
            };
        }
    }
}
