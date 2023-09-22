using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Text.RegularExpressions;

namespace Project_Work_MAUI;

public partial class RegisterPage : ContentPage
{
    private string NomeTitolare;
    private string CognomeTitolare;
    private string Email;
    private string Password;
    private string ConfermaPassword;

    public RegisterPage()
	{
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    private void Registrazione_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NomeTitoloareEntry.Text) ||
            string.IsNullOrWhiteSpace(CognomeTitolareEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(PasswordEntry.Text) ||
            string.IsNullOrWhiteSpace(ConfermaPasswordEntry.Text))
        {
            DisplayAlert("Errore", "Tutti i campi sono richiesti.", "OK");
            return;
        }

        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (!Regex.IsMatch(EmailEntry.Text, emailPattern))
        {
            DisplayAlert("Errore", "Inserisci un indirizzo email valido.", "OK");
            return;
        }

        string passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[@#$%^&+=]).{8,}$";
        if (!Regex.IsMatch(PasswordEntry.Text, passwordPattern))
        {
            DisplayAlert("Errore", "La password deve contenere almeno 8 caratteri, almeno una maiuscola, almeno un numero e almeno un simbolo (@#$%^&+=).", "OK");
            return;
        }

        if (PasswordEntry.Text != ConfermaPasswordEntry.Text)
        {
            DisplayAlert("Errore", "Le password non corrispondono.", "OK");
            return;
        }

        NomeTitolare = NomeTitoloareEntry.Text;
        CognomeTitolare = CognomeTitolareEntry.Text;
        Email = EmailEntry.Text;
        Password = PasswordEntry.Text;
        ConfermaPassword = ConfermaPasswordEntry.Text;

        MakeRegistration();
    }

    private async void MakeRegistration()
    {
        var builder = new ConfigurationBuilder().AddUserSecrets<MainPage>();
        var configuration = builder.Build();


        using(HttpClient client = new HttpClient())
        {
            string apiURL = "http://192.168.40.45:8080/api/register";

            RegisterData registerData = new RegisterData
            {
                firstName = NomeTitolare,
                lastName = CognomeTitolare,
                picture = "https://images.pexels.com/photos/268533/pexels-photo-268533.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500",
                username = Email,
                password = Password
            };

            try
            {
                string jsonData = JsonConvert.SerializeObject(registerData);

                StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiURL, content);

                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Successo", "Registrazione avvenuta con successo!", "OK");
                }
                else
                {
                    await DisplayAlert("Errore", "Email già esistente.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "C'e stato un errore: " + ex.Message, "OK");
            }
        }
     }
 }
