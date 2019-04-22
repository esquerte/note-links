using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteLinks.Service.Authentication
{
    public class AuthOptions
    {
        public const string ISSUER = "NoteLinksService";
        public const string AUDIENCE = "ClientApplications";
        const string KEY = "J[ei<oeiy48^zc]kc,yeqo*87";
        public const int LIFETIME = 30;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
