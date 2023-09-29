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
        int amount;

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

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenProvider.Token);

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Successo", "Transazione avvenuta con successo.", "OK");
                        return;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "Quantità di denaro insufficiente, riprova.", "OK");
                        return;
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

