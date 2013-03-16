using System.Collections.Generic;
using System.Linq;
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
            _plugins.Add(new Plugin());
        }

        public IEnumerable<ISessionWatcher> Subscribers
        {
            get { return _plugins.Select(p => p.SessionWatcher); }
        }
    }
}
