using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class RicaricaPage : ContentPage
{
	public RicaricaPage(RicaricaViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}