using System;

namespace FocalPoint.SDK
{
    public interface ISessionWatcher
    {
        void Start(ISession session);
        void Update(ISession session);
        void Stop();
    }
}
