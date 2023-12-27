using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiForKwork.Models;
using ApiForKwork.SqlDbContext;
using System.Text;
using Microsoft.Extensions.Hosting;
using System.Data.OleDb;
using System.Runtime.Intrinsics.Arm;

namespace ApiForKwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UsersController(MyDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_context.Users == null)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
            }
            else if(_context.Users.Count() == 0)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
            }

            var user = await _context.Users
                .Include("ProfileImage")
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
            }

            return user;
        }

        // GET: api/Users/avatar/get/5
        [HttpGet]
        [Route("avatar/get")]
        public async Task<ActionResult> GetUserAvatar(int userId)
        {
            try
            {
                if (_context.Users == null)
                {
                    return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
                }
                var avatar = await _context.ProfileImages.FirstOrDefaultAsync(a => a.UserId == userId);

                if (avatar == null)
                {
                    return Ok(new ApiResponse { Status = "failed", Message = "avatar is empty." });
                }
                else
                {
                    //if (avatar.DataBytes == null)
                    //{
                    //    byte[] image = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAIAAAACDbGyAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAANSURBVBhXY6AqYGAAAABQAAHTR4hjAAAAAElFTkSuQmCC");
                    //    return File(image, "image/png");
                    //}
                    //else
                    //{
                        byte[] image = System.IO.File.ReadAllBytes(Path.Combine(_environment.ContentRootPath, avatar.Data));
                        return File(image, "image/png");
                    //}
                }
            }
            catch(Exception ex)
            {
                return Ok(new ApiResponse { Status = "failed", Message = $"{ex.Message} Stack:{ex.StackTrace}" });
            }
        }
       
        [HttpPost]
        [Route("avatar/upload")]
        public async Task<ActionResult> Post(UserAvatar avatar)
        {
            try
            {
                if (UserExists(avatar.Id))
                {
                    var uploads = Path.Combine("uploads", "users");
                    var fullUploads = Path.Combine(_environment.ContentRootPath, uploads);
                    string FileName = "";

                    if (avatar.Avatar.Length > 0)
                    {
                        byte[] massiv = GetUrlFromBase64(avatar.Avatar);
                        FileName = $"usr{avatar.Id}_{Guid.NewGuid()}.png";

                        try
                        {
                            ProfileImage image = new ProfileImage()
                            {
                                UserId = avatar.Id,
                                Data = Path.Combine(uploads, FileName),
                                Url = $"/api/Users/avatar/get?userId={avatar.Id}"
                            };

                            if (_context.ProfileImages.Count(i => i.UserId == avatar.Id) > 0)
                            {
                                var db_image = _context.ProfileImages.First(i => i.UserId == avatar.Id);
                                db_image.Data = image.Data;
                            }
                            else
                            {
                                _context.ProfileImages.Add(image);
                            }

                            await _context.SaveChangesAsync();

                            using (var fileStream = new FileStream
                            (
                               Path.Combine(fullUploads, FileName),
                               FileMode.Create, FileAccess.Write
                            ))
                            {
                                fileStream.Write(massiv, 0, massiv.Length);
                            }

                            return Ok(new ApiResponse { Status = "success", Message = "avatar is update." });
                        }
                        catch (Exception ex)
                        {
                            return Problem(ex.Message, ex.StackTrace);
                        }
                    }
                    else
                    {
                        return Ok(new ApiResponse { Status = "failed", Message = "file is empty." });
                    }
                }
                else
                {
                    return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
                }
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse { Status = "failed", Message = $"{ex.Message} Stack:{ex.StackTrace}" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(UserAuth userAuth)
        {
            string email = userAuth.Email;
            string passwd = userAuth.Password;

            if (_context.Users == null)
            {
                return Problem("Oops... Something happened to the database");
            }

            if (await _context.Users.CountAsync(u => u.Email == email && u.Password == passwd) > 0)
            {
                var user = await _context.Users
                    .Include("ProfileImage")
                    .FirstAsync(u=> u.Email == email && u.Password == passwd);
                
                return Ok(user);
            }
            else
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
            }

        }

        [HttpPost]
        [Route("registration")]
        public async Task<ActionResult> Registration(UserReg user)
        {
            if (_context.Users == null)
            {               
                return Problem("Oops... Something happened to the database");
            }

            if (_context.Users.Count(u => u.Email == user.Email) > 0)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user with this email is already registered." });
            }
            else
            {
                var new_user = new User()
                {
                    Email = user.Email,
                    Password = user.Password,
                    Name = user.Name
                };

                _context.Users.Add(new_user);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse { Status = "successful", Message = "user has been created." });
            }

        }

        [HttpPost]
        [Route("password/recovery")]
        public async Task<ActionResult> PasswordRecovery(UserRecovery recovery)
        {
            string email = recovery.Email;
            if (_context.Users == null)
            {
                return Problem("Oops... Something happened to the database");
            }

            if (_context.Users.Count(u => u.Email == email) > 0)
            {
                var user = _context.Users.First(u => u.Email == email);
                user.Password = RandomString(6, true);

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse { Status = "successful", Message = "password has been changed." });
            }
            else
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
            }

        }

        [HttpPost]
        [Route("password/change")]
        public async Task<ActionResult> PasswordChange(UserChange inputUser)
        {
            if (_context.Users == null)
            {
                return Problem("Oops... Something happened to the database");
            }

            if (_context.Users.Count(u => u.Id == inputUser.Id) > 0)
            {
                var user = _context.Users.First(u => u.Id == inputUser.Id);
                if(user.Password == inputUser.OldPass)
                {
                    user.Password = inputUser.NewPass;
                    
                    await _context.SaveChangesAsync();

                    return Ok(new ApiResponse { Status = "successful", Message = "password has been changed." });
                }
                else
                {
                    return Ok(new ApiResponse { Status = "failed", Message = "user's password does not match." });
                }
            }
            else
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
            }

        }


        // GET: api/Users/5
        [HttpGet]
        [Route("adv/get")]
        public async Task<ActionResult> GetUserAdvertisements(int userId)
        {
            if (_context.Users == null)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
            }
            var adv = await _context.Advertisements.Where(a => a.UserId == userId).ToListAsync();

            if (adv == null)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user haven't advertisements." });
            }
            else if (adv.Count() > 0)
            {
                return Ok(adv);
            }
            else
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user haven't advertisements." });
            }
        }



        #region Local Methods
        string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }


        //// DELETE: api/Users/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUser(int id)
        //{
        //    if (_context.Users == null)
        //    {
        //        return NotFound();
        //    }
        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Users.Remove(user);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private User? GetUserById(int id)
        {
            return _context.Users?.First(e => e.Id == id);
        }

        private byte[] GetUrlFromBase64(string image64)
        {
            byte[] bytes = Convert.FromBase64String(image64);
            return bytes;
        }

        #endregion
    }
}
