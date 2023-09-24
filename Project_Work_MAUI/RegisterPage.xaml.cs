using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm)
	{
        InitializeComponent();
        BindingContext = vm;
    }
 }
