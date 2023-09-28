using Project_Work_MAUI.ViewModels;

namespace Project_Work_MAUI;

public partial class TransazioniPage : ContentPage
{
	public TransazioniPage(TransazioniViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}