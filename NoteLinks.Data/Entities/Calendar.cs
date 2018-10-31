using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Entities
{
    public class Calendar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        public string Code { get; set; }
        public List<Note> Notes { get; set; }
    }
}
