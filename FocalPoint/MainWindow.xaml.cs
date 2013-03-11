using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public partial class MainWindow : Window, IViewFor<SessionViewModel>
    {
        public MainWindow()
        {
            ViewModel = new SessionViewModel();
            InitializeComponent();

            this.BindCommand(ViewModel, vm => vm.StartSession);
            this.OneWayBind(ViewModel, vm => vm.Running, form => form.StartSession.Visibility, running => running ? Visibility.Hidden : Visibility.Visible);

            this.BindCommand(ViewModel, vm => vm.EndSession);
            this.OneWayBind(ViewModel, vm => vm.Running, form => form.EndSession.Visibility, running => running ? Visibility.Visible : Visibility.Hidden);

            this.Bind(ViewModel, vm => vm.PercentComplete);
            this.Bind(ViewModel, vm => vm.Duration, form => form.Duration.Value);
            this.Bind(ViewModel, vm => vm.Running, form => form.Duration.IsReadOnly);

            InitializeComponent();

            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
            ni.Visible = true;
            ni.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }

        public SessionViewModel ViewModel
        {
            get { return (SessionViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SessionViewModel),typeof(MainWindow), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SessionViewModel)value; }
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
