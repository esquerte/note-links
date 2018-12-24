using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Web.SignalR
{
    public class CalendarHub : Hub
    {
        public async Task Change(string calendarCode)
        {
            await Clients.Others.SendAsync("Update", calendarCode);
        }
    }
}
