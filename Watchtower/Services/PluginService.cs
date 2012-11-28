using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Watchtower.Core;
using Watchtower.Models;

namespace Watchtower.Services
{
    public class PluginService
    {
        private Dictionary<string, IPlugin> _plugins;
        public Dictionary<string, IPlugin> Plugins
        {
            get { return _plugins; }
            private set { _plugins = value; }
        }

        public PluginService()
        {
            Plugins = new Dictionary<string, IPlugin>();

            Initialize();
        }

        private void Initialize()
        {
            AppDomainSetup appDomainSetup = AppDomain.CurrentDomain.SetupInformation;
            string privateBinPath = appDomainSetup.PrivateBinPath;
            string[] pluginDirectoryFullPaths = privateBinPath.Split(';');

            foreach (string pluginDirectoryFullPath in pluginDirectoryFullPaths)
            {
                RegisterPlugins(pluginDirectoryFullPath);
            }
        }

        private void RegisterPlugins(string directoryFullPath)
        {
            string[] pluginFileNames = Directory.GetFiles(directoryFullPath, "*.dll");

            var iType = typeof(IPlugin);
            foreach (string fileName in pluginFileNames)
            {
                Assembly assembly = Assembly.LoadFile(fileName);

                //TODO: Find out why this would throw on "some" environments.
                try
                {
                    var plugins = assembly.GetTypes().Where(p => iType.IsAssignableFrom(p) && p.IsClass);
                    if (null != plugins && plugins.Count() > 0)
                    {
                        var pType = plugins.First();
                        IPlugin plugin = (IPlugin)Activator.CreateInstance(pType);

                        if (!Plugins.ContainsKey(plugin.RepositoryType))
                            Plugins.Add(plugin.RepositoryType, plugin);
                    }
                }
                catch (Exception ex)
                {
                }

            }
        }

        public bool SetRepositoryType(ref ExtendedRepository repository)
        {
            bool result = false;

            foreach (IPlugin plugin in Plugins.Values)
            {
                if (plugin.VerifyRepository(repository.Path))
                {
                    repository.Type = plugin.RepositoryType;
                    repository.PluginIcon = plugin.PluginIcon;
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
