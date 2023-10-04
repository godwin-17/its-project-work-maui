using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Text.RegularExpressions;

namespace Project_Work_MAUI.ViewModels
{
    
    public partial class MainViewModel : ObservableObject
    {
        IDispatcher dispatcher;
        public MainViewModel(IDispatcher dispatcher) 
        { 
            this.dispatcher = dispatcher;
            email = "filippo@tescaro.com";
            password = "Filippotescaro1";
        }

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        bool loading;

        private bool timerExpired;
        private bool pageDisappearing = false;

        public void StartTimer()
        {
            timerExpired = false;
            bool showAlert = true;


            dispatcher.StartTimer(TimeSpan.FromSeconds(300), () =>
            {
                if (pageDisappearing)
                {
                    return false;
                }

                timerExpired = true;
                if (showAlert)
                {
                    dispatcher.Dispatch(async () =>
                    {
                        showAlert = false;
                        await Application.Current.MainPage.DisplayAlert("Timeout", "Il tempo per fare il login è scaduto. Perfavore riprova.", "OK");
                        ClearInputs();
                        StartTimer();
                    });
                }
                return false;
            });
        }

        public void StopTimer()
        {
            pageDisappearing = true;
        }

        public void ResetPageDisappearing()
        {
            pageDisappearing = false;
        }

        RootUser user = new RootUser();

        [RelayCommand]
        async Task TapRegister()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        [RelayCommand]
        async Task Login()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Tutti i campi sono richiesti.", "OK");
                return;
            }

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(Email, emailPattern))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Inserisci un indirizzo email valido.", "OK");
                return;
            }
            
            await MakeLogin();
        }

        private async Task MakeLogin()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiURL = "https://bbankapidaniel.azurewebsites.net/api/login";

                LoginData loginData = new LoginData
                {
                    username = Email,
                    password = Password,
                };

                try
                {
                    Loading = true;
                    string jsonData = JsonConvert.SerializeObject(loginData);

                    StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiURL, content);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var responseObject = System.Text.Json.JsonSerializer.Deserialize<RootUser>(responseContent);

                        user = responseObject;

                        await SecureStorage.Default.SetAsync("oauth_token", user.token);
                        var toast = Toast.Make("Logged In", ToastDuration.Short, 12);
                        Password = "";
                        await toast.Show();
                        //TokenProvider.Token = user.token;
                        //await Application.Current.MainPage.DisplayAlert("Successo", "Login avvenuto con successo.", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "Credenziali errate.", "OK");
                        Loading = false;
                        return;
                    }
                    Loading = false;
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", "C'è stato un errore: " + ex.Message, "OK");
                    return;
                }
            }

            //await Shell.Current.GoToAsync("//HomePage", new Dictionary<string, object>
            //{
            //    ["User"] = user,
            //});
            await Shell.Current.GoToAsync("//HomePage");
        }

        private void ClearInputs()
        {
            Email = string.Empty;
            Password = string.Empty;
            timerExpired = false;
        }
    }
}
