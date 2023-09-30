using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Net.Http.Headers;

namespace Project_Work_MAUI.ViewModels
{
    public partial class MoviementiViewModel: ObservableObject
    {
        [ObservableProperty]
        RootTransaction transaction;

        [ObservableProperty]
        bool loading;

        [ObservableProperty]
        bool visibility= false;

        [ObservableProperty]
        List<TransactionCategory> transactionCategories = new List<TransactionCategory>();

        [ObservableProperty]
        TransactionCategory selectedCategory;

        [ObservableProperty]
        float number;

        private decimal balance;
        public decimal Balance
        {
            get { return balance; }
            set { SetProperty(ref balance, value); }
        }
        public async Task LoadData()
        {
            await LoadBalanceData();
            await LoadTranscationsData();
            Visibility = false;
            await LoadCategoriesData();
        }
        private async Task LoadCategoriesData()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://bbankapidaniel.azurewebsites.net/api/transaction-type/";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<List<TransactionCategory>>(jsonResponse);
                        TransactionCategories = result;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "Errore nella richiesta dei movimenti", "OK");
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
                    Visibility = true;
                    Loading = true;
                    //logica da implementare .....................................................................
                    string apiUrl = "https://bbankapidaniel.azurewebsites.net/api/transaction/research?num=50";

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<RootTransaction>(jsonResponse);

                        Transaction = result;
                        Loading = false;
                        Visibility = false;

                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "Errore nella richiesta dei movimenti", "OK");
                        Loading = false;
                        Visibility = false;
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
