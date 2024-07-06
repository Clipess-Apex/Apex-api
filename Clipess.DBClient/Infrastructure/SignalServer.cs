using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Clipess.DBClient.Contracts;
using Clipess.DBClient.EntityModels;
using Clipess.DBClient.Infrastructure;
using Clipess.DBClient.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Clipess.DBClient.Infrastructure
{
    public interface INotificationClient
    {
        Task RecieveNotification(List<String> notifications);
    }
    public class SignalServer : Hub<INotificationClient>
    {

        private readonly EFDbContext _dbContext;
        public SignalServer(EFDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var httpContext = Context.GetHttpContext();

            var userId = httpContext.Request.Query["userId"].ToString();

            ConnectionManager.AddConnection(userId, connectionId);

            Debug.WriteLine("User Connections :");
            foreach (var item in ConnectionManager._userConnections)
            {
                Debug.WriteLine($"User ID : {item.Key},Connection ID : {item.Value}");
            }

            await base.OnConnectedAsync();
        }

        public async Task SendNotificationToUser(string connectionId, List<string> notifications)
        {
            // Call the interface method directly on the client proxy
            await Clients.Client(connectionId).RecieveNotification(notifications);
        }

    }
}


