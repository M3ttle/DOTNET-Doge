using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace DOGEOnlineGeneralEditor.Services.Hubs
{
    // Our Hub for files, using SignalR library.
    public class FileHub : Hub
    {
        // If group does not exist it is automatically created on first call.
        public Task AddToGroup(string groupName)
        {
            // Context is the client calling
            return Groups.Add(Context.ConnectionId, groupName);
        }
        
        // Runs automatically when connection ends
        public Task RemoveFromGroup(string groupName)
        {
            // Context is the client calling
            return Groups.Remove(Context.ConnectionId, groupName);
        }

        // Broadcasts message to others in group.
        public void BroadcastFileToGroup(string groupName, string value, int row, int column)
        {
            Clients.OthersInGroup(groupName).updateFile(value, row, column);
            //Clients.All.updateFile(value, row, column); // For testing purposes
        }

        // Overrides
        public override Task OnConnected()  // TODO Add counter on reconnect etc...
        {
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
    }
}