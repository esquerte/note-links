using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Service.Helpers
{
    public static class CodeHelper
    {
        public static string GetCode()
        {
            string code = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            code = code.Replace("/", "_").Replace("+", "-");
            return code.Substring(0, 22);
        }
    }
}
