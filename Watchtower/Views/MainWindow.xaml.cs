using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

using Watchtower.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Watchtower.Services;

namespace Watchtower
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window//, IDisposable //IDisposable and thus sealed is unneccessary.
    {
        //internal System.Windows.Forms.NotifyIcon _trayIcon;
        private WindowState _storedWindowState = WindowState.Normal;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void Initialize()
        {
            NotificationService svc = SimpleIoc.Default.GetInstance<NotificationService>();
            svc._trayIcon.Click += OnIconClicked;

            //_trayIcon = new System.Windows.Forms.NotifyIcon();
            //_trayIcon.BalloonTipText = Constants.Application.BaloonTip;
            //_trayIcon.BalloonTipTitle = Constants.Application.Title;
            //_trayIcon.Text = Constants.Application.Description;

            //SwitchToBrightIcon();
            //_trayIcon.Visible = true;

            //_trayIcon.Click += new EventHandler(OnIconClicked);


            Top = Properties.Settings.Default.Top;
            Left = Properties.Settings.Default.Left;
            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;
            if (Properties.Settings.Default.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
        }


        #region Window state related methods
        
        private void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                //if (_trayIcon != null)
                //    _trayIcon.ShowBalloonTip(2000);
            }
            else
                _storedWindowState = WindowState;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            //CheckTrayIcon();
        }

        private void OnIconClicked(object sender, EventArgs e)
        {
            if (WindowState != System.Windows.WindowState.Minimized)
            {
                WindowState = System.Windows.WindowState.Minimized;
            }
            else
            {
                Show();
                WindowState = _storedWindowState;
            }
        }

        #endregion


        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected override void OnClosed(EventArgs e)
        {
            //_trayIcon.Dispose();
            //_trayIcon = null;

            //Save window position and size.
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = RestoreBounds.Top;
                Properties.Settings.Default.Left = RestoreBounds.Left;
                Properties.Settings.Default.Height = RestoreBounds.Height;
                Properties.Settings.Default.Width = RestoreBounds.Width;
                Properties.Settings.Default.Maximized = true;
            }
            else
            {
                Properties.Settings.Default.Top = Top;
                Properties.Settings.Default.Left = Left;
                Properties.Settings.Default.Height = Height;
                Properties.Settings.Default.Width = Width;
                Properties.Settings.Default.Maximized = false;
            }
            Properties.Settings.Default.Save();

            //Clear resources.
            SimpleIoc.Default.GetInstance<Watchtower.Views.NotificationWindow>().Close();
            Watchtower.Services.NotificationService nSvc = SimpleIoc.Default.GetInstance<Watchtower.Services.NotificationService>();
            try
            {
                nSvc._trayIcon.Click -= OnIconClicked;
            }
            catch
            {
            }
            nSvc.Dispose();

            //And finally close.
            base.OnClosed(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (e.Cancel)
                return;

            //e.Cancel = true;
            base.OnClosing(e);
        }


        //public void Dispose()
        //{
        //    _trayIcon.Click -= OnIconClicked;
        //    _trayIcon.Dispose();
        //    _trayIcon = null;
        //    //Dispose(true);
        //    //GC.SuppressFinalize(this);
        //}
    }
}