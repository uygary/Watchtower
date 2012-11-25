using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Ioc;

using Watchtower.Models;
using Watchtower.Views;

namespace Watchtower.Services
{
    public class WorkerService
    {
        public event IncomingChangesDetectedEventHandler IncomingChangesDetected;
        public event ProgressChangedEventHandler ProgressChanged;
        private readonly IDataService _dataService;
        private DispatcherTimer _timer;
        private bool _workSequential;
        private List<ExtendedRepository> _repositoriesToCheck;
        private Dictionary<string, ExtendedRepository> _updatedRepositories;

        private WorkerProgress _progress;
        public WorkerProgress Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnProgressChanged();
            }
        }

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

            int period = _dataService.GetConfiguration().UpdatePeriod;
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, period, 0);
            //_timer.Interval = new TimeSpan(0, 0, 5);
            _timer.Tick += new EventHandler(OnTimerTick);

            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            //_timer.Stop();
            Progress = WorkerProgress.Active;
            _updatedRepositories.Clear();
            _dataService.BeginGetRepositories(OnGetRepositoriesCompleted);
        }

        #region Callbacks

        private void OnGetRepositoriesCompleted(IList<ExtendedRepository> repositories, Exception exception)
        {
            if (null == repositories || repositories.Count == 0)
            {
                Progress = WorkerProgress.Idle;
                return;
            }

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
                Progress = WorkerProgress.Idle;

                if (_updatedRepositories.Count > 0)
                    OnIncomingChangesDetected();
            }
        }

        #endregion


        private void OnIncomingChangesDetected()
        {
            if (null != IncomingChangesDetected)
                IncomingChangesDetected(this, new IncomingChangesDetectedEventArgs(new ObservableCollection<ExtendedRepository>(_updatedRepositories.Values)));
        }
        private void OnProgressChanged()
        {
            if (null != ProgressChanged)
                ProgressChanged(this, new ProgressChangedEventArgs(Progress));
        }
    }


    public enum WorkerProgress
    {
        Idle,
        Active
    }


    #region Events

    #region Repository update event
    public delegate void IncomingChangesDetectedEventHandler(object sender, IncomingChangesDetectedEventArgs e);
    public class IncomingChangesDetectedEventArgs: EventArgs
    {
        public IncomingChangesDetectedEventArgs(ObservableCollection<ExtendedRepository> repositories)
        {
            Repositories = repositories;
        }

        public readonly ObservableCollection<ExtendedRepository> Repositories;
    }
    #endregion

    #region Progress change event
    public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);
    public class ProgressChangedEventArgs : EventArgs
    {
        public ProgressChangedEventArgs(WorkerProgress workerProgress)
        {
            WorkerProgress = workerProgress;
        }

        public readonly WorkerProgress WorkerProgress;
    }
    #endregion

    #endregion
}