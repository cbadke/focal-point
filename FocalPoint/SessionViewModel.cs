using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocalPoint
{
    public class SessionViewModel : ReactiveObject
    {
        private bool _Running;
        public bool Running
        {
            get { return _Running; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        private int _Duration = 1;
        public int Duration
        {
            get { return _Duration; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        public ReactiveAsyncCommand StartSession { get; protected set; }

        public SessionViewModel()
        {
            var canStartSession = this.WhenAny(vm => vm.Running, running => !running.Value);
            StartSession = new ReactiveAsyncCommand(canStartSession);
            StartSession.RegisterAsyncAction(RunSession);
        }

        private void RunSession(object _)
        {
            Running = true;
            var endTime = DateTime.UtcNow.AddMinutes(_Duration);

            var l = new Lync2013Plugin.LyncStatusUpdater();
            l.StartSession(endTime);

            var timeRemaining = endTime - DateTime.UtcNow;
            while (timeRemaining.TotalMilliseconds > 0)
            {
                Observable.Interval(TimeSpan.FromMilliseconds(1000));
                timeRemaining = endTime - DateTime.UtcNow;
            }

            l.StopSession();
            Running = false;
        }
    }
}
