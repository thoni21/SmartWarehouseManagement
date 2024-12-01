using Microsoft.EntityFrameworkCore;
using SmartWarehouseManagement.Server.Controllers;
using SmartWarehouseManagement.Server.Data;
using SmartWarehouseManagement.Server.Models;
using Microsoft.AspNetCore.Mvc;


namespace SmartWarehouseManager.Tests
{
    [TestFixture]
    public class ItemControllerTests
    {
        private ApplicationDbContext _dbContext;
        private ItemController _controller;
        private List<Item> _items;

        [SetUp]
        public void SetUp()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _items = new List<Item>
            {
                new Item { Id = 1, Category = "cat1", Name = "name1", Price = 10, QuantityInStock = 10, Shelf = "A1", ShelfPosition = "11", Size = "L", WeightInKg = 1 },
                new Item { Id = 2, Category = "cat2", Name = "name2", Price = 20, QuantityInStock = 20, Shelf = "A2", ShelfPosition = "12", Size = "L", WeightInKg = 1 }
            };

            // Seed test data
            _dbContext.Items.AddRange(_items);
            _dbContext.SaveChanges();

            // Detach all entities to ensure AsNoTracking behavior in tests
            foreach (var entity in _dbContext.ChangeTracker.Entries())
            {
                entity.State = EntityState.Detached;
            }

            _controller = new ItemController(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void GetItems_ReturnsAllItems()
        {
            // Act
            var result = _controller.GetItems();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<Item>>>(result);

            var gottenItems = result.Value.ToList();
            Assert.That(gottenItems.Count, Is.EqualTo(_items.Count()));
            Assert.That(gottenItems[0].Name, Is.EqualTo(_items[0].Name));
            Assert.That(gottenItems[1].Name, Is.EqualTo(_items[1].Name));
        }

        [Test]
        public void GetItem_ReturnsItemByID()
        {
            // Act
            var result = _controller.GetItem(1);

            // Assert
            Assert.IsInstanceOf<ActionResult<Item>>(result);

            var gottenItem = result.Value;
            Assert.IsNotNull(gottenItem);
            Assert.That(gottenItem.Id, Is.EqualTo(1));
        }

        [Test]
        public void PostItem_AddsItemToDB()
        {
            // Arrange
            Item payload = new Item { Id = 3, Category = "cat3", Name = "name3", Price = 30, QuantityInStock = 30, Shelf = "A3", ShelfPosition = "13", Size = "L", WeightInKg = 1 };

            // Act
            _controller.PostItem(payload);
            var result = _controller.GetItem(payload.Id);

            // Assert
            Assert.IsInstanceOf<ActionResult<Item>>(result);

            var addedItem = result.Value;
            Assert.IsNotNull(addedItem);
            Assert.That(addedItem, Is.EqualTo(payload));
        }

        [Test]
        public void PutItem_UpdatesItemInDB()
        {
            // Arrange
            Item payload = new Item { Id = 1, Category = "EditedCat1", Name = "Editedname1", Price = 10, QuantityInStock = 10, Shelf = "EditedA1", ShelfPosition = "Edited11", Size = "EditedL", WeightInKg = 1 };
            Item beforePut = _items.First();

            // Act
            _controller.PutItem(beforePut.Id, payload);
            var result = _controller.GetItem(beforePut.Id);

            // Assert
            Assert.IsInstanceOf<ActionResult<Item>>(result);

            var upatedItem = result.Value;
            Assert.IsNotNull(upatedItem);
            Assert.That(upatedItem, Is.EqualTo(payload));
            Assert.That(upatedItem, Is.Not.EqualTo(beforePut));
        }

        [Test]
        public void DeleteItem_DeletesItemFromDB() 
        {
            // Arrange
            int itemToDelete = 1;

            // Act
            var result = _controller.GetItem(itemToDelete);
            _controller.DeleteItem(1);

            // Assert
            Assert.IsInstanceOf<ActionResult<Item>>(result);

            var deletedItem = result.Value;
            Assert.IsNotNull(deletedItem);
            Assert.Throws<InvalidOperationException>(() => _controller.GetItem(deletedItem.Id));
        }
    }
}
