using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Watchtower.Core;
using Watchtower.Model;

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
            string pluginsFolder = @".\Plugins\";
            string fullPath = Path.GetFullPath(pluginsFolder);
            string[] pluginFileNames = Directory.GetFiles(fullPath, "*.dll");

            foreach (string fileName in pluginFileNames)
            {
                Assembly assembly = Assembly.LoadFile(fileName);
                Type type = assembly.GetType("Watchtower.Plugin");
                if (null != type)
                {
                    IPlugin plugin = (IPlugin)Activator.CreateInstance(type);

                    if (!Plugins.ContainsKey(plugin.RepositoryType))
                        Plugins.Add(plugin.RepositoryType, plugin);
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
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
