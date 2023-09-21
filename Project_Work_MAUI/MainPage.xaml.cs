namespace Project_Work_MAUI
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}