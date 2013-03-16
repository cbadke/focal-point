using System;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace FocalPoint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IViewFor<SessionViewModel>
    {
        readonly NotifyIcon taskBarIcon ;

        public MainWindow()
        {
            var pluginManager = new PluginManager();
            
            ViewModel = new SessionViewModel(pluginManager.Subscribers);

            InitializeComponent();

            this.BindCommand(ViewModel, vm => vm.StartSession);
            this.OneWayBind(ViewModel, vm => vm.Running, form => form.StartSession.Visibility, running => running ? Visibility.Hidden : Visibility.Visible);

            this.BindCommand(ViewModel, vm => vm.EndSession);
            this.OneWayBind(ViewModel, vm => vm.Running, form => form.EndSession.Visibility, running => running ? Visibility.Visible : Visibility.Hidden);

            this.Bind(ViewModel, vm => vm.PercentComplete);
            this.Bind(ViewModel, vm => vm.Duration, form => form.Duration.Value);
            this.Bind(ViewModel, vm => vm.Running, form => form.Duration.IsReadOnly);

            ViewModel.ObservableForProperty(new [] {"ErrorMessage"}).Subscribe(NotifyErrors);
            ViewModel.ObservableForProperty(new[] {"Running"}).Subscribe(change =>
                {
                    var running = (bool) change.Value;

                    if (!running && Visibility == Visibility.Hidden)
                    {
                        taskBarIcon.ShowBalloonTip(
                            2000,
                            "Pomodoro Complete",
                            "Good work!", 
                            ToolTipIcon.Info);
                    }
                });

            InitializeComponent();

            taskBarIcon = new NotifyIcon
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
                taskBarIcon.ShowBalloonTip(
                    5000,
                    error.Title,
                    error.Message,
                    ToolTipIcon.Error);
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
                DragMove();
        }

        private void Hide_Click(object sender, EventArgs e)
        {
            Hide();

            taskBarIcon.ContextMenu.MenuItems["Show"].Visible = true;
            taskBarIcon.ContextMenu.MenuItems["Hide"].Visible = false;
        }

        private void Show_Click(object sender, EventArgs e)
        {
            Show();

            taskBarIcon.ContextMenu.MenuItems["Show"].Visible = false;
            taskBarIcon.ContextMenu.MenuItems["Hide"].Visible = true;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            ViewModel.EndSession.Execute(null);
            taskBarIcon.Visible = false;
            Close();
        }

        private ContextMenu CreateTaskBarContextMenu()
        {
            var context = new ContextMenu();

            var openMenuItem = new MenuItem {Index = 0, Text = "Show", Name = "Show"};
            openMenuItem.Click += Show_Click;
            openMenuItem.Visible = false;

            var hideMenuItem = new MenuItem {Index = 1, Text = "Hide", Name = "Hide"};
            hideMenuItem.Click += Hide_Click;

            var separator = new MenuItem {Index = 2, Text = "-"};

            var exitMenuItem = new MenuItem {Index = 3, Text = "Exit", Name = "Exit"};
            exitMenuItem.Click += Exit_Click;

            context.MenuItems.AddRange(new [] {openMenuItem, hideMenuItem, separator,exitMenuItem});

            return context;
        }
    }
}
