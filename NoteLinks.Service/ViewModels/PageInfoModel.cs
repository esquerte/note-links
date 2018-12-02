using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service.ViewModels
{
    public class PageInfoModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public bool Desc { get; set; }
    }
}
