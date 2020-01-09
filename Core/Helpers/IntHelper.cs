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
            
            return percentage; 
        }

        public static decimal GetRoundedPercentage(double totalValue, double desiredParam)
        {
            if (totalValue == 0) return 0;
            var percentage = Convert.ToDecimal((desiredParam * 100) / totalValue);

            return decimal.Round(percentage, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// (desiredParam * weight) / totalValue
        /// </summary>
        /// <param name="totalValue">Based on what will this calculation operate</param>
        /// <param name="desiredParam">parameter which percentage you want to find</param>
        /// <param name="weight">total weight</param>
        /// <returns>percentage</returns>
        public static decimal GetRoundedPercentage(double totalValue, double desiredParam, double weight)
        {
            if (weight == 0) throw new ArgumentException("parameters cannot be 0");

            if (totalValue == 0) return 0;
            var percentage = Convert.ToDecimal((desiredParam * weight) / totalValue);

            return decimal.Round(percentage, 0, MidpointRounding.AwayFromZero);
        }

        public static decimal GetPercentage(double totalValue, double desiredParam, double weight)
        {
            if (weight == 0) throw new ArgumentException("parameters cannot be 0");

            if (totalValue == 0) return 0;
            var percentage = Convert.ToDecimal((desiredParam * weight) / totalValue);

            return percentage;
        }

        public static decimal Round(decimal value)
        {
            return decimal.Round(value, 0, MidpointRounding.AwayFromZero);
        }



    }
}
