using System.Collections.Generic;

namespace StallFruitsManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalItems { get; set; }
        public decimal FreshStock { get; set; }
        public int LowStockItems { get; set; }
        public int SpoiledItems { get; set; }
        public List<InventoryItem> RecentItems { get; set; } = new();
    }
}
