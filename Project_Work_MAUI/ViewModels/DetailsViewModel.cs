using CommunityToolkit.Mvvm.ComponentModel;
using Project_Work_MAUI.Models;

namespace Project_Work_MAUI.ViewModels
{
    [QueryProperty("Transaction", "Transaction")]
    public partial class DetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        Transaction transaction;
    }
}
