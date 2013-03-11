using System.Reactive.Linq;
using FocalPoint.SDK;
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
        private int _Duration = 25;
        public int Duration
        {
            get { return _Duration; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        private bool _Running = false;
        public bool Running
        {
            get { return _Running; }
            protected set { this.RaiseAndSetIfChanged(value); }
        }

        private int _PercentComplete = 0;
        public int PercentComplete
        {
            get { return _PercentComplete; }
            protected set { this.RaiseAndSetIfChanged(value); }
        }

        public ReactiveCommand StartSession { get; set; }
        protected ReactiveCommand UpdateProgress { get; set; }
        public ReactiveCommand EndSession { get; set; }

        public SessionViewModel()
        {
            var l = new Lync2013Plugin.LyncStatusUpdater();

            IDisposable cancelToken = null;

            var canStartSession = this.WhenAny(vm => vm.Running, running => !running.Value);
            StartSession = new ReactiveCommand(canStartSession);
            StartSession.Subscribe(_ =>
                {
                    var session = new Session
                        {
                            Duration = _Duration,
                            EndTime = DateTime.UtcNow.AddMinutes(_Duration)
                        };

                    Running = true;
                    l.Start(session);

                    cancelToken = Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(__ =>
                         {
                             UpdateProgress.Execute(session); 

                             if (session.PercentComplete == 100)
                             {
                                 EndSession.Execute(null);
                             }
                         });
                });

            UpdateProgress = new ReactiveCommand();
            UpdateProgress.Subscribe(obj =>
                {
                    var session = (Session) obj;

                    PercentComplete = session.PercentComplete;

                    l.Update(session);
                });

            var canEndSession = this.WhenAny(vm => vm.Running, running => running.Value);
            EndSession = new ReactiveCommand(canEndSession);
            EndSession.Subscribe(_ =>
                {
                    Running = false;
                    PercentComplete = 0;

                    if (cancelToken != null)
                    {
                        cancelToken.Dispose();
                        cancelToken = null;
                    }

                    l.Stop();
                });
        }
    }
}
