using ClosedXML.Excel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Transactions;
using Transaction = Project_Work_MAUI.Models.Transaction;

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
                return;
            }
            if (SelectedCategory != null)
            {
                string categoryId = SelectedCategory.Id;
                apiUrl = $"https://bbankapidaniel.azurewebsites.net/api/transaction/research?num={NumberOfTransactions}&categoryId={categoryId}";
                await RicercaMovimenti(apiUrl);
                return;
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
                return;
            }
        }
        #endregion

        #region ricerca movimenti
        private async Task RicercaMovimenti(string apiUrl)
        {
            ExportEnabled = false;
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
                        ExportEnabled = true;
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
                        ExportEnabled = true;
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

        [RelayCommand]
        public async Task ExportFileExcel()
        {
            PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageRead>();
            PermissionStatus status2 = await Permissions.RequestAsync<Permissions.StorageWrite>();
            try
            {
                if (status != null && status2 != null)
                {
                    var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Export Transactions");
                    worksheet.Cell("A1").Value = "Date";
                    worksheet.Cell("B1").Value = "Amount";
                    worksheet.Cell("C1").Value = "Balance";
                    worksheet.Cell("D1").Value = "Category";
                    worksheet.Cell("E1").Value = "Typology";
                    worksheet.Cell("F1").Value = "CategoryId";
                    worksheet.Cell("G1").Value = "Description";
                    int row = 2;
                    foreach (var transaction in Transaction.transactions)
                    {
                        worksheet.Cell("A" + row).Value = transaction.date;
                        worksheet.Cell("B" + row).Value = transaction.amount;
                        worksheet.Cell("C" + row).Value = transaction.balance;
                        worksheet.Cell("D" + row).Value = transaction.categoryid.category;
                        worksheet.Cell("E" + row).Value = transaction.categoryid.typology;
                        worksheet.Cell("F" + row).Value = transaction.categoryid.id;
                        worksheet.Cell("G" + row).Value = transaction.description;
                        row++;
                    }
                    var basePath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads, "BBank");
                    string baseFileName = "transactions.xlsx";
                    string path = Path.Combine(basePath, baseFileName);

                    int index = 1;
                    while (File.Exists(path))
                    {
                        string newFileName = Path.GetFileNameWithoutExtension(baseFileName)
                                           + "_" + index
                                           + Path.GetExtension(baseFileName);
                        path = Path.Combine(basePath, newFileName);
                        index++;
                    }

                    workbook.SaveAs(path);
                    var toast = Toast.Make("File saved in "+ path, ToastDuration.Short, 12);
                    await toast.Show();
                }
            }catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", ex.Message, "OK");
                return;
            }
        }

        [RelayCommand]
        public async Task ExportFileCSV()
        {
            List<Transaction> records= Transaction.transactions;
            var basePath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads, "BBank");
            string baseFileName = "transactions.csv";
            string path = Path.Combine(basePath, baseFileName);
            int index = 1;
            while (File.Exists(path))
            {
                string newFileName = Path.GetFileNameWithoutExtension(baseFileName)
                                   + "_" + index
                                   + Path.GetExtension(baseFileName);
                path = Path.Combine(basePath, newFileName);
                index++;
            }
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
            var toast = Toast.Make("File saved in " + path, ToastDuration.Short, 12);
            await toast.Show();
        }
    }
}
