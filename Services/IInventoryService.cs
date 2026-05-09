using System.Collections.Generic;
using StallFruitsManagement.Models;

namespace StallFruitsManagement.Services
{
    public interface IInventoryService
    {
        IEnumerable<InventoryItem> GetAll();
        InventoryItem? GetById(int id);
        InventoryItem Add(InventoryItem item);
        void Update(InventoryItem item);
        bool Delete(int id);
        DashboardViewModel GetDashboard();
    }
}
