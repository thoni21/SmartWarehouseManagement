using System.ComponentModel.DataAnnotations;

namespace SmartWarehouseManagement.Server.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public required string Customer {  get; set; }
        public required string OrderNr { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? Price { get; set; }
        public bool Shipped { get; set; } = false;
        public bool Cancelled { get; set; } = false;
    }
}
