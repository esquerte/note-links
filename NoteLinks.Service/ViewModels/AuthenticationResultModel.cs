using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service.ViewModels
{
    public class AuthenticationResultModel
    {
        public string AccessToken { get; set; }
        public string DisplayName { get; set; }
        public int ExpirationTime { get; set; }
    }
}
