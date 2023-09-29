using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class MovimentiPage : ContentPage
{
	public MovimentiPage(MoviementiViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}