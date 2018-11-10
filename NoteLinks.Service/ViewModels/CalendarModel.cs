using NoteLinks.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service.ViewModels
{
    public class CalendarModel
    {
        public CalendarModel() { }
        public CalendarModel(Calendar calendar)
        {
            Code = calendar.Code;
            Name = calendar.Name;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
