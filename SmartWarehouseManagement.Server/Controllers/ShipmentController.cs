using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
            var shipment = _dbContext.Shipments.Include(s => s.Order)
                .FirstOrDefault(s => s.Id == id);

            if (shipment == null) {
                return BadRequest($"No shipment with id: {id} exists");
            }

            return shipment;
        }

        [HttpPost]
        public ActionResult<Shipment> CreateShipment(Shipment shipment) {
            if (_dbContext.Orders.Find(shipment.Order.Id) is not Order order)
            {
                return BadRequest($"No order with an id of {shipment.Order.Id} exists");
            }

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
            var shipmentToDelete = _dbContext.Shipments
                .Include(s => s.Order)
                .FirstOrDefault(s => s.Id == id);

            if (shipmentToDelete == null) {
                return BadRequest($"No shipment with id: {id} exists.");
            }

            if (_dbContext.Orders.Find(shipmentToDelete.Order.Id) is not Order order)
            {
                return BadRequest($"No order with an id of {shipmentToDelete.Order.Id} exists");
            }

            order.Shipped = false;

            _dbContext.Shipments.Remove(shipmentToDelete);
            _dbContext.SaveChanges();
            return NoContent();
        }
    }
}
