using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Project.Animations
{
    /// <summary>
    /// Helpers to animate framework elements in specific ways
    /// </summary>
    public static class FrameworkElementAnimations
    {
       

       public static async Task SlideUpAsync(this FrameworkElement element, float seconds = 0.3f, bool keepMargin = true, int height = 0)
        {




            await Task.Delay((int)seconds * 1000);

        }

        public static async Task SlideDownAsync(this FrameworkElement element, float seconds = 0.3f, bool keepMargin = true, int height = 0)
        {





            await Task.Delay((int)seconds * 1000);

        }
    }
}
