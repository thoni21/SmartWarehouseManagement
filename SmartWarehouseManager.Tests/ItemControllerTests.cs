using Moq;
using Microsoft.EntityFrameworkCore;
using SmartWarehouseManagement.Server.Controllers; 
using SmartWarehouseManagement.Server.Models;      
using SmartWarehouseManagement.Server.Data;
using Microsoft.AspNetCore.Mvc;

[TestFixture]
public class ItemControllerTests
{
    private Mock<DbSet<Item>> _mockDbSet;
    private Mock<ApplicationDbContext> _mockDbContext; 
    private ItemController _controller;        

    [SetUp]
    public void Setup()
    {
        _mockDbSet = new Mock<DbSet<Item>>();
        _mockDbContext = new Mock<ApplicationDbContext>();

        _mockDbContext.Setup(db => db.Items).Returns(_mockDbSet.Object);

        _controller = new ItemController(_mockDbContext.Object);
    }

    [Test]
    public void GetItems_ReturnsAllItems()
    {
        // Arrange
        var testItems = new List<Item>
        {
            new Item { Id = 1, Name = "Item1" },
            new Item { Id = 2, Name = "Item2" },
        }.AsQueryable();

        _mockDbSet.As<IQueryable<Item>>().Setup(m => m.Provider).Returns(testItems.Provider);
        _mockDbSet.As<IQueryable<Item>>().Setup(m => m.Expression).Returns(testItems.Expression);
        _mockDbSet.As<IQueryable<Item>>().Setup(m => m.ElementType).Returns(testItems.ElementType);
        _mockDbSet.As<IQueryable<Item>>().Setup(m => m.GetEnumerator()).Returns(testItems.GetEnumerator());

        // Act
        var result = _controller.GetItems();

        // Assert
        Assert.IsInstanceOf<ActionResult<IEnumerable<Item>>>(result);
        var actionResult = result.Result as OkObjectResult;
        Assert.IsNotNull(actionResult);
        var items = actionResult.Value as IEnumerable<Item>;
        Assert.IsNotNull(items);
        Assert.AreEqual(2, items.Count());
        Assert.AreEqual("Item1", items.First().Name);
    }
}
