using System;
using System.Collections.Generic;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Ioc;

using Watchtower.Model;
using Watchtower.View;

namespace Watchtower.Services
{
    public class WorkerService
    {
        public event IncomingChangesDetectedEventHandler IncomingChangesDetected;
        private readonly IDataService _dataService;
        private DispatcherTimer _timer;
        private Dictionary<string, ExtendedRepository> _updatedRepositories;
        public Dictionary<string, ExtendedRepository> UpdatedRepositories
        {
            get { return _updatedRepositories; }
            set
            {
                _updatedRepositories = value;
                //RaisePropertyChanged(UpdatedRepositories);
            }
        }

        public WorkerService(IDataService dataService)
        {
            _dataService = dataService;
            UpdatedRepositories = new Dictionary<string, ExtendedRepository>();
            Initialize();
        }
        private void Initialize()
        {
            int period = _dataService.ReadConfiguration().UpdatePeriod;
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, period, 0);
            _timer.Tick += new EventHandler(OnTimerTick);

            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            UpdatedRepositories.Clear();
            _dataService.GetRepositories(OnGetRepositoriesCompleted);
        }

        #region Callbacks

        //TODO: check repositories sequentially.
        private void OnGetRepositoriesCompleted(IList<ExtendedRepository> repositories, Exception exception)
        {
            foreach (ExtendedRepository repo in repositories)
            {
                _dataService.GetIncomingChanges(repo, OnGetIncomingChangesCompleted);
            }
        }
        private void OnGetIncomingChangesCompleted(ExtendedRepository repository, Exception exception)
        {
            if (null != repository && null != repository.IncomingChangesets && repository.IncomingChangesets.Count > 0)
            {
                UpdatedRepositories.Add(repository.Name, repository);
                OnIncomingChangesDetected();
            }
        }

        #endregion


        #region OnIncomingChangesDetected

        private void OnIncomingChangesDetected()
        {
            //Show NotificationWindow.
            NotificationWindow nv = SimpleIoc.Default.GetInstance<NotificationWindow>();
            nv.FadeIn();

            //Fire event.
            if (null != IncomingChangesDetected)
                IncomingChangesDetected(this, new IncomingChangesDetectedEventArgs(new List<ExtendedRepository>(UpdatedRepositories.Values)));
        }

        #endregion

    }


    #region Repository update event
    public delegate void IncomingChangesDetectedEventHandler(object sender, IncomingChangesDetectedEventArgs e);
    public class IncomingChangesDetectedEventArgs: EventArgs
    {
        public IncomingChangesDetectedEventArgs(IList<ExtendedRepository> repositories)
        {
            Repositories = repositories;
        }

        public IList<ExtendedRepository> Repositories { get; set; }
    }
    #endregion

}