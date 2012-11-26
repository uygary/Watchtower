using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

using Watchtower.Models;
using Watchtower.Services;

namespace Watchtower.ViewModels
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly PluginService _pluginService;
        private readonly WorkerService _workerService;
        public RelayCommand<DragEventArgs> DragEnterCommand { get; private set; }
        public RelayCommand<DragEventArgs> DragLeaveCommand { get; private set; }
        public RelayCommand<DragEventArgs> DropCommand { get; private set; }
        public RelayCommand<object> DeleteCommand { get; private set; }
        public RelayCommand LoadCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        /// <summary>
        /// The <see cref="Status" /> property's name.
        /// </summary>
        public const string StatusPropertyName = "Status";
        private string _status;
        /// <summary>
        /// Sets and gets the Status property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                if (string.Equals(_status, value))
                    return;

                RaisePropertyChanging(RepositoriesPropertyName);
                _status = value;
                RaisePropertyChanged(RepositoriesPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Repositories" /> property's name.
        /// </summary>
        public const string RepositoriesPropertyName = "Repositories";
        private ObservableCollection<ExtendedRepository> _repositories;
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
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _pluginService = SimpleIoc.Default.GetInstance<PluginService>();
            _workerService = SimpleIoc.Default.GetInstance<WorkerService>();
            Status = Constants.Strings.Ready;

            DeleteCommand = new RelayCommand<object>(DeleteRepository);
            LoadCommand = new RelayCommand(LoadRepositories);
            SaveCommand = new RelayCommand(SaveRepositories);
            DragEnterCommand = new RelayCommand<DragEventArgs>(OnDragEnter);
            DragLeaveCommand = new RelayCommand<DragEventArgs>(OnDragLeave);
            DropCommand = new RelayCommand<DragEventArgs>(OnDrop);

            _workerService.ProgressChanged += OnWorkerServiceProgressChanged;

            Initialize();
        }

        private void Initialize()
        {
            Repositories = new ObservableCollection<ExtendedRepository>();
            LoadRepositories();
        }

        private void DeleteRepository(object o)
        {
            ExtendedRepository repo = (ExtendedRepository)o;
            Repositories.Remove(repo);
        }
        private void LoadRepositories()
        {
            _dataService.BeginGetRepositories(OnGetRepositoriesCompleted);
        }
        private void SaveRepositories()
        {
            _dataService.SaveRepositories(Repositories);
        }

        private void OnGetRepositoriesCompleted(IList<ExtendedRepository> repositories, Exception exception)
        {
            Repositories = new ObservableCollection<ExtendedRepository>(repositories);
        }
        private void OnWorkerServiceProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.WorkerProgress == WorkerProgress.Active)
                Status = Constants.Strings.Checking;
            else
                Status = Constants.Strings.Ready;
        }

        #region Drag and drop related methods

        private void OnDragEnter(DragEventArgs e)
        {
            //e.Effect = DragDropEffects.Link;
        }
        private void OnDragLeave(DragEventArgs e)
        {

        }
        private void OnDrop(DragEventArgs e)
        {
            //var files = e.Data.GetData(DataFormats.FileDrop) as DirectoryInfo[];
            //if (null != files && files.Length > 0)
            //{
            //    foreach (DirectoryInfo di in files)
            //    {
            //        string path = di.FullName;
            //    }
            //}

            string[] repositories = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string path in repositories)
            {
                bool repoIsNew = true;
                ExtendedRepository repository = new ExtendedRepository(string.Empty, path);

                foreach (ExtendedRepository r in Repositories)
                {
                    if (string.Equals(path, r.Path))
                    {
                        repoIsNew = false;
                        break;
                    }
                }
                if (repoIsNew && _pluginService.SetRepositoryType(ref repository))
                {
                    Repositories.Add(repository);

                    //FIXME: This is ugly.
                    if (string.IsNullOrEmpty(repository.Name))
                    {
                        string[] s = repository.Path.Split('\\');
                        string name = s[s.Length - 1];
                        repository.Name = name;
                    }
                }
            }
        }

        #endregion


        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}