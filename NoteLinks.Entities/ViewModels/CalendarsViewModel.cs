using NoteLinks.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Entities.ViewModels
{
    public class CalendarsViewModel
    {
        public string Name { get; set; }
        public List<Note> Notes { get; set; }
    }
}
