using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils
{
    public class ColorPool
    {

        private static readonly string[] Colors = {
            "#B043EF",  // purple
            "#5757ED",  // dark blue
            "#549DEA",  // light blue 
            "#9c27b0",  // purple
            "#d81b60",  // pink
            "#4caf50",  // green
            "#8bc34a",  // light green
            "#c0ca33",  // lime
            "#fbc02d",  // Yellow
        };

        private static Stack<string> mColours = new Stack<string>(Colors);

        public static string GetColor()
        {
            if (mColours.Count() == 0) mColours = new Stack<string>(Colors);
            return mColours.Pop();
        }

    }
}
