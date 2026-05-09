using Microsoft.AspNetCore.SignalR;
using StallFruitsManagement.Services;
using System.Threading.Tasks;

namespace StallFruitsManagement.Hubs
{
    public class InventoryHub : Hub
    {
        private readonly IInventoryService _inventoryService;

        public InventoryHub(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public override async Task OnConnectedAsync()
        {
            // Send current dashboard state to the newly connected client
            var dashboard = _inventoryService.GetDashboard();
            await Clients.Caller.SendAsync("InventoryChanged", dashboard);
            await base.OnConnectedAsync();
        }

        // Client can request current dashboard explicitly
        public async Task RequestDashboard()
        {
            var dashboard = _inventoryService.GetDashboard();
            await Clients.Caller.SendAsync("InventoryChanged", dashboard);
        }
    }
}
