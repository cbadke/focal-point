using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocalPoint.SDK;

namespace FocalPoint.Lync2013Plugin
{
    public class Factory : IFactory
    {
        private IPlugin _plugin = null;
        public IPlugin Plugin
        {
            get
            {
                if (_plugin == null)
                {
                    _plugin = new Plugin();
                }
                return _plugin;
            }
        }
    }
}
