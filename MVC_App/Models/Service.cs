using System.ComponentModel.DataAnnotations;

namespace MVC_App.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Title daxil edilmelidir")]
        [MinLength(2, ErrorMessage = "Minimum uzunluq 2 olmalidir")]

        public string Title { get; set; } = null!;
        [MaxLength(100, ErrorMessage = "Maximum uzunluq 100 olmalidir")]
        [Required]
        public string Description { get; set; } = null!;
        [MinLength(4, ErrorMessage = "Minimum uzunluq 4 olmalidir")]

        public string PhotoUrl { get; set; } = null!;
    }
}
