using Project_Work_MAUI.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Project_Work_MAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as MainViewModel)?.StopTimer();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            var viewModel = BindingContext as MainViewModel;
            viewModel?.ResetPageDisappearing();

            viewModel?.StartTimer();

        }
    }
}