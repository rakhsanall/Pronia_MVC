using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MVC_App.Models
{
    public class Product:BaseEntity
    {
        [Required(ErrorMessage ="Name mutleq daxil edilmelidir")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Precision(5,2)]
        public decimal Price { get; set; }
        public string PhotoUrl { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
