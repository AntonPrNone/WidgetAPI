using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace API_Library
{
    public static class AnimationHelper
    {
        public static void FadeIn(this UIElement element, double durationSeconds = 0.5)
        {
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(durationSeconds),
            };
            element.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public static void FadeOut(this UIElement element, double durationSeconds = 0.5)
        {
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(durationSeconds),
            };
            element.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public static async Task FadeOut2Async(UIElement element, double durationSeconds = 0.5)
        {
            if (element == null) return;

            var storyboard = new Storyboard();
            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(durationSeconds)
            };
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            storyboard.Completed += (s, e) => element.Visibility = Visibility.Collapsed;

            storyboard.Begin();

            await Task.Delay(TimeSpan.FromSeconds(durationSeconds));
        }
    }
}
