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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FocalPoint.Lync2013Plugin;
using FocalPoint.SDK;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace FocalPoint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<SessionViewModel>
    {
        readonly System.Windows.Forms.NotifyIcon taskBarIcon ;

        public MainWindow()
        {
            ViewModel = new SessionViewModel(new List<ISessionWatcher>{new LyncStatusUpdater()});
            InitializeComponent();

            this.BindCommand(ViewModel, vm => vm.StartSession);
            this.OneWayBind(ViewModel, vm => vm.Running, form => form.StartSession.Visibility, running => running ? Visibility.Hidden : Visibility.Visible);

            this.BindCommand(ViewModel, vm => vm.EndSession);
            this.OneWayBind(ViewModel, vm => vm.Running, form => form.EndSession.Visibility, running => running ? Visibility.Visible : Visibility.Hidden);

            this.Bind(ViewModel, vm => vm.PercentComplete);
            this.Bind(ViewModel, vm => vm.Duration, form => form.Duration.Value);
            this.Bind(ViewModel, vm => vm.Running, form => form.Duration.IsReadOnly);

            ViewModel.ObservableForProperty(new [] {"ErrorMessage"}).Subscribe(NotifyErrors);

            InitializeComponent();

            taskBarIcon = new System.Windows.Forms.NotifyIcon
                {
                    Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location),
                    Visible = true,
                    ContextMenu = CreateTaskBarContextMenu()
                };
            taskBarIcon.DoubleClick += Show_Click;
        }

        private void NotifyErrors(IObservedChange<SessionViewModel, object> obj)
        {
            var error = obj.Value as Error;

            if (error != null)
            {
                taskBarIcon.BalloonTipTitle = error.Title;
                taskBarIcon.BalloonTipText = error.Message;
                taskBarIcon.ShowBalloonTip(5000);
            }
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Hide_Click(object sender, EventArgs e)
        {
            this.Hide();

            taskBarIcon.ContextMenu.MenuItems["Show"].Visible = true;
            taskBarIcon.ContextMenu.MenuItems["Hide"].Visible = false;
        }

        private void Show_Click(object sender, EventArgs e)
        {
            this.Show();

            taskBarIcon.ContextMenu.MenuItems["Show"].Visible = false;
            taskBarIcon.ContextMenu.MenuItems["Hide"].Visible = true;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            ViewModel.EndSession.Execute(null);
            taskBarIcon.Visible = false;
            this.Close();
        }

        private System.Windows.Forms.ContextMenu CreateTaskBarContextMenu()
        {
            var context = new System.Windows.Forms.ContextMenu();

            var openMenuItem = new System.Windows.Forms.MenuItem {Index = 0, Text = "Show", Name = "Show"};
            openMenuItem.Click += Show_Click;
            openMenuItem.Visible = false;

            var hideMenuItem = new System.Windows.Forms.MenuItem {Index = 1, Text = "Hide", Name = "Hide"};
            hideMenuItem.Click += Hide_Click;

            var separator = new System.Windows.Forms.MenuItem {Index = 2, Text = "-"};

            var exitMenuItem = new System.Windows.Forms.MenuItem {Index = 3, Text = "Exit", Name = "Exit"};
            exitMenuItem.Click += Exit_Click;

            context.MenuItems.AddRange(new [] {openMenuItem, hideMenuItem, separator,exitMenuItem});

            return context;
        }
    }
}
