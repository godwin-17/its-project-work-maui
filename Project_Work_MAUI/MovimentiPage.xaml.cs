using Project_Work_MAUI.Models;
using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class MovimentiPage : ContentPage
{
	public MovimentiPage(MoviementiViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        (BindingContext as MoviementiViewModel)?.LoadData();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Transaction transaction)
        {
            await Shell.Current.GoToAsync("DetailsPage", new Dictionary<string, object>
            {
                ["Transaction"] = transaction
            });
        }
    }
}