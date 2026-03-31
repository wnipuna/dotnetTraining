using Microsoft.AspNetCore.SignalR;

namespace EmployeeWebAPI.Hubs
{
    public class EmployeeHub : Hub
    {
        public async Task SendEmployeeNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveEmployeeNotification", message);
        }

        public async Task NotifyEmployeeAdded(object employee)
        {
            await Clients.All.SendAsync("EmployeeAdded", employee);
        }

        public async Task NotifyEmployeeUpdated(object employee)
        {
            await Clients.All.SendAsync("EmployeeUpdated", employee);
        }

        public async Task NotifyEmployeeDeleted(int employeeId)
        {
            await Clients.All.SendAsync("EmployeeDeleted", employeeId);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
