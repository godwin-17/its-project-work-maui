using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Net.Http.Headers;

namespace Project_Work_MAUI.ViewModels
{
    [QueryProperty("User", "User")]
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        RootUser user;

        [ObservableProperty]
        string token;

        [ObservableProperty]
        RootTransaction transaction;

        private decimal balance;
        public decimal Balance
        {
            get { return balance; }
            set { SetProperty(ref balance, value); }
        }



        public HomeViewModel()
        {
            Token = TokenProvider.Token;
        }

        public async Task LoadData()
        {
           await LoadBalanceData();
           await LoadTranscationsData();
        }

        private async Task LoadTranscationsData()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Di default restituisce 5 movimenti
                    string apiUrl = "http://192.168.40.45:8080/api/transaction/research";


                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenProvider.Token);

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
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "http://192.168.40.45:8080/api/users/balance";


                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenProvider.Token);

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
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
                return;
            }

        }
    }
}
