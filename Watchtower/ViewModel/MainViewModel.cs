﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly PluginService _pluginService;
        private readonly WorkerService _workerService;
        public RelayCommand<DragEventArgs> DragEnterCommand { get; private set; }
        public RelayCommand<DragEventArgs> DragLeaveCommand { get; private set; }
        public RelayCommand<DragEventArgs> DropCommand { get; private set; }
        public RelayCommand LoadCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        /// <summary>
        /// The <see cref="Status" /> property's name.
        /// </summary>
        public const string StatusPropertyName = "Status";
        private string _status = "Ready";
        /// <summary>
        /// Sets and gets the Status property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status == value)
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
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _pluginService = SimpleIoc.Default.GetInstance<PluginService>();
            _workerService = SimpleIoc.Default.GetInstance<WorkerService>();

            DragEnterCommand = new RelayCommand<DragEventArgs>(OnDragEnter);
            DragLeaveCommand = new RelayCommand<DragEventArgs>(OnDragLeave);
            DropCommand = new RelayCommand<DragEventArgs>(OnDrop);
            LoadCommand = new RelayCommand(OnLoad);
            SaveCommand = new RelayCommand(OnSave);

            //_dataService.GetIncomingChanges( "D:\\CODE\\Hess",
            //    (changesets, error) =>
            //    {
            //        if (error != null)
            //        {
            //            // Report error here
            //            return;
            //        }

            //        if (null != changesets && changesets.Count > 0)
            //            Changesets = changesets;
            //        else
            //            Changesets = new List<Changeset>();
            //    });

            Initialize();
        }

        private void Initialize()
        {
            _dataService.GetRepositories(OnGetRepositoriesCompleted);
        }

        private void OnGetRepositoriesCompleted(IList<ExtendedRepository> repositories, Exception exception)
        {
            Repositories = new ObservableCollection<ExtendedRepository>(repositories);
        }


        private void OnLoad()
        {
            Repositories = new ObservableCollection<ExtendedRepository>(_dataService.ReadRepositories());
        }
        private void OnSave()
        {
            _dataService.UpdateRepositories(Repositories);
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