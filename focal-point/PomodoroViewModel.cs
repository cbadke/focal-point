using ReactiveUI;
using ReactiveUI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocalPoint
{
    public class PomodoroViewModel : ReactiveObject
    {
        private bool _Running;
        public bool Running
        {
            get { return _Running; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        public ReactiveCommand StartSession { get; protected set; }

        public PomodoroViewModel()
        {
            var canStartSession = this.WhenAny(vm => vm.Running, running => !running.Value);
            StartSession = new ReactiveCommand(canStartSession);
            StartSession.Subscribe(_ => this.Running = !this.Running);
        }
    }
}
