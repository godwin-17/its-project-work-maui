using Project_Work_MAUI.Models;
using Project_Work_MAUI.ViewModels;
using System.Globalization;

namespace Project_Work_MAUI;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        (BindingContext as HomeViewModel)?.LoadData();

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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as HomeViewModel)?.LoadData();
    }


}