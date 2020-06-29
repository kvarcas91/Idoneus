using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Domain.Helpers
{
    public static class ProcessHelper
    {

        public static void Run(string path)
        {
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }

        public static void RunLink(string url)
        {
            try
            {
               
                Process.Start(url);
              
              
            }
            catch
            {
               
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                    
               
               
            }
        }
    }
}
