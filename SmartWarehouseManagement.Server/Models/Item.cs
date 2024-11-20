using System.ComponentModel.DataAnnotations;

namespace SmartWarehouseManagement.Server.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public decimal WeightInKg { get; set; }
        public string? Size { get; set; }
        public string? Shelf { get; set; }
        public string? ShelfPosition { get; set; }
        public string? Category { get; set; }
        public int QuantityInStock  { get; set; }
    }
}
