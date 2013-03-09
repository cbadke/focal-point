using System;

namespace FocalPoint.SDK
{
    public interface IActivityWatcher
    {
        void ActivityStarted(DateTime completionTime);
        void ActivityStopped();
    }
}
