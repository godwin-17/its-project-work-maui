using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Net.Http.Headers;

namespace Project_Work_MAUI.ViewModels
{
    public partial class ChangePasswordViewModel: ObservableObject
    {
        [ObservableProperty]
        string currentPassword;

        [ObservableProperty]
        string newPassword;

        [ObservableProperty]
        string confirmNewPassword;

        //ChangePasswordRequest changePassword;

        [RelayCommand]
        async Task TapChangePassword()
        {
            if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmNewPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Riempire tutti i campi", "OK");
                return;
            }

            if (NewPassword != ConfirmNewPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "La nuova password e la conferma non corrispondono", "OK");
                return;
            }

            await ChangePassword();
        }

        private async Task ChangePassword()
        {
            try
            {
                using(HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://bbankapidaniel.azurewebsites.net/api/changePassword";

                    ChangePasswordRequest changePasswordData = new ChangePasswordRequest
                    {
                        oldPassword = CurrentPassword,
                        newPassword = NewPassword
                    };

                    string jsonData = JsonConvert.SerializeObject(changePasswordData);

                    StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenProvider.Token);

                    HttpResponseMessage response = await client.PatchAsync(apiUrl, content);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Successo", "Password cambiata con successo.", "OK");
                        return;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                        await Application.Current.MainPage.DisplayAlert("Errore", errorResponse.message, "OK");
                        return;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "La password corrente è sbagliata.", "OK");
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
            }
        }
    }
}
