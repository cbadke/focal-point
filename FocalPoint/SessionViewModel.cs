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

        private int _Duration = 25;
        public int Duration
        {
            get { return _Duration; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        public ReactiveCommand StartSession { get; protected set; }

        public SessionViewModel()
        {
            var canStartSession = this.WhenAny(vm => vm.Running, running => !running.Value);
            StartSession = new ReactiveCommand(canStartSession);
            StartSession.Subscribe(_ =>
                {
                    this.Running = true;
                    var endTime = DateTime.UtcNow.AddMinutes(_Duration);

                    var l = new FocalPoint.Lync2013Plugin.LyncStatusUpdater();
                    l.StartSession(endTime);
                });
        }
    }
}
