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
            if (_dbContext.Items.Find(id) is not Item item)
            {
                return BadRequest($"No item with an id of {id} exists");
            }

            return item;
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
                return BadRequest("Id's doesn't match");
            }

            _dbContext.Items.Update(item);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id) {
            if (_dbContext.Items.Find(id) is not Item itemToRemove)
            {
                return BadRequest($"No item with an id of {id} exists");
            }

            _dbContext.Items.Remove(itemToRemove);
            _dbContext.SaveChanges();
            return NoContent();
        }
        
    }
}
