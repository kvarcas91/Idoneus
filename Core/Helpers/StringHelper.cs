using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class StringHelper
    {
        /// <summary>
        /// Checks if string is null, empty of contains only white spaces
        /// </summary>
        /// <param name="param">given string to check</param>
        /// <returns>true if given parameter is not null, empty or contains only white spaces</returns>
        public static bool CanUse(string param)
        {
            return !string.IsNullOrWhiteSpace(param);
        }

    }
}
