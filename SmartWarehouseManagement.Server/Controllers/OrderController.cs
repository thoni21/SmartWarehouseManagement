using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouseManagement.Server.Data;
using SmartWarehouseManagement.Server.Models;

namespace SmartWarehouseManagement.Server.Controllers
{
    [Route("orders/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderController(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetOrders() {
            return _dbContext.Orders.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Order> GetOrder(int id)
        {
            if (_dbContext.Orders.Find(id) is not Order order)
            {
                return BadRequest($"No order with an id of {id} exists");
            }

            return order;
        }

        [HttpPost]
        public ActionResult<Order> PostOrder(Order order) {
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public IActionResult EditShipmentStatus(int id, Order order) { 
            if(id != order.Id)
            {
                throw new InvalidOperationException("Id mismatch");
            }

            if (_dbContext.Orders.Find(id) is not Order orderToEdit)
            {
                return BadRequest($"No order with an id of {id} exists");
            }

            orderToEdit.Shipped = order.Shipped;
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id) {
            if (_dbContext.Orders.Find(id) is not Order orderToDelete)
            {
                return BadRequest($"No order with an id of {id} exists");
            }            

            _dbContext.Orders.Remove(orderToDelete);
            _dbContext.SaveChanges();
            return NoContent();
        }
    }
}
