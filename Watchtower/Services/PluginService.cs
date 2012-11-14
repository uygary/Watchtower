﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

            var iType = typeof(IPlugin);
            foreach (string fileName in pluginFileNames)
            {
                Assembly assembly = Assembly.LoadFile(fileName);

                var plugins = assembly.GetTypes().Where(p => iType.IsAssignableFrom(p) && p.IsClass);
                if (null != plugins && plugins.Count() > 0)
                {
                    var pType = plugins.First();
                    IPlugin plugin = (IPlugin)Activator.CreateInstance(pType);

                    if (!Plugins.ContainsKey(plugin.RepositoryType))
                        Plugins.Add(plugin.RepositoryType, plugin);

                }

                //Type type = assembly.GetType("Watchtower.Plugin");
                //if (null != type)
                //{
                //    IPlugin plugin = (IPlugin)Activator.CreateInstance(type);

                //    if (!Plugins.ContainsKey(plugin.RepositoryType))
                //        Plugins.Add(plugin.RepositoryType, plugin);
                //}
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
