using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public class IntHelper
    {

        public static decimal GetPercentage (double totalValue, double desiredParam)
        {
            if (totalValue == 0) return 0;
            var percentage = Convert.ToDecimal((desiredParam * 100) / totalValue);
            
            return decimal.Round(percentage, 0, MidpointRounding.AwayFromZero); 
        }

    }
}
