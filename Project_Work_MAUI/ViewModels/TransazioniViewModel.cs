using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Net.Http.Headers;

namespace Project_Work_MAUI.ViewModels
{
    public partial class TransazioniViewModel: ObservableObject
    {
        [ObservableProperty]
        List<TransactionCategory> transactionCategories = new List<TransactionCategory>
        {
            new TransactionCategory
            {
                Category = "Pagamento Utenze",
                Typology = "Uscita",
                Id = "650d865bff8d876d587ff468"
            },
            new TransactionCategory
            {
                Category = "Versamento Bancomat",
                Typology = "Entrata",
                Id = "650d86baff8d876d587ff46c"
            },
            new TransactionCategory
            {
                Category = "Prelievo Contanti",
                Typology = "Uscita",
                Id = "6513efa173766e8ac7e52570"
            }
        };

        [ObservableProperty]
        float amount;

        [ObservableProperty]
        string reason;

        [ObservableProperty]
        TransactionCategory selectedCategory;

        [RelayCommand]
        async Task Transaction()
        {
            if(string.IsNullOrEmpty(Reason))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Inserire una causale", "OK");
                return;
            }

            if (Amount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Inserire un importo positivo", "OK");
                return;
            }


            if (SelectedCategory == null)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Selezionare una categoria", "OK");
                return;
            }

            await MakeTransaction();
        }

        private async Task MakeTransaction()
        {
            string oauthToken = await SecureStorage.Default.GetAsync("oauth_token");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://bbankapidaniel.azurewebsites.net/api/transaction";


                    TransactionRequest transactionData = new TransactionRequest
                    {
                        amount = Amount,
                        categoryid = SelectedCategory.Id,
                        description = Reason
                    };


                    string jsonData = JsonConvert.SerializeObject(transactionData);

                    StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthToken);

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        //await Application.Current.MainPage.DisplayAlert("Successo", "Transazione avvenuta con successo.", "OK");
                        var toast = Toast.Make(SelectedCategory.Category+" avvenuto con successo", ToastDuration.Short, 12);
                        await toast.Show();
                        await Shell.Current.GoToAsync("//HomePage");
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

