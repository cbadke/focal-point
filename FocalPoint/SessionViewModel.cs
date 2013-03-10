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
        private int _Duration = 3;
        public int Duration
        {
            get { return _Duration; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        private bool _Running = false;
        private bool Running
        {
            get { return _Running; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        private int _PercentComplete = 0;
        public int PercentComplete
        {
            get { return _PercentComplete; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        public ReactiveAsyncCommand StartSession { get; protected set; }
        protected ReactiveCommand UpdateProgress { get; set; }

        public SessionViewModel()
        {
            var canStartSession = this.WhenAny(vm => vm.Running, running => !running.Value);
            StartSession = new ReactiveAsyncCommand(canStartSession);
            StartSession.RegisterAsyncAction(RunSession);

            UpdateProgress = new ReactiveCommand();
            UpdateProgress.Subscribe(percent =>
                {
                    PercentComplete = (int)percent;
                });

        }

        private void RunSession(object _)
        {
            var l = new Lync2013Plugin.LyncStatusUpdater();


            var sessionLength = new TimeSpan(0, _Duration, 0);
            var sessionEndTime = DateTime.UtcNow.AddMinutes(_Duration);

            Running = true;
            l.StartSession(sessionEndTime);

            while (sessionEndTime > DateTime.UtcNow)
            {
                var timeRemaining = (sessionEndTime - DateTime.UtcNow).TotalMilliseconds;

                var percentComplete = (int)(100*(sessionLength.TotalMilliseconds - timeRemaining)/sessionLength.TotalMilliseconds);
                UpdateProgress.Execute(percentComplete);

                l.UpdateSession(sessionEndTime);

                System.Threading.Thread.Sleep(1000);
            }

            UpdateProgress.Execute(0);
            l.StopSession();
            Running = false;
        }
    }
}
