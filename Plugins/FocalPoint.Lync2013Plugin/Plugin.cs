using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocalPoint.SDK;

namespace FocalPoint.Lync2013Plugin
{
    public class Plugin : IPlugin
    {
        private ISessionWatcher _watcher = null;
        public ISessionWatcher SessionWatcher 
        { 
            get
            {
                if (_watcher == null)
                {
                    _watcher = new LyncStatusUpdater();
                }

                return _watcher;
            } 
        }
    }
}
