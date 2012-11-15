using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

using Watchtower.ViewModels;
using GalaSoft.MvvmLight.Ioc;

namespace Watchtower
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, IDisposable //IDisposable and thus sealed is unneccessary.
    {
        internal System.Windows.Forms.NotifyIcon _trayIcon;
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
            _trayIcon = new System.Windows.Forms.NotifyIcon();
            _trayIcon.BalloonTipText = Constants.Application.BaloonTip;
            _trayIcon.BalloonTipTitle = Constants.Application.Title;
            _trayIcon.Text = Constants.Application.Description;

            SwitchToBrightIcon();
            _trayIcon.Visible = true;

            _trayIcon.Click += new EventHandler(OnIconClicked);
        }


        #region Tray icon related methods

        private void SwitchToBrightIcon()
        {
            Stream imageStream = Application.GetResourceStream(new Uri("pack://application:,,/Images/AoP13.ico")).Stream;
            _trayIcon.Icon = new System.Drawing.Icon(imageStream);
        }
        private void SwitchToGlossyIcon()
        {
            Stream imageStream = Application.GetResourceStream(new Uri("pack://application:,,/Images/AoP13Glossy.ico")).Stream;
            _trayIcon.Icon = new System.Drawing.Icon(imageStream);
        }

        private void OnClose(object sender, CancelEventArgs args)
        {
            _trayIcon.Dispose();
            _trayIcon = null;
        }

        private void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (_trayIcon != null)
                    _trayIcon.ShowBalloonTip(2000);
            }
            else
                _storedWindowState = WindowState;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        private void OnIconClicked(object sender, EventArgs e)
        {
            Show();
            WindowState = _storedWindowState;
        }

        private void CheckTrayIcon()
        {
            SwitchTrayIcon(IsVisible);
        }

        private void SwitchTrayIcon(bool show)
        {
            if (_trayIcon != null)
            {
                //_trayIcon.Visible = show;
                if (show)
                    SwitchToBrightIcon();
                else
                    SwitchToGlossyIcon();
            }
        }

        #endregion


        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Watchtower.Views.NotificationWindow>().Close();
            base.OnClosing(e);
        }


        public void Dispose()
        {
            _trayIcon.Dispose();
            _trayIcon = null;
            //Dispose(true);
            //GC.SuppressFinalize(this);
        }
    }
}