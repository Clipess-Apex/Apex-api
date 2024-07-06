using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Threading.Tasks;

public class NotificationHub : Hub
{
    public async Task SendNotification(string message)
    {
        // Make sure the message is properly serialized as JSON
        var notification = new { Message = message, CreatedAt = DateTime.UtcNow.ToString("o") }; // ISO 8601 format
        var serializedNotification = JsonSerializer.Serialize(notification);
        await Clients.All.SendAsync("ReceiveNotification", serializedNotification);
    }
}
