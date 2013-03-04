using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace focal_point
{
    public interface IActivityWatcher
    {
        void ActivityStarted(DateTime completionTime);
        void ActivityStopped();
    }
}
