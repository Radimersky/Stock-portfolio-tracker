using System.ComponentModel.DataAnnotations;

namespace stockManager.Models
{
    public class Market
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        public string MIC { get; set; }
    }
}
 