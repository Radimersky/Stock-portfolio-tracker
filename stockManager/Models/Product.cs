using System.ComponentModel.DataAnnotations;

namespace stockManager.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public string ISIN { get; set; }

        public string Ticker { get; set; }

        public Currency ProductCurrency { get; set; }

        public int ProductCurrencyId { get; set; }
    }
}
 