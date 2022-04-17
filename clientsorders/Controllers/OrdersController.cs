using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientsOrders.Models;

namespace ClientsOrders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public OrdersController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public IEnumerable<OrderBase> GetOrders() =>
            _context.Orders.Select(o => new OrderBase(o));

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderBase>> GetOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            
            return Ok(new OrderBase(order));
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder([FromRoute] int id, [FromBody] Order order)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            if (id != order.Id) return BadRequest();
            
            _context.Entry(order).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> PostOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            
            _context.Orders.Remove(order);
            
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        private bool OrderExists(int id) =>
            _context.Orders.Any(e => e.Id == id);
    }
}