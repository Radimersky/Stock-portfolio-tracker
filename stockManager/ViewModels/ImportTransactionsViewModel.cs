using System.ComponentModel.DataAnnotations;

namespace stockManager.ViewModels
{
    public class ImportTransactionsViewModel
    {
        [Display(Name = "CSV Data")]
        public string CSV { get; set; }
    }
}
