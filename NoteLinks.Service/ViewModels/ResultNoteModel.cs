﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service.ViewModels
{
    public class ResultNoteModel
    {
        public List<NoteModel> Notes { get; set; }
        public int TotalCount { get; set; }
    }
}
