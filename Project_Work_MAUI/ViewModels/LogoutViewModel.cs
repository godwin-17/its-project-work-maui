using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Windows.Input;

namespace Project_Work_MAUI.ViewModels
{
    public class LogoutViewModel
    {
        public ICommand LogoutCommand { get; }

        public LogoutViewModel()
        {
            LogoutCommand = new Command(OnLogout);
        }
        private async void OnLogout()
        {
            bool success = SecureStorage.Default.Remove("oauth_token");
            if (success)
            {
                var toast = Toast.Make("Logged out", ToastDuration.Short, 12);
                await toast.Show();
                await Shell.Current.GoToAsync("//MainPage");
                Shell.Current.FlyoutIsPresented = false;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("LogOut Error", "Errore durante il logout, chiudere l'applicazione e riporvare", "OK");
            }
        }
    }
}
