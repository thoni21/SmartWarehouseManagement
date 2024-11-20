using System.ComponentModel.DataAnnotations;

namespace SmartWarehouseManagement.Server.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public required Order Order { get; set; }
        public required Item Item { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
