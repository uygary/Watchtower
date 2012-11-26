using System;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;

using Watchtower.Views;

namespace Watchtower.Services
{
    public class NotificationService : IDisposable
    {
        private WorkerService _workerService;
        private NotificationWindow _notificationWindow;
        internal System.Windows.Forms.NotifyIcon _trayIcon;

        public NotificationService(WorkerService workerService)
        {
            _workerService = workerService;

            Initialize();
        }

        private void Initialize()
        {
            _trayIcon = new System.Windows.Forms.NotifyIcon();
            _trayIcon.BalloonTipText = Constants.Application.BaloonTip;
            _trayIcon.BalloonTipTitle = Constants.Application.Title;
            _trayIcon.Text = Constants.Application.Description;

            SwitchTrayIcon(_workerService.Progress);
            _trayIcon.Visible = true;

            _notificationWindow = SimpleIoc.Default.GetInstance<NotificationWindow>();
            _workerService.IncomingChangesDetected += OnIncomingChangesDetected;
            _workerService.ProgressChanged += OnWorkerServiceProgressChanged;
        }

        private void ShowNotificationWindow()
        {
            //Show NotificationWindow.
            _notificationWindow = SimpleIoc.Default.GetInstance<NotificationWindow>();
            _notificationWindow.FadeIn();
        }
        private void SwitchTrayIcon(WorkerProgress workerProgress)
        {
            switch (workerProgress)
            {
                case (WorkerProgress.Idle):
                    {
                        Stream imageStream = Application.GetResourceStream(new Uri("pack://application:,,/Images/WatchtowerIdle.ico")).Stream;
                        _trayIcon.Icon = new System.Drawing.Icon(imageStream);
                        break;
                    }
                case (WorkerProgress.Active):
                    {
                        Stream imageStream = Application.GetResourceStream(new Uri("pack://application:,,/Images/WatchtowerActive.ico")).Stream;
                        _trayIcon.Icon = new System.Drawing.Icon(imageStream);
                        break;
                    }
            }
        }


        private void OnIncomingChangesDetected(object sender, IncomingChangesDetectedEventArgs e)
        {
            //SwitchTrayIcon();
            ShowNotificationWindow();
        }
        private void OnWorkerServiceProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SwitchTrayIcon(e.WorkerProgress);
        }


        #region IDisposable members
        public void Dispose()
        {
            if (null != _workerService)
            {
                _workerService.IncomingChangesDetected -= OnIncomingChangesDetected;
                _workerService.ProgressChanged -= OnWorkerServiceProgressChanged;
                _workerService = null;
            }
            if (null != _trayIcon)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
                _trayIcon = null;
                GC.SuppressFinalize(this);
            }
        }
        #endregion

    }
}
