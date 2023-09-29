using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class BonificoPage : ContentPage
{
	public BonificoPage(BonificoViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}