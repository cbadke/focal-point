using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FocalPoint.Lync2013Plugin;
using FocalPoint.SDK;

namespace FocalPoint
{
    class PluginManager
    {
        private List<IPlugin> _plugins;
 
        public PluginManager()
        {
            _plugins = new List<IPlugin>();

            var asm = Assembly.LoadFrom("FocalPoint.Lync2013Plugin.dll");
            var factoryType = asm.GetType(asm.GetName().Name + ".Factory");
            var cInfo = factoryType.GetConstructor(new Type[] { });

            if (cInfo != null)
            {
                var pluginFactory = cInfo.Invoke(null) as FocalPoint.SDK.IFactory;

                if (pluginFactory != null)
                {
                    _plugins.Add(pluginFactory.Create());
                }
            }
        }

        public IEnumerable<ISessionWatcher> Subscribers
        {
            get { return _plugins.Select(p => p.SessionWatcher); }
        }
    }
}
