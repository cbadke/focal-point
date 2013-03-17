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
        public IPlugin Create()
        {
            return new Plugin();
        }
    }
}
