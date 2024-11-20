using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartWarehouseManagement.Server.Data;
using SmartWarehouseManagement.Server.Models;
using System;

namespace SmartWarehouseManagement.Server.Controllers
{
    [Route("inventory/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ItemController(ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Item>> GetItems()
        {
            return _dbContext.Items.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Item> GetItem(int id)
        {
            return _dbContext.Items.Find(id) ?? 
                throw new InvalidOperationException("No item with an id of " + id + " exists");
        }

        [HttpPost]
        public ActionResult<Item> PostItem(Item item) { 
            _dbContext.Items.Add(item);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult PutItem(int id, Item item) { 
            
            if (id != item.Id)
            {
                throw new Exception("Id's doesn't match");
            }

            _dbContext.Items.Update(item);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id) {
            Item itemToRemove = _dbContext.Items.Find(id) ??
                throw new InvalidOperationException("No item with an id of " + id + " exists");

            _dbContext.Items.Remove(itemToRemove);
            _dbContext.SaveChanges();
            return NoContent();
        }
        
    }
}
