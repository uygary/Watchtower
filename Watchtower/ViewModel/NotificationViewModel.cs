using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

using Watchtower.Model;
using Watchtower.Services;

namespace Watchtower.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class NotificationViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly WorkerService _workerService;

        /// <summary>
        /// The <see cref="Repositories" /> property's name.
        /// </summary>
        public const string RepositoriesPropertyName = "Repositories";
        private ObservableCollection<ExtendedRepository> _repositories = new ObservableCollection<ExtendedRepository>();
        /// <summary>
        /// Sets and gets the Repositories property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<ExtendedRepository> Repositories
        {
            get { return _repositories; }
            set
            {
                if (_repositories == value)
                    return;

                RaisePropertyChanging(RepositoriesPropertyName);
                _repositories = value;
                RaisePropertyChanged(RepositoriesPropertyName);
            }
        }


        /// <summary>
        /// Initializes a new instance of the NotificationViewModel class.
        /// </summary>
        public NotificationViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _workerService = SimpleIoc.Default.GetInstance<WorkerService>();
        }

        private void Initialize()
        {
            _workerService.IncomingChangesDetected += OnIncomingChangesDetected;
        }

        private void OnIncomingChangesDetected(object sender, IncomingChangesDetectedEventArgs e)
        {
            Repositories = new ObservableCollection<ExtendedRepository>(e.Repositories);
        }


        public override void Cleanup()
        {
            _workerService.IncomingChangesDetected -= OnIncomingChangesDetected;

            base.Cleanup();
        }
    }
}