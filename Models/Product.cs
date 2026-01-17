namespace CrackersStore.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string Image { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public bool InStock { get; set; }
        public List<string> Features { get; set; } = new();
        public string Safety { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
