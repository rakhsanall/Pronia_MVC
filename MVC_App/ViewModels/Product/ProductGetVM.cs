namespace MVC_App.ViewModels.Product
{
    public class ProductGetVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public string MainImagePath { get; set; }
        public string HoverImagePath { get; set; }
        public int Rate { get; set; }
        public List<string> TagNames    { get; set; }
    }
}
