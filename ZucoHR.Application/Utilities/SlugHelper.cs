using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Application.Utilities
{
    public static class SlugHelper
    {
        public static string Generate(string text)
        {
            return text
                .Trim()
                .ToLower()
                .Replace(" ", "-");
        }
    }
}
