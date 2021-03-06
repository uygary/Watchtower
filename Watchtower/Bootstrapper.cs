﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

using Watchtower.Services;
using Watchtower.ViewModels;
using Watchtower.Views;

namespace Watchtower
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            //TODO: Do we really need this ugly workaround in order to ensure the type initializer is run?
            string workaround = System.IO.Packaging.PackUriHelper.UriSchemePack;

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            RegisterServices();
            RegisterViewModels();
            RegisterViews();

            InstantiateServices();
        }

        private void RegisterServices()
        {
            //Register PluginService
            SimpleIoc.Default.Register<PluginService>();
            
            //Register IDataService implementation
            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            //Register WorkerService
            SimpleIoc.Default.Register<WorkerService>();

            //Register NotificationService
            SimpleIoc.Default.Register<NotificationService>();
        }
        private void InstantiateServices()
        {
            //Instantiate WorkerService
            SimpleIoc.Default.GetInstance<IDataService>();

            //Instantiate WorkerService
            SimpleIoc.Default.GetInstance<WorkerService>();

            //Instantiate PluginService
            SimpleIoc.Default.GetInstance<PluginService>();

            //Instantiate NotificationService
            SimpleIoc.Default.GetInstance<NotificationService>();
        }
        private void RegisterViewModels()
        {
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<NotificationViewModel>();
        }

        private void RegisterViews()
        {
            SimpleIoc.Default.Register<NotificationWindow>();
        }
    }
}
