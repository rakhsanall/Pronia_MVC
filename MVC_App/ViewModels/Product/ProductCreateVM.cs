using MVC_App.Models;
using System.ComponentModel.DataAnnotations;

namespace MVC_App.ViewModels.Product
{
    public class ProductCreateVM
    {
        public string Name { get; set; }
        public string Description { get; set; } 
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }
        public IFormFile MainImage { get; set; }
        public IFormFile HoverImage { get; set; }

        [Range(1,5,ErrorMessage ="1 ve 5 arasi deyerlendirin")]
        public int Rate { get; set; }
        public ICollection<IFormFile> Images { get; set; } = [];
        

    }
}
