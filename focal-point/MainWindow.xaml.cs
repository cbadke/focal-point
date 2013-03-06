using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace FocalPoint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<PomodoroViewModel>
    {
        public MainWindow()
        {
            ViewModel = new PomodoroViewModel();
            InitializeComponent();

            this.BindCommand(ViewModel, vm => vm.StartSession);
            InitializeComponent();
        }

        public PomodoroViewModel ViewModel
        {
            get { return (PomodoroViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(PomodoroViewModel),typeof(MainWindow), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (PomodoroViewModel)value; }
        }

    }
}
