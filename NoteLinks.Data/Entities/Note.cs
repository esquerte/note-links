using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Entities
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        public int CalendarId { get; set; }
        public Calendar Calendar { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        public string Text { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
