using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class RicaricaPage : ContentPage
{
	public RicaricaPage(RicaricaViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
		RadioButton radioButton = sender as RadioButton;

		string mobileProvider = (string)radioButton.Value;

		(BindingContext as RicaricaViewModel)?.SetMobileProvider(mobileProvider);
	}
}