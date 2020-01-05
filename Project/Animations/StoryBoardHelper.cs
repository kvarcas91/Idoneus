using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Project.Animations
{
    public static class StoryBoardHelper
    {

        public static void SlideUp (this Storyboard storyboard, double offset, float seconds, float decelerationRatio = 0.9f, bool keepMargin = true)
        {

            var animation = new ThicknessAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(seconds)),
                                    // left, top, right, bottom
                From = new Thickness(0, keepMargin ? offset : 0, 0, -offset),
                To = new Thickness(0),
                DecelerationRatio = decelerationRatio
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            storyboard.Children.Add(animation);

        }

        public static void SlideDown (this Storyboard storyboard, double offset, float seconds, float decelerationRatio = 0.9f, bool keepMargin = true)
        {

        }


    }
}
