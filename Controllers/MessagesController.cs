using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiForKwork.Models;
using ApiForKwork.SqlDbContext;

namespace ApiForKwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public MessagesController(MyDbContext context)
        {
            _context = context;
        }
        // GET: api/Messages/list
        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IEnumerable<UserMessageList>>> GetMessagesList(int userId)
        {
            if (_context.Messages == null)
            {
                return Problem("Oops... Something happened to the database");
            }
            return await _context.UserMessagesList.Where(m => m.ReciverId == userId).ToListAsync();
        }
        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            if (_context.Messages == null)
            {
                return Problem("Oops... Something happened to the database");
            }
            return await _context.Messages.ToListAsync();
        }

        // GET: api/Messages
        [HttpGet]
        [Route("inbox")]
        public async Task<ActionResult<IEnumerable<Message>>> GetInMessagesByUserId(int userId)
        {
            if (_context.Messages == null)
            {
                return Problem("Oops... Something happened to the database");
            }
            return await _context.Messages.Where(m=> m.ReciverId == userId).ToListAsync();
        }

        // GET: api/Messages
     
        [HttpGet]
        [Route("outbox")]
        public async Task<ActionResult<IEnumerable<Message>>> GetOutMessagesByUserId(int userId)
        {
            if (_context.Messages == null)
            {
                return Problem("Oops... Something happened to the database");
            }
            return await _context.Messages.Where(m => m.SenderId == userId).ToListAsync();
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            if (_context.Messages == null)
            {
                return Problem("Oops... Something happened to the database");
            }
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return Ok(new ApiResponse { Status = "successful", Message = "message is empty." });
            }

            return message;
        }


        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutMessage(int id, Message message)
        //{
        //    if (id != message.Id)
        //    {
        //        return Ok(new ApiResponse { Status = "failed", Message = "wrong request." });
        //    }

        //    _context.Entry(message).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MessageExists(id))
        //        {
        //            return Ok(new ApiResponse { Status = "failed", Message = "message is empty." });
        //        }
        //        else
        //        {
        //            return Ok(new ApiResponse { Status = "failed", Message = "message is empty." });
        //        }
        //    }

        //    return Ok(new ApiResponse { Status = "failed", Message = "wrong request." });
        //}

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("send")]
        public async Task<ActionResult<Message>> PostMessage(MessageSend send)
        {
            if (_context.Messages == null)
            {
                return Problem("Entity set 'MyDbContext.Messages'  is null.");
            }

            try
            {

                var message = new Message()
                {
                    ReciverId = send.ReciverId,
                    SenderId = send.SenderId,
                    CreatedAt = DateTime.Now,
                    Body = send.Body,
                    Title = send.Title,
                    IsDeleted = false,
                    IsReaded = false
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse { Status = "successful", Message = "message is send." });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "message not send." });
            }
        }

        // DELETE: api/Messages/5
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            if (_context.Messages == null)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "message is empty." });
            }
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "message is empty." });
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse { Status = "successful", Message = "message is removed." });
        }

        private bool MessageExists(int id)
        {
            return (_context.Messages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
