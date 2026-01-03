using System.ComponentModel.DataAnnotations;

namespace MVC_App.ViewModels.Product
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }
        public IFormFile? MainImage { get; set; }
        public IFormFile? HoverImage { get; set; }

        [Range(1, 5, ErrorMessage = "1 ve 5 arasi deyerlendirin")]
        public int Rate { get; set; }
        public string? MainImagePath { get; set; }
        public string? HoverImagePath { get; set; }
        public ICollection<IFormFile>? Images { get; set; } = [];
        public List<string>? AdditionalImagePaths { get; set; } = [];

        public List<int>? AdditionalImageIds { get; set; } = [];

    }
}
