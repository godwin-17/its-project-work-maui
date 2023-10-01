﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

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
        int numberOfTransactions;

        [ObservableProperty]
        DateTime startingDate;

        [ObservableProperty]
        DateTime endingDate;

        [ObservableProperty]
        TimePicker startingTimer;

        [ObservableProperty]
        TimePicker endingTimer;

        [ObservableProperty]
        bool exportEnabled;

        private decimal balance;
        public MoviementiViewModel()
        {
            startingDate = DateTime.Today;
            endingDate = DateTime.Today;
            startingTimer = new TimePicker();
            endingTimer = new TimePicker();
        }
        public decimal Balance
        {
            get { return balance; }
            set { SetProperty(ref balance, value); }
        }
        public class UserBalanceResponse
        {
            public List<Account> accout { get; set; }
        }
        public class Account
        {
            public decimal balance { get; set; }
        }


        #region filtri ricerca
        [RelayCommand]
        public async Task ExecuteFiltering()
        {
            string apiUrl="";
            if (NumberOfTransactions < 1)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Numero di movimenti cercati non valido", "OK");
                return;
            } else if (SelectedCategory == null && DateTime.Equals(StartingDate, EndingDate))
            {
                apiUrl = $"https://bbankapidaniel.azurewebsites.net/api/transaction/research?num={NumberOfTransactions}";
                await RicercaMovimenti(apiUrl);
            }
            if (SelectedCategory != null)
            {
                string categoryId = SelectedCategory.Id;
                apiUrl = $"https://bbankapidaniel.azurewebsites.net/api/transaction/research?num={NumberOfTransactions}&categoryId={categoryId}";
                await RicercaMovimenti(apiUrl);
            }
            if (StartingDate > EndingDate)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "La data di inizio deve essere precedente alla data di fine.", "OK");
                return;
            }
            else if (DateTime.Equals(StartingDate, EndingDate))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "La data di inizio e di fine non possono essere uguali.", "OK");
                return;
            }
            else
            {
                TimeSpan startingTimeSpan = StartingTimer.Time;
                TimeSpan endingTimeSpan = EndingTimer.Time;
                DateTime combinedStartDate = StartingDate + startingTimeSpan;
                DateTime combinedEndDate = EndingDate + endingTimeSpan;
                string isoStartingDate = combinedStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                string isoEndingDate = combinedEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                apiUrl = $"https://bbankapidaniel.azurewebsites.net/api/transaction/research?num={NumberOfTransactions}&startDate={isoStartingDate}&endDate={isoEndingDate}";
                await RicercaMovimenti(apiUrl);
            }
        }
        #endregion

        #region ricerca movimenti
        private async Task RicercaMovimenti(string apiUrl)
        {
            string oauthToken = await SecureStorage.Default.GetAsync("oauth_token");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Transaction = null;
                    Visibility = true;
                    Loading = true;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<RootTransaction>(jsonResponse);
                        Transaction = result;
                        Loading = false;
                        Visibility = false;
                        NumberOfTransactions = 0;
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
        #endregion

        public async Task LoadData()
        {
            await LoadBalanceData();
            await LoadTranscationsData();
            Visibility = false;
            await LoadCategoriesData();
        }

        #region carica categorie
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
        #endregion

        #region carica movimenti iniziali
        private async Task LoadTranscationsData()
        {
            string oauthToken = await SecureStorage.Default.GetAsync("oauth_token");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Visibility = true;
                    Loading = true;
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
        #endregion

        #region carica balance
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
        #endregion
    }
}
