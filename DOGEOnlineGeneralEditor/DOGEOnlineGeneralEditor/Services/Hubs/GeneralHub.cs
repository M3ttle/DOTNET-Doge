using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DOGEOnlineGeneralEditor.Services.Hubs
{
    public class GeneralHub : Hub
    {
        private static int visitorCount = 0;

        public void RefreshCount()
        {
            Clients.All.updateVisitorCounter(visitorCount);
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            visitorCount++;
            RefreshCount();
            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            visitorCount--;
            RefreshCount();
            return base.OnDisconnected(stopCalled);
        }
    }
}