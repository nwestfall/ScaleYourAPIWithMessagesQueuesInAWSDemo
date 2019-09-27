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
    public class BusLocationController : Controller
    {
        readonly IAmazonSQS _sqsClient;

        readonly BusLocationDataContext _context;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="sqsClient"></param>
        public BusLocationController(BusLocationDataContext context, IAmazonSQS sqsClient)
        {
            _context = context;
            _sqsClient = sqsClient;
        }

        // GET api/buslocation
        [HttpGet]
        public async Task<IEnumerable<BusLocation>> Get()
            => await _context.BusLocations.Include(bl => bl.Bus).ToListAsync();

        // GET api/buslocation/5
        [HttpGet("{id}")]
        public async Task<BusLocation> Get(int id)
            => await _context.BusLocations.Include(bl => bl.Bus).FirstOrDefaultAsync(bl => bl.BusLocationID == id);

        // POST api/buslocation
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]BusLocation value)
        {
            // Database
            await _context.BusLocations.AddAsync(value);
            await _context.SaveChangesAsync();

            // Get the bus
            value.Bus = await _context.Buses.FirstOrDefaultAsync(b => b.BusID == value.BusID);

            // SQS
            await _sqsClient.SendMessageAsync(new SendMessageRequest()
            {
                MessageBody = JsonConvert.SerializeObject(value),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>()
                {
                    { "Action", new MessageAttributeValue() { DataType = "String", StringValue = "ADD" } }
                },
                QueueUrl = Startup.QUEUE_URL
            });

            return Ok(value.BusLocationID);
        }

        // PUT api/buslocation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]BusLocation value)
        {
            // Database
            var originalBusLocation = _context.BusLocations.FirstOrDefault(bl => bl.BusLocationID == id);
            originalBusLocation.BusID = value.BusID;
            originalBusLocation.Latitude = value.Latitude;
            originalBusLocation.Longitude = value.Longitude;
            originalBusLocation.StopID = value.StopID;
            _context.BusLocations.Update(originalBusLocation);
            await _context.SaveChangesAsync();

            // Set bus
            value.BusLocationID = originalBusLocation.BusLocationID;
            value.Bus = await _context.Buses.FirstOrDefaultAsync(b => b.BusID == value.BusID);

            // SQS
            await _sqsClient.SendMessageAsync(new SendMessageRequest()
            {
                MessageBody = JsonConvert.SerializeObject(value),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>()
                {
                    { "Action", new MessageAttributeValue() { DataType = "String", StringValue = "UPDATE" } }
                },
                QueueUrl = Startup.QUEUE_URL
            });

            return NoContent();
        }

        // DELETE api/buslocation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var busLocation = await _context.BusLocations.FirstOrDefaultAsync(bl => bl.BusLocationID == id);
            if(busLocation != null)
                _context.BusLocations.Remove(busLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
