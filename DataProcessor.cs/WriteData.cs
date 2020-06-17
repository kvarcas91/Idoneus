using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.cs
{
    public class WriteData
    {
        public static Response Write(string path, Action<string> setter)
        {

                setter("Creating file");
                Task.Delay(500).Wait();
                setter("Creating Generating report");
                Task.Delay(3000).Wait();
                setter("Writing report..");
                Task.Delay(1000).Wait();
                setter("Saving file");
                Task.Delay(500).Wait();
            return new Response
            {
                Success = true
            };

        }


    }
}
