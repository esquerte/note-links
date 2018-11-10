using NoteLinks.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service.ViewModels
{
    public class NoteModel
    {
        public NoteModel() {}
        public NoteModel(Note note)
        {
            Id = note.Id;
            Name = note.Name;
            Text = note.Text;
            FromDate = note.FromDate;
            ToDate = note.ToDate;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
