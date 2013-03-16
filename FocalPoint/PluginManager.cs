using System.Collections.Generic;
using FocalPoint.Lync2013Plugin;
using FocalPoint.SDK;

namespace FocalPoint
{
    class PluginManager
    {
        public IEnumerable<ISessionWatcher> Subscribers
        {
            get { return new List<ISessionWatcher> {new LyncStatusUpdater()}; }
        }
    }
}
