using System;

namespace FocalPoint.SDK
{
    public interface ISessionWatcher
    {
        void Start(Session session);
        void Update(Session session);
        void Stop();
    }
}
