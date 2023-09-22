﻿using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Text.RegularExpressions;

namespace Project_Work_MAUI
{
    public partial class MainPage : ContentPage
    {
        private string Email;
        private string Password;
        public MainPage()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        private void Login_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text)) 
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

            Email = EmailEntry.Text;
            Password = PasswordEntry.Text;

            MakeLogin();
        }

        private async void MakeLogin()
        {
            using(HttpClient client = new HttpClient())
            {
                string apiURL = "http://192.168.40.45:8080/api/login";

                LoginData loginData = new LoginData
                {
                    username = Email,
                    password = Password,
                };

                try
                {
                    string jsonData = JsonConvert.SerializeObject(loginData);

                    StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiURL, content);

                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Successo", "Login avvenuto con successo.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Errore", "Credenziali errate.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Errore", "C'è stato un errore: " + ex.Message, "OK");
                }
            }
        }
    }
}