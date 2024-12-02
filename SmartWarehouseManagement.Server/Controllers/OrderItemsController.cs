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
            var orderItem = _dbContext.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Item)
                .FirstOrDefault(oi => oi.Id == id);

            if (orderItem == null)
            {
                return BadRequest($"No OrderItem with id: {id} exists.");
            }

            return orderItem;
        }

        [HttpGet("{orderId}/items")]
        public ActionResult<IEnumerable<OrderItem>> GetOrderItemsByOrderId(int orderId)
        {
            var orderItems = _dbContext.OrderItems
                                       .Include(oi => oi.Order)
                                       .Include(oi => oi.Item)
                                       .Where(oi => oi.Order.Id == orderId)
                                       .ToList();

            if (orderItems == null || !orderItems.Any())
            {
                return BadRequest($"No OrderItems found for OrderId: {orderId}");
            }

            return Ok(orderItems);
        }


        [HttpPost]
        public ActionResult<OrderItem> PostOrderItem(OrderItem orderItem)
        {
            if (_dbContext.Orders.Find(orderItem.Order.Id) is not Order order)
            {
                return BadRequest($"No order with id: {orderItem.Order.Id} exists.");
            }
            
            if (_dbContext.Items.Find(orderItem.Item.Id) is not Item item)
            {
                return BadRequest($"No item with id: {orderItem.Item.Id} exists.");
            }

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

        [HttpDelete("{id}")]
        public IActionResult DeleteOrderItem(int id) {
            if (_dbContext.OrderItems.Find(id) is not OrderItem orderItemToDelete)
            {
                return BadRequest($"No OrderItem with id: {id} exists.");
            }

            _dbContext.OrderItems.Remove(orderItemToDelete);
            _dbContext.SaveChanges();
            return NoContent();
        }
    }
}
