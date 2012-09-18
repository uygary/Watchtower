using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

using Watchtower.Services;
using Watchtower.View;
using Watchtower.ViewModel;

namespace Watchtower
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            RegisterServices();
            InstantiateServices();
            RegisterViewModels();

            InitializeNotificationWindow();
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
        }
        private void InstantiateServices()
        {
            //Instantiate WorkerService
            SimpleIoc.Default.GetInstance<WorkerService>();

            //Instantiate PluginService
            SimpleIoc.Default.GetInstance<PluginService>();
        }
        private void RegisterViewModels()
        {
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<NotificationViewModel>();
        }

        private void InitializeNotificationWindow()
        {
            SimpleIoc.Default.Register<NotificationWindow>();
        }
    }
}
