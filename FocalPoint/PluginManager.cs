using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using FocalPoint.SDK;

namespace FocalPoint
{
    class PluginManager
    {
        private List<IPlugin> _plugins;
 
        public PluginManager()
        {
            _plugins = new List<IPlugin>();

            var pluginDirectory = GetPluginDirectory();

            Directory.GetFiles(pluginDirectory, "*.dll").ToList().ForEach(RegisterPlugin);
        }

        private static string GetPluginDirectory()
        {
            var pluginDirectory = ConfigurationManager.AppSettings["pluginDirectory"];

            if (!Path.IsPathRooted(pluginDirectory))
            {
                var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                assemblyPath = new Uri(assemblyPath).LocalPath;

                pluginDirectory = Path.Combine(assemblyPath, pluginDirectory);
            }

            return pluginDirectory;
        }

        public IEnumerable<ISessionWatcher> Subscribers
        {
            get { return _plugins.Select(p => p.SessionWatcher); }
        }

        private void RegisterPlugin(string filePath)
        {
            try
            {
                var asm = Assembly.LoadFrom(filePath);
                var factoryType = asm.GetType(asm.GetName().Name + ".Factory");

                if (factoryType == null) return;

                var cInfo = factoryType.GetConstructor(new Type[] { });

                if (cInfo == null) return;

                var pluginFactory = cInfo.Invoke(null) as FocalPoint.SDK.IFactory;

                if (pluginFactory != null)
                {
                    _plugins.Add(pluginFactory.Create());
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
