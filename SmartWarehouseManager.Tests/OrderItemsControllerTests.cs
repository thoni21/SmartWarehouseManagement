using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Interfaces;
using SmartWarehouseManagement.Server.Controllers;
using SmartWarehouseManagement.Server.Data;
using SmartWarehouseManagement.Server.Models;


namespace SmartWarehouseManager.Tests
{
    internal class OrderItemsControllerTests
    {
        private ApplicationDbContext _dbContext;
        private OrderItemsController _controller;
        private List<OrderItem> _orderItems;

        [SetUp]
        public void SetUp()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            Item item1 = new Item { Id = 1, Category = "cat1", Name = "name1", Price = 10, QuantityInStock = 10, Shelf = "A1", ShelfPosition = "11", Size = "L", WeightInKg = 1 };
            Order order1 = new Order { Id = 1, Customer = "a@a.com", OrderNr = "1234", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = false };
            
            Item item2 = new Item { Id = 2, Category = "cat2", Name = "name2", Price = 20, QuantityInStock = 20, Shelf = "A2", ShelfPosition = "12", Size = "L", WeightInKg = 1 };
            Order order2 = new Order { Id = 2, Customer = "b@b.com", OrderNr = "4321", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = false };


            _orderItems = new List<OrderItem>
            {
                new OrderItem { Id = 1, Item = item1, Order = order1, Price = 10, Quantity = 2},
                new OrderItem { Id = 2, Item = item2, Order = order2, Price = 10, Quantity = 2}
            };

            // Seed test data
            _dbContext.OrderItems.AddRange(_orderItems);
            _dbContext.SaveChanges();

            // Detach all entities to ensure AsNoTracking behavior in tests
            foreach (var entity in _dbContext.ChangeTracker.Entries())
            {
                entity.State = EntityState.Detached;
            }

            _controller = new OrderItemsController(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void GetOrderItems_ReturnsAllOrderItems()
        {
            // Act
            var result = _controller.GetOrderItems();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<OrderItem>>>(result);

            var gottenItems = result.Value.ToList();
            Assert.That(gottenItems.Count, Is.EqualTo(_orderItems.Count()));
            Assert.That(gottenItems[0].Id, Is.EqualTo(_orderItems[0].Id));
            Assert.That(gottenItems[1].Id, Is.EqualTo(_orderItems[1].Id));
        }

        [Test]
        public void GetOrderItem_ReturnsOrderItemByID()
        {
            // Act
            var result = _controller.GetOrderItem(1);

            // Assert
            Assert.IsInstanceOf<ActionResult<OrderItem>>(result);

            var gottenOrderItem = result.Value;
            Assert.IsNotNull(gottenOrderItem);
            Assert.That(gottenOrderItem.Id, Is.EqualTo(1));
        }

        [Test]
        public void GetOrderItemsByOrderId_GetOrderItemsByOrderId()
        {
            // Arrange
            int orderId = 1;

            // Act
            var result = _controller.GetOrderItemsByOrderId(orderId);

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<OrderItem>>>(result);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var gottenOrderItems = okResult.Value as IEnumerable<OrderItem>;
            Assert.IsNotNull(gottenOrderItems);
            Assert.That(gottenOrderItems.ToList().Count(),Is.EqualTo(_orderItems.Where(oi => oi.Order.Id == orderId).Count()));

        }

        [Test]
        public void PostOrderItem_AddOrderItemToDB()
        {
            // Arrange
            Item item = new Item { Id = 1, Category = "cat1", Name = "name1", Price = 10, QuantityInStock = 10, Shelf = "A1", ShelfPosition = "11", Size = "L", WeightInKg = 1 };
            Order order = new Order { Id = 1, Customer = "a@a.com", OrderNr = "1234", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = false };

            OrderItem orderItem = new OrderItem { Id = 3, Item = item, Order = order, Price = 10, Quantity = 2 };

            // Act
            var result = _controller.PostOrderItem(orderItem);

            // Assert
            Assert.IsInstanceOf<ActionResult<OrderItem>>(result);

            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var createdOrderItem = createdResult.Value as OrderItem;
            Assert.IsNotNull(createdOrderItem);
            Assert.That(createdOrderItem.Id, Is.EqualTo(orderItem.Id));
        }

        // Testing ability to handle order of quantity higher than quantity in stock
        [Test]
        public void PostOrderItem_CantBuyItemOutOfStock()
        {
            // Arrange
            Item item = new Item { Id = 1, Category = "cat1", Name = "name1", Price = 10, QuantityInStock = 10, Shelf = "A1", ShelfPosition = "11", Size = "L", WeightInKg = 1 };
            Order order = new Order { Id = 1, Customer = "a@a.com", OrderNr = "1234", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = false };

            OrderItem orderItem = new OrderItem { Id = 3, Item = item, Order = order, Price = 10, Quantity = 20 };

            // Act
            var result = _controller.PostOrderItem(orderItem);

            // Assert
            Assert.IsInstanceOf<ActionResult<OrderItem>>(result);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);

            var expectedMessage = "Not enough stock available. Item quantity: " + item.QuantityInStock + " Amount trying to be bought: " + orderItem.Quantity;
            Assert.That(badRequestResult.Value, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DeleteOrderItem_DeletesOrderFromDB()
        {
            // Arrange
            int orderItemToDelete = 1;

            // Act
            _controller.DeleteOrderItem(orderItemToDelete);
            var result = _controller.DeleteOrderItem(orderItemToDelete);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);

            var expectedMessage = $"No OrderItem with id: {orderItemToDelete} exists.";
            Assert.That(badRequestResult.Value, Is.EqualTo(expectedMessage));
        }
    }
}
