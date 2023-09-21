namespace Project_Work_MAUI;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
        InitializeComponent();
	}

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}