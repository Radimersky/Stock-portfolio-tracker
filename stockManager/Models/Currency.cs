using System.ComponentModel.DataAnnotations;

namespace stockManager.Models
{
    public class Currency
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }


    }
}
 