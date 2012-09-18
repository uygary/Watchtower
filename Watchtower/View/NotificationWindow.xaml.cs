using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

using Watchtower.ViewModel;

namespace Watchtower.View
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        public NotificationWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void Show()
        {
            base.Show();
            Initialize();
        }

        private void Initialize()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

                this.Left = corner.X - this.ActualWidth - 8;
                this.Top = corner.Y - this.ActualHeight - 8;
            }));
        }

        internal void FadeIn()
        {
            if (!this.IsVisible)
                Show();
            Storyboard fadeAnimation = (Storyboard)FindResource("FadeInStoryboard");
            fadeAnimation.Begin(this);
        }
        internal void FadeOut()
        {
            Storyboard fadeAnimation = (Storyboard)FindResource("FadeOutStoryboard");
            fadeAnimation.Begin(this);
        }

        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            FadeOut();
        }
    }
}
