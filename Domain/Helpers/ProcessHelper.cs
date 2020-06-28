using System.Diagnostics;

namespace Domain.Helpers
{
    public static class ProcessHelper
    {

        public static void Run(string path)
        {
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
    }
}
