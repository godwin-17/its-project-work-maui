using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Net.Http.Headers;

namespace Project_Work_MAUI.ViewModels
{
    [QueryProperty("User", "User")]
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        User user;

        //[ObservableProperty]
        //string token;

        [ObservableProperty]
        RootTransaction transaction;

        private decimal balance;
        public decimal Balance
        {
            get { return balance; }
            set { SetProperty(ref balance, value); }
        }

        [RelayCommand]
        async Task TapLogout()
        {
            bool success = SecureStorage.Default.Remove("oauth_token");
            if(success)
            {
                //await Application.Current.MainPage.DisplayAlert("Logged out", "LogOut effettuato con successo", "OK");
                var toast = Toast.Make("Logged out", ToastDuration.Short, 12);
                await toast.Show();
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("LogOut Error", "Errore durante il logout, chiudere l'applicazione e riporvare", "OK");
            }
        }

        public HomeViewModel()
        {
            //Token = TokenProvider.Token;
        }

        public async Task LoadData()
        {
           await LoadBalanceData();
           await LoadTranscationsData();
           await LoadProfile();
        }

        private async Task LoadProfile()
        {
            string oauthToken = await SecureStorage.Default.GetAsync("oauth_token");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://bbankapidaniel.azurewebsites.net/api/users/me";
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<User>(jsonResponse);
                        User = result;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "Errore con il caricamento del profilo", "OK");
                        return;
                    }
                }
            }
            catch (Exception ex) 
            {
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
                return;
            }
        }

        private async Task LoadTranscationsData()
        {
            string oauthToken = await SecureStorage.Default.GetAsync("oauth_token");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Di default restituisce 5 movimenti
                    string apiUrl = "https://bbankapidaniel.azurewebsites.net/api/transaction/research";

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<RootTransaction>(jsonResponse);

                        Transaction = result;
                        
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "Errore con l'autorizzazione", "OK");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
                return;
            }
        }

        public class UserBalanceResponse
        {
            public List<Account> accout { get; set; }
        }

        public class Account
        {
            public decimal balance { get; set; }
        }

        private async Task LoadBalanceData()
        {
            string oauthToken = await SecureStorage.Default.GetAsync("oauth_token");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://bbankapidaniel.azurewebsites.net/api/users/balance";

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<UserBalanceResponse>(jsonResponse);

                        Balance = result.accout[0].balance;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "Errore con l'autorizzazione", "OK");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Errore!", ex.Message, "OK");
                return;
            }

        }
    }
}
