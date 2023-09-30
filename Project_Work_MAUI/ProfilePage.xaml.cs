using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfileViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

		var viewModel = BindingContext as ProfileViewModel;
		viewModel?.LoadData();
    }
}