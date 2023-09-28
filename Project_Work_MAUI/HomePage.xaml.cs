using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class HomePage : ContentPage
{
	public HomePage(HomeViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        (BindingContext as HomeViewModel)?.LoadData();

    }
}