using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private bool _workSequential;
        private List<ExtendedRepository> _repositoriesToCheck;
        private Dictionary<string, ExtendedRepository> _updatedRepositories;

        public WorkerService(IDataService dataService)
        {
            _dataService = dataService;
            _repositoriesToCheck = new List<ExtendedRepository>();
            _updatedRepositories = new Dictionary<string, ExtendedRepository>();
            Initialize();
        }
        private void Initialize()
        {
            //TODO: Make sequential progress an option. Read it from configuration.
            _workSequential = true;

            int period = _dataService.ReadConfiguration().UpdatePeriod;
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, period, 0);
            _timer.Tick += new EventHandler(OnTimerTick);

            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _updatedRepositories.Clear();
            _dataService.BeginGetRepositories(OnGetRepositoriesCompleted);
        }

        #region Callbacks

        private void OnGetRepositoriesCompleted(IList<ExtendedRepository> repositories, Exception exception)
        {
            foreach (ExtendedRepository repo in repositories)
            {
                _repositoriesToCheck.Add(repo);
            }

            if (_workSequential)
            {
                if (_repositoriesToCheck.Count > 0)
                {
                    ExtendedRepository firstRepo = _repositoriesToCheck[0];
                    _dataService.BeginGetIncomingChanges(firstRepo, OnGetIncomingChangesCompleted);
                }
            }
            else
            {
                if (_repositoriesToCheck.Count > 0)
                {
                    foreach (ExtendedRepository repo in _repositoriesToCheck)
                    {
                        _dataService.BeginGetIncomingChanges(repo, OnGetIncomingChangesCompleted);
                    }
                }
            }
        }
        private void OnGetIncomingChangesCompleted(ExtendedRepository repository, Exception exception)
        {
            _repositoriesToCheck.RemoveAt(0);

            if (null != repository && null != repository.IncomingChangesets && repository.IncomingChangesets.Count > 0)
            {
                _updatedRepositories.Add(repository.Name, repository);
            }

            if (_repositoriesToCheck.Count > 0)
            {
                ExtendedRepository firstRepo = _repositoriesToCheck[0];
                _dataService.BeginGetIncomingChanges(firstRepo, OnGetIncomingChangesCompleted);
            }
            else
            {
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
                IncomingChangesDetected(this, new IncomingChangesDetectedEventArgs(new ObservableCollection<ExtendedRepository>(_updatedRepositories.Values)));
        }

        #endregion

    }


    #region Repository update event
    public delegate void IncomingChangesDetectedEventHandler(object sender, IncomingChangesDetectedEventArgs e);
    public class IncomingChangesDetectedEventArgs: EventArgs
    {
        public IncomingChangesDetectedEventArgs(ObservableCollection<ExtendedRepository> repositories)
        {
            Repositories = repositories;
        }

        public ObservableCollection<ExtendedRepository> Repositories { get; set; }
    }
    #endregion

}