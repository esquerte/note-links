using NoteLinks.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Entities
{
    public class UserCalendar
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int CalendarId { get; set; }
        public Calendar Calendar { get; set; }
    }
}
