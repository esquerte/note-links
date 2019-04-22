using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service.ViewModels
{
    public class DeleteUserModel
    {
        public string Password { get; set; }
        public bool DeleteCalendars { get; set; }
    }
}
