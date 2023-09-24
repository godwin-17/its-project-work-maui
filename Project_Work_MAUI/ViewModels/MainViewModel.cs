﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Project_Work_MAUI.Models;
using System.Text.RegularExpressions;

namespace Project_Work_MAUI.ViewModels
{
    
    public partial class MainViewModel : ObservableObject
    {
        IDispatcher dispatcher;
        public MainViewModel(IDispatcher dispatcher) 
        { 
            this.dispatcher = dispatcher;
        }

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        private bool timerExpired;
        private bool pageDisappearing = false;

        public void StartTimer()
        {
            timerExpired = false;
            bool showAlert = true;


            dispatcher.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                if (pageDisappearing)
                {
                    return false;
                }

                timerExpired = true;
                if (showAlert)
                {
                    dispatcher.Dispatch(async () =>
                    {
                        showAlert = false;
                        await Application.Current.MainPage.DisplayAlert("Timeout", "Il tempo per fare il login è scaduto. Perfavore riprova.", "OK");
                        ClearInputs();
                        StartTimer();
                    });
                }
                return false;
            });
        }

        public void StopTimer()
        {
            pageDisappearing = true;
        }

        public void ResetPageDisappearing()
        {
            pageDisappearing = false;
        }

        [RelayCommand]
        async Task TapRegister()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }

        [RelayCommand]
        async Task Login()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password)) 
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

            await MakeLogin();
        }

        private async Task MakeLogin()
        {
            using (HttpClient client = new HttpClient())
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
                        await Application.Current.MainPage.DisplayAlert("Successo", "Login avvenuto con successo.", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Errore", "Credenziali errate.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", "C'è stato un errore: " + ex.Message, "OK");
                }
            }
        }

        private void ClearInputs()
        {
            Email = string.Empty;
            Password = string.Empty;
            timerExpired = false;
        }
    }
}