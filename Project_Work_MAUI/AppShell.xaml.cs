using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI
{
    public partial class AppShell : Shell
    {
        public LogoutViewModel LogoutViewModel { get; } = new LogoutViewModel();
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
            Routing.RegisterRoute(nameof(FiltriPage), typeof(FiltriPage));
            Routing.RegisterRoute(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
            BindingContext = this;
        }
    }
}