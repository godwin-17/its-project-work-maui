using CommunityToolkit.Mvvm.ComponentModel;
using Project_Work_MAUI.Models;

namespace Project_Work_MAUI.ViewModels
{
    public partial class TransazioniViewModel: ObservableObject
    {
        [ObservableProperty]
        List<TransactionCategory> transactionCategories = new List<TransactionCategory>
        {
            new TransactionCategory
            {
                Category = "Pagamento Utenze",
                Typology = "Uscita",
                Id = "650d865bff8d876d587ff468"
            },
            new TransactionCategory
            {
                Category = "Versamento Bancomat",
                Typology = "Entrata",
                Id = "650d86baff8d876d587ff46c"
            },
            new TransactionCategory
            {
                Category = "Prelievo Contanti",
                Typology = "Uscita",
                Id = "6513efa173766e8ac7e52570"
            }
        };

        

    }
}
