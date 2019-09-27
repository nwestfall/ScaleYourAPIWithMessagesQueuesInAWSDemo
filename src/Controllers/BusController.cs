using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Amazon.SQS;
using API.Models;
using Newtonsoft.Json;
using Amazon.SQS.Model;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class BusController : Controller
    {
        readonly BusLocationDataContext _context;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context"></param>
        public BusController(BusLocationDataContext context)
        {
            _context = context;
        }

        // GET api/bus
        [HttpGet]
        public async Task<IEnumerable<Bus>> Get()
            => await _context.Buses.Include(b => b.BusLocations).ToListAsync();

        // GET api/bus/5
        [HttpGet("{id}")]
        public async Task<Bus> Get(int id)
            => await _context.Buses.Include(b => b.BusLocations).FirstOrDefaultAsync(bl => bl.BusID == id);

        // POST api/bus
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Bus value)
        {
            // Database
            await _context.Buses.AddAsync(value);
            await _context.SaveChangesAsync();

            return Ok(value.BusID);
        }

        // PUT api/bus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]Bus value)
        {
            // Database
            var originalBus = _context.Buses.FirstOrDefault(bl => bl.BusID == id);
            originalBus.BusNumber = value.BusNumber;
            _context.Buses.Update(originalBus);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/bus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bus = await _context.Buses.FirstOrDefaultAsync(bl => bl.BusID == id);
            var busLocations = _context.BusLocations.Where(bl => bl.BusID == id);
            if(await busLocations?.AnyAsync() == true)
                _context.BusLocations.RemoveRange(busLocations);
            if(bus != null)
                _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
