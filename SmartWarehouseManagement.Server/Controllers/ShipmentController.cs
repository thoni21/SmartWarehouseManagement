using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWarehouseManagement.Server.Data;
using SmartWarehouseManagement.Server.Models;

namespace SmartWarehouseManagement.Server.Controllers
{
    [Route("shipment/[controller]")]
    [ApiController]
    public class ShipmentController : ControllerBase
    {
        ApplicationDbContext _dbContext;

        public ShipmentController(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Shipment>> GetShipments() {
            return _dbContext.Shipments.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Shipment> GetShipment(int id)
        {
            return _dbContext.Shipments.Include(s => s.Order)
                                       .FirstOrDefault(s => s.Id == id)
                                       ?? throw new InvalidOperationException("No shipment with id: " + id + " exists.");
        }

        [HttpPost]
        public ActionResult<Shipment> CreateShipment(Shipment shipment) { 

            Order order = _dbContext.Orders.Find(shipment.Order.Id) ?? 
                throw new InvalidOperationException("No order with id: " + shipment.Order.Id + " exists.");

            if (order.Shipped)
            {
                return BadRequest("Order has already been shipped.");
            }

            order.Shipped = true;

            shipment.Order = order;

            _dbContext.Shipments.Add(shipment);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetShipment), new { id = shipment.Id }, shipment);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteShipment(int id) { 
            Shipment shipmentToDelete = _dbContext.Shipments.Include(s => s.Order)
                                                            .FirstOrDefault(s => s.Id == id) 
                                                            ?? throw new InvalidOperationException("No shipment with id: " + id + " exists.");
            
            _dbContext.Orders.Find(shipmentToDelete.Order.Id).Shipped = false;

            _dbContext.Shipments.Remove(shipmentToDelete);
            _dbContext.SaveChanges();
            return NoContent();
        }
    }
}
