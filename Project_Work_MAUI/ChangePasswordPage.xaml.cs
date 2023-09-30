using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class ChangePasswordPage : ContentPage
{
	public ChangePasswordPage(ChangePasswordViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}