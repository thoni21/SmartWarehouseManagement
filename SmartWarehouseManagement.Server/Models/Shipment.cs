using System.ComponentModel.DataAnnotations;

namespace SmartWarehouseManagement.Server.Models
{
    public class Shipment
    {
        [Key]
        public int Id { get; set; }
        public required Order Order { get; set; }
        public DateTime DateOfShipment { get; set; }
        public decimal WeightOfShipment { get; set; }
        public string? SizeOfShipment { get; set; }
        public string? Carrier {  get; set; }
        public string? TrackingNumber { get; set; }
    }
}
