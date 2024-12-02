using Microsoft.EntityFrameworkCore;
using SmartWarehouseManagement.Server.Controllers;
using SmartWarehouseManagement.Server.Data;
using SmartWarehouseManagement.Server.Models;
using Microsoft.AspNetCore.Mvc;


namespace SmartWarehouseManager.Tests
{
    [TestFixture]
    public class OrderControllerTests
    {
        private ApplicationDbContext _dbContext;
        private OrderController _controller;
        private List<Order> _orders;

        [SetUp]
        public void SetUp()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _orders = new List<Order>
            {
                new Order { Id = 1, Customer = "a@a.com", OrderNr = "1234", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = false },
                new Order { Id = 2, Customer = "b@b.com", OrderNr = "4321", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = false }
            };

            // Seed test data
            _dbContext.Orders.AddRange(_orders);
            _dbContext.SaveChanges();

            // Detach all entities to ensure AsNoTracking behavior in tests
            foreach (var entity in _dbContext.ChangeTracker.Entries())
            {
                entity.State = EntityState.Detached;
            }

            _controller = new OrderController(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void GetOrders_ReturnsAllOrders()
        {
            // Act
            var result = _controller.GetOrders();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<Order>>>(result);

            var gottenOrders = result.Value.ToList();
            Assert.That(gottenOrders.Count, Is.EqualTo(_orders.Count()));
            Assert.That(gottenOrders[0].Id, Is.EqualTo(_orders[0].Id));
            Assert.That(gottenOrders[1].Id, Is.EqualTo(_orders[1].Id));
        }

        [Test]
        public void GetOrder_ReturnsOrderByID()
        {
            // Act
            var result = _controller.GetOrder(1);

            // Assert
            Assert.IsInstanceOf<ActionResult<Order>>(result);

            var gottenOrder = result.Value;
            Assert.IsNotNull(gottenOrder);
            Assert.That(gottenOrder.Id, Is.EqualTo(1));
        }

        [Test]
        public void PostOrder_AddsOrderToDB()
        {
            // Arrange
            Order payload = new Order { Id = 3, Customer = "c@c.com", OrderNr = "3333", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = false };

            // Act
            _controller.PostOrder(payload);
            var result = _controller.GetOrder(payload.Id);

            // Assert
            Assert.IsInstanceOf<ActionResult<Order>>(result);

            var addedOrder = result.Value;
            Assert.IsNotNull(addedOrder);
            Assert.That(addedOrder, Is.EqualTo(payload));
        }

        [Test]
        public void EditShipmentStatus_EditsShipmentStatusOfOrder()
        {
            // Arrange
            Order payload = new Order { Id = 1, Customer = "a@a.com", OrderNr = "1234", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = true };
            Order beforePut = _orders.First();

            // Act
            _controller.EditShipmentStatus(beforePut.Id, payload);
            var result = _controller.GetOrder(beforePut.Id);

            // Assert
            Assert.IsInstanceOf<ActionResult<Order>>(result);

            var upatedOrder = result.Value;
            Assert.IsNotNull(upatedOrder);
            Assert.That(upatedOrder.Shipped, Is.EqualTo(payload.Shipped));
            Assert.That(upatedOrder.Shipped, Is.Not.EqualTo(beforePut.Shipped));
        }

        [Test]
        public void DeleteOrder_DeletesOrderFromDB()
        {
            // Arrange
            int orderToDelete = 1;

            // Act
            _controller.DeleteOrder(orderToDelete);
            var result = _controller.DeleteOrder(orderToDelete);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);

            var expectedMessage = $"No order with an id of {orderToDelete} exists";
            Assert.That(badRequestResult.Value, Is.EqualTo(expectedMessage));
        }
    }
}
