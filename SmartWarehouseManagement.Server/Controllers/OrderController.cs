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
            return _dbContext.Orders.Find(id) ?? 
                throw new InvalidOperationException("No item with an id of " + id + " exists");
        }

        [HttpPost]
        public ActionResult<Order> PostOrder(Order order) { 
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpDelete]
        public IActionResult DeleteOrder(int id) {

            Order orderToDelete = _dbContext.Orders.Find(id) ?? 
                throw new InvalidOperationException("No item with an id of " + id + " exists");

            _dbContext.Orders.Remove(orderToDelete);
            _dbContext.SaveChanges();
            return NoContent();
        }
    }
}
