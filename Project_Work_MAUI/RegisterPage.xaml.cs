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

    }
}