using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Net.Http.Headers;

namespace Project_Work_MAUI.ViewModels
{
    public partial class RicaricaViewModel: ObservableObject
    {
        [ObservableProperty]
        string telefono;

        [ObservableProperty]
        int amount;

        [ObservableProperty]
        List<TransactionCategory> transactionCategories = new List<TransactionCategory>
        {
            new TransactionCategory
            {
                Category = "Ricarica Telefonica",
                Typology = "Uscita",
                Id = "650d866cff8d876d587ff46a"
            },
        };

        [ObservableProperty]
        TransactionCategory selectedCategory;

        [ObservableProperty]
        string selectedProvider;

        public RicaricaViewModel()
        {
            SelectedCategory = transactionCategories[0];
        }

        [RelayCommand]
        async Task Transaction()
        {
            if (string.IsNullOrEmpty(Telefono))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Inserire un numero di telefono", "OK");
                return;
            }

            if (Amount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Inserire un importo positivo", "OK");
                return;
            }

            if (SelectedProvider == null)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Selezionare un operatore telefonico", "OK");
                return;
            }

            await MakeTransaction();
        }

        public void SetMobileProvider(string provider)
        {
            SelectedProvider = provider;
        }

        public void SetAmount(int amount)
        {
            Amount = amount;
        }

        public string CreateDescription(string telefono, string provider)
        {
            return $"Ricarica telefono al numero {telefono}. Operatore telefonico {provider}";
        }

        private async Task MakeTransaction()
        {
            string oauthToken = await SecureStorage.Default.GetAsync("oauth_token");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://bbankapidaniel.azurewebsites.net/api/transaction";


                    string reason = CreateDescription(Telefono, SelectedProvider);

                    TransactionRequest transactionData = new TransactionRequest
                    {
                        amount = Amount,
                        categoryid = SelectedCategory.Id,
                        description = reason
                    };


                    string jsonData = JsonConvert.SerializeObject(transactionData);

                    StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Successo", "Transazione avvenuta con successo.", "OK");
                        return;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);

                        if (errorResponse != null)
                        {
                            await Application.Current.MainPage.DisplayAlert("Errore", errorResponse.message, "OK");
                            return;
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Errore", "Errore con l'autorizzazione.", "OK");
                            return;
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "Errore con l'autorizzazione.", "OK");
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
