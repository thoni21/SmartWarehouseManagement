using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWarehouseManagement.Server.Controllers;
using SmartWarehouseManagement.Server.Data;
using SmartWarehouseManagement.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWarehouseManager.Tests
{
    internal class ShipmentControllerTests
    {
        private ApplicationDbContext _dbContext;
        private ShipmentController _controller;
        private List<Shipment> _shipments;

        [SetUp]
        public void SetUp()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            Order order1 = new Order { Id = 1, Customer = "a@a.com", OrderNr = "1234", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = false };
            Order order2 = new Order { Id = 2, Customer = "b@b.com", OrderNr = "4321", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = true };

            _shipments = new List<Shipment>
            {
                new Shipment { Id = 1, Order = order1, Carrier = "DHL", DateOfShipment = new DateTime(), SizeOfShipment = "L", TrackingNumber = "asd123", WeightOfShipment = 1 },
                new Shipment { Id = 2, Order = order2, Carrier = "DHL", DateOfShipment = new DateTime(), SizeOfShipment = "L", TrackingNumber = "asd321", WeightOfShipment = 1 }
            };

            // Seed test data
            _dbContext.Shipments.AddRange(_shipments);
            _dbContext.SaveChanges();

            // Detach all entities to ensure AsNoTracking behavior in tests
            foreach (var entity in _dbContext.ChangeTracker.Entries())
            {
                entity.State = EntityState.Detached;
            }

            _controller = new ShipmentController(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void GetShipments_ReturnsAllShipments()
        {
            // Act
            var result = _controller.GetShipments();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<Shipment>>>(result);

            var gottenShipments = result.Value.ToList();
            Assert.That(gottenShipments.Count, Is.EqualTo(_shipments.Count()));
            Assert.That(gottenShipments[0].Id, Is.EqualTo(_shipments[0].Id));
            Assert.That(gottenShipments[1].Id, Is.EqualTo(_shipments[1].Id));
        }

        [Test]
        public void GetShipment_ReturnsShipmentByID()
        {
            // Act
            var result = _controller.GetShipment(1);

            // Assert
            Assert.IsInstanceOf<ActionResult<Shipment>>(result);

            var gottenShipment = result.Value;
            Assert.IsNotNull(gottenShipment);
            Assert.That(gottenShipment.Id, Is.EqualTo(1));
        }

        [Test]
        public void CreateShipment_AddsShipmentToDB()
        {
            // Arrange
            Order order = new Order { Id = 1, Customer = "a@a.com", OrderNr = "1234", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = false };
            Shipment payload = new Shipment { Id = 3, Order = order, Carrier = "DHL", DateOfShipment = new DateTime(), SizeOfShipment = "L", TrackingNumber = "qwe456", WeightOfShipment = 1 };

            // Act
            var result = _controller.CreateShipment(payload);

            // Assert
            Assert.IsInstanceOf<ActionResult<Shipment>>(result);

            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var postedShipment = createdResult.Value as Shipment;
            Assert.IsNotNull(postedShipment);
            Assert.That(postedShipment, Is.EqualTo(payload));

            // Assures that the "shipped" value of the order item is set to true
            Assert.That(postedShipment.Order.Shipped, Is.EqualTo(true));
        }

        [Test]
        public void CreateShipment_CantAddShippedShipmentToDB()
        {
            // Arrange
            Order order = new Order { Id = 2, Customer = "b@b.com", OrderNr = "4321", OrderDate = new DateTime(), Price = 10, Cancelled = false, Shipped = true };
            Shipment payload = new Shipment { Id = 3, Order = order, Carrier = "DHL", DateOfShipment = new DateTime(), SizeOfShipment = "L", TrackingNumber = "qwe456", WeightOfShipment = 1 };

            // Act
            var result = _controller.CreateShipment(payload);

            // Assert
            Assert.IsInstanceOf<ActionResult<Shipment>>(result);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);

            var expectedMessage = "Order has already been shipped.";
            Assert.That(badRequestResult.Value, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void DeleteShipment_DeletesShipmentFromDB()
        {
            // Arrange
            int shipmentToDelete = 1;

            // Act
            var result = _controller.GetShipment(shipmentToDelete);
            _controller.DeleteShipment(shipmentToDelete);

            // Assert
            Assert.IsInstanceOf<ActionResult<Shipment>>(result);

            var deletedShipment = result.Value;
            Assert.IsNotNull(deletedShipment);
            Assert.Throws<InvalidOperationException>(() => _controller.GetShipment(deletedShipment.Id));
        }
    }
}
