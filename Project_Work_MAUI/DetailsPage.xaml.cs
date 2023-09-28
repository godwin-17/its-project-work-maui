using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class DetailsPage : ContentPage
{
	public DetailsPage(DetailsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}