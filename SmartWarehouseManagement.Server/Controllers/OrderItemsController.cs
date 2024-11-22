using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWarehouseManagement.Server.Data;
using SmartWarehouseManagement.Server.Models;

namespace SmartWarehouseManagement.Server.Controllers
{
    [Route("order/items/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public OrderItemsController(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<OrderItem>> GetOrderItems()
        {
            return _dbContext.OrderItems.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<OrderItem> GetOrderItem(int id)
        {
            return _dbContext.OrderItems.Include(oi => oi.Order)
                                        .Include(oi => oi.Item) 
                                        .FirstOrDefault(oi => oi.Id == id) 
                                        ?? throw new InvalidOperationException("No OrdeItem with id: " + id + " exists."); 
        }

        [HttpPost]
        public ActionResult<OrderItem> PostOrderItem(OrderItem orderItem)
        {
            Order order = _dbContext.Orders.Find(orderItem.Order.Id) ??
                throw new InvalidOperationException("No order with id: " + orderItem.Order + " exists.");

            Item item = _dbContext.Items.Find(orderItem.Item.Id) ?? 
                throw new InvalidOperationException("No item with id: " + orderItem.Item + " exists.");

            orderItem.Order = order;
            orderItem.Item = item;  

            if(orderItem.Item.QuantityInStock < orderItem.Quantity)
            {
                return BadRequest("Not enough stock available. Item quantity: " + orderItem.Item.QuantityInStock + " Amount trying to be bought: " + orderItem.Quantity);
            }

            item.QuantityInStock -= orderItem.Quantity;

            _dbContext.OrderItems.Add(orderItem);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.Id }, orderItem);
        }
    }
}
