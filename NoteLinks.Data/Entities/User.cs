using Microsoft.AspNetCore.Identity;
using NoteLinks.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Entities
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }
        public List<UserCalendar> UserCalendars { get; set; } = new List<UserCalendar>();        
    }
}
