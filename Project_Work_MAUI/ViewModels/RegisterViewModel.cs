using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Text.RegularExpressions;

namespace Project_Work_MAUI.ViewModels
{
    public partial class RegisterViewModel: ObservableObject
    {
        [ObservableProperty]
        string name;

        [ObservableProperty]
        string surname;

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        string confirmPassword;


        [RelayCommand]
        async Task TapLogin()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        [RelayCommand]
        async Task Register()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Surname) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
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

            string passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=]).{8,}$";
            if (!Regex.IsMatch(Password, passwordPattern))
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "La password deve contenere almeno 8 caratteri, almeno una maiuscola, almeno un numero e almeno un simbolo (@#$%^&+=).", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", "Le password non corrispondono.", "OK");
                return;
            }

            await MakeRegistration();
        }

        private async Task MakeRegistration()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiURL = "https://bbankapidaniel.azurewebsites.net/api/register";

                RegisterData registerData = new RegisterData
                {
                    firstName = Name,
                    lastName = Surname,
                    username = Email,
                    password = Password,
                    confermaPassword = ConfirmPassword,
                };

                try
                {
                    string jsonData = JsonConvert.SerializeObject(registerData);

                    StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiURL, content);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var toast = Toast.Make("Registrazione avvenuta con successo", ToastDuration.Short, 12);
                        await toast.Show();
                        await Shell.Current.GoToAsync("//MainPage");
                        return;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "Email già esistente.", "OK");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "C'e stato un errore: " + ex.Message, "OK");
                    return;
                }
            }
        }
    }
}
