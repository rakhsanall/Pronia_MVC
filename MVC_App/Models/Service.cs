namespace MVC_App.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;
        public string PhotoUrl { get; set; } = null!;
    }
}
