using System;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace Watchtower
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();

            //AppDomain domain = AppDomain.CurrentDomain;
            //domain.SetupInformation.PrivateBinPath = @"\\Plugins";

            //AppDomainSetup domain = new AppDomainSetup();
            //domain.PrivateBinPath = @"Plugins";

            //FIXME: AppendPrivatePath is deprecated.
            string pluginsFolder = @".\Plugins\";
            string pluginsFolderFullPath = Path.GetFullPath(pluginsFolder);
            if (!Directory.Exists(pluginsFolderFullPath))
                Directory.CreateDirectory(pluginsFolderFullPath);

            AppDomain.CurrentDomain.AppendPrivatePath(pluginsFolderFullPath);

            string[] pluginSubdirectories = Directory.GetDirectories(pluginsFolderFullPath);
            foreach (string pluginSubdirectory in pluginSubdirectories)
            {
                AppDomain.CurrentDomain.AppendPrivatePath(pluginSubdirectory);
            }
            
            Bootstrapper bootstrapper = new Bootstrapper();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
