using System.Diagnostics;
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
    public class Session : ISession
    {
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }

        public double PercentComplete
        {
            get
            {
                var currentTime = DateTime.UtcNow;
                if (currentTime > EndTime) return 100.0d;

                var sessionLength = new TimeSpan(0, Duration, 0);
                var timeRemaining = (EndTime - currentTime).TotalMilliseconds;

                return 100.0d * (sessionLength.TotalMilliseconds - timeRemaining) / sessionLength.TotalMilliseconds;
            }
        }
    }

    public class Error
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }

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

        private double _PercentComplete = 0;
        public double PercentComplete
        {
            get { return _PercentComplete; }
            protected set { this.RaiseAndSetIfChanged(value); }
        }

        private Error _ErrorMessage = null;
        public Error ErrorMessage
        {
            get { return _ErrorMessage; }
            protected set { this.RaiseAndSetIfChanged(value); }
        }

        public ReactiveCommand StartSession { get; set; }
        protected ReactiveCommand UpdateProgress { get; set; }
        public ReactiveCommand EndSession { get; set; }

        public SessionViewModel(IEnumerable<ISessionWatcher> plugins)
        {
            Debug.Assert(plugins != null, "plugins != null");

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

                    NotifyPluginsOfStart(plugins, session);

                    cancelToken = Observable.Interval(TimeSpan.FromMilliseconds(1000)).Subscribe(__ =>
                         {
                             UpdateProgress.Execute(session); 

                             if (session.PercentComplete >= 100.0d)
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

                    NotifyPluginsOfUpdate(plugins, session);
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

                    NotifyPluginsOfStop(plugins);
                });
        }

        private void NotifyPluginsOfStart(IEnumerable<ISessionWatcher> plugins, ISession session)
        {
            foreach (var p in plugins)
            {
                try
                {
                    p.Start(session);
                }
                catch (PluginException ex)
                {
                    this.ErrorMessage = new Error { Title = p.Name, Message = ex.Message };
                }
                catch (NotImplementedException) { }
                catch (Exception)
                {
                    this.ErrorMessage = new Error { Title = p.Name, Message = "An unknown error has occurred with plugin." };
                }
            }
        }

        private void NotifyPluginsOfStop(IEnumerable<ISessionWatcher> plugins)
        {
            foreach (var p in plugins)
            {
                try
                {
                    p.Stop();
                }
                catch (PluginException ex)
                {
                    this.ErrorMessage = new Error { Title = p.Name, Message = ex.Message };
                }
                catch (NotImplementedException) { }
                catch (Exception)
                {
                    this.ErrorMessage = new Error { Title = p.Name, Message = "An unknown error has occurred with plugin." };
                }
            }
        }

        private void NotifyPluginsOfUpdate(IEnumerable<ISessionWatcher> plugins, ISession session)
        {
            foreach (var p in plugins)
            {
                try
                {
                    p.Update(session);
                }
                catch (PluginException ex)
                {
                    this.ErrorMessage = new Error { Title = p.Name, Message = ex.Message };
                }
                catch (NotImplementedException) { }
                catch (Exception)
                {
                    this.ErrorMessage = new Error { Title = p.Name, Message = "An unknown error has occurred with plugin." };
                }
            }
        }
    }
}
