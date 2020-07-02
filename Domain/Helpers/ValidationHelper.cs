using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Helpers
{
    public class ValidationHelper
    {
        public static bool Validate(params string[] input)
        {
            foreach (var item in input)
            {
                if (string.IsNullOrEmpty(item)) return false;
            }

            return true;
        }
    }
}
