using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiForKwork.Models;
using ApiForKwork.SqlDbContext;

namespace ApiForKwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdvertController(MyDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Advertisements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Advertisement>> GetAdvertisement(int id)
        {
            if (_context.Advertisements == null)
            {
                return NotFound();
            }
            else if (_context.Advertisements.Count() == 0)
            {
                return Problem("Oops... Something happened to the database");
            }
            else
            {
                var advertisement = await _context.Advertisements
                .Include("Images")
                .FirstAsync(u => u.Id == id);

                if (advertisement == null)
                {
                    return NotFound();
                }

                return advertisement;
            }
        }

        [HttpGet]
        [Route("image/get")]
        public async Task<ActionResult> GetAdvertisementMainImage(int advId)
        {
            if (_context.Advertisements == null)
            {
                return NotFound();
            }
            var mainImage = await _context.Advertisements.FirstOrDefaultAsync(a => a.Id == advId);

            if (mainImage == null)
            {
                return NotFound();
            }
            else
            {
                byte[] image = System.IO.File.ReadAllBytes(Path.Combine(_environment.ContentRootPath, mainImage.MainImageData));
                return File(image, "image/png");
            }
        }

        [HttpGet]
        [Route("image/list")]
        public async Task<IEnumerable<ActionResult>> GetAdvertisementImages(int advId)
        {
            if (_context.Advertisements == null)
            {
                return new NotFoundResult[] { NotFound() };
            }

            var images = await _context.AdvertisementImages.Where(a => a.AdvertisementId == advId).ToListAsync();

            if (images == null)
            {
                return new NotFoundResult[] { NotFound() };
            }
            else
            {
                List<FileContentResult> files = new List<FileContentResult>();
                foreach (var item in images)
                {
                    byte[] image = System.IO.File.ReadAllBytes(Path.Combine(_environment.ContentRootPath, item.ImageData));
                    //File(image, "image/png");
                    files.Add(File(image, "image/png"));
                }

                return files.ToArray();
            }
        }


        [HttpGet]
        [Route("image/list/get")]
        public async Task<ActionResult> GetAdvertisementImages(int advId, int index)
        {
            if (_context.Advertisements == null)
            {
                return NotFound();
            }

            var mainImage = await _context.AdvertisementImages.FirstOrDefaultAsync(a => a.AdvertisementId == advId && a.Index == index);

            if (mainImage == null)
            {
                return NotFound();
            }
            else
            {
                byte[] image = System.IO.File.ReadAllBytes(Path.Combine(_environment.ContentRootPath, mainImage.ImageData));
                return File(image, "image/png");
            }
        }

        // POST: api/Advertisements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> PostAdvertisementAdd(AdvertisementAdd adv)
        {
            if (_context.Advertisements == null)
            {
                return Problem("Oops... Something happened to the database");
            }

            if (_context.Users.Count(u => u.Id == adv.UserId) > 0)
            {
                var new_adv = new Advertisement()
                {
                    UserId = adv.UserId,
                    CategoryId = adv.CategoryId,
                    CityId = adv.CityId,
                    Description = adv.Description,
                    Title = adv.Title,
                    Price = adv.Price,
                    Latitude = adv.latitude, Longitude = adv.longitude,
                };

                _context.Advertisements.Add(new_adv);
                var count = await _context.SaveChangesAsync();

                if (count > 0)
                {
                    var imgData = await SaveImage(new_adv.Id, true, 0, GetUrlFromBase64(adv.MainImage));
                    new_adv.MainImageUrl = imgData[0];
                    new_adv.MainImageData = imgData[1];
                }

                _context.Entry(new_adv).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                new_adv.Images = new List<AdvertisementImage>();
                int index = 0;
                foreach (var item in adv.Images)
                {
                    var imgData = await SaveImage(new_adv.Id, false, index, GetUrlFromBase64(item.Image));
                    new_adv.Images.Add(new AdvertisementImage
                    {
                        AdvertisementId = new_adv.Id,
                        ImageUrl = imgData[0],
                        ImageData = imgData[1],
                        Index = index
                    });
                    index++;
                }

                _context.Entry(new_adv).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse { Status = "successful", Message = "advert has been created." });
            }
            else
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> PutAdvertisement(AdvertisementUpd adv)
        {
            if (_context.Advertisements == null)
            {
                return Problem("Oops... Something happened to the database");
            }

            if (_context.Advertisements.Count(u => u.Id == adv.Id) > 0)
            {
                try
                {
                    var upd_adv = _context.Advertisements.Include("Images").First(u => u.Id == adv.Id);

                    upd_adv.UserId = adv.UserId;
                    upd_adv.CategoryId = adv.CategoryId;
                    upd_adv.CityId = adv.CityId;
                    upd_adv.Description = adv.Description;
                    upd_adv.Title = adv.Title;
                    upd_adv.Price = adv.Price;

                    var imgData = await SaveImage(upd_adv.Id, true, 0, GetUrlFromBase64(adv.MainImage));

                    upd_adv.MainImageUrl = imgData[0];
                    upd_adv.MainImageData = imgData[1];


                    await _context.SaveChangesAsync();



                    foreach (var item in upd_adv.Images)
                    {
                        _context.AdvertisementImages.Remove(item);
                    }
                    upd_adv.Images.Clear();


                    upd_adv.Images = new List<AdvertisementImage>();
                    int index = 0;
                    foreach (var item in adv.Images)
                    {
                        var subImgData = await SaveImage(upd_adv.Id, false, index, GetUrlFromBase64(item.Image));
                        upd_adv.Images.Add(new AdvertisementImage
                        {
                            AdvertisementId = upd_adv.Id,
                            ImageUrl = subImgData[0],
                            ImageData = subImgData[1]
                        });
                    }

                    await _context.SaveChangesAsync();

                    return Ok(new ApiResponse { Status = "successful", Message = "advert has been updated." });
                }
                catch (Exception ex)
                {
                    return Ok(new ApiResponse { Status = "failed", Message = $"{ex.Message} StackTrace:{ex.StackTrace}" });
                }
            }
            else
            {
                return Ok(new ApiResponse { Status = "failed", Message = "user not found." });
            }
        }

        [HttpPost]
        [Route("delete")]
        public async Task<ActionResult> DeleteAdvertisement(int userId, int advId)
        {
            if (_context.Advertisements == null)
            {
                return Problem("Oops... Something happened to the database");
            }

            if (AdvertExists(advId))
            {
                var adv = await _context.Advertisements.FindAsync(advId);

                if (adv == null)
                {
                    return Ok(new ApiResponse { Status = "failed", Message = "advert not found." });
                }

                if (adv.UserId == userId)
                {
                    _context.Advertisements.Remove(adv);
                    await _context.SaveChangesAsync();

                    return Ok(new ApiResponse { Status = "successful", Message = "advert has been removed." });
                }
                else
                {
                    return Ok(new ApiResponse { Status = "failed", Message = "permission denied." });
                }
            }
            else
            {
                return Ok(new ApiResponse { Status = "failed", Message = "advert not found." });
            }
        }

        [HttpPost]
        [Route("moderate")]
        public async Task<ActionResult> ModerateAdvertisement(int userId, int advId)
        {
            if (_context.Advertisements == null)
            {
                return Problem("Oops... Something happened to the database");
            }

            if (AdvertExists(advId))
            {
                var adv = await _context.Advertisements.FindAsync(advId);

                if (adv == null)
                {
                    return Ok(new ApiResponse { Status = "failed", Message = "advert not found." });
                }

                var user = await _context.Users.FindAsync(userId);

                if (user.Email == "admin@site.com")
                {
                    _context.Advertisements.Remove(adv);
                    await _context.SaveChangesAsync();

                    return Ok(new ApiResponse { Status = "successful", Message = "advert has been removed." });
                }
                else
                {
                    return Ok(new ApiResponse { Status = "failed", Message = "permission denied." });
                }
            }
            else
            {
                return Ok(new ApiResponse { Status = "failed", Message = "advert not found." });
            }
        }


        // GET: api/Users/5
        [HttpGet]
        [Route("cities/list")]
        public async Task<ActionResult> GetAllCities()
        {
            if (_context.Cities == null)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "cities is empty." });
            }
            var cities = await _context.Cities.ToListAsync();

            if (cities == null)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "cities is empty." });
            }
            else if (cities.Count() > 0)
            {
                return Ok(cities);
            }
            else
            {
                return Ok(new ApiResponse { Status = "failed", Message = "cities is empty." });
            }
        }

        // GET: api/Users/5
        [HttpGet]
        [Route("categories/list")]
        public async Task<ActionResult> GetAllCategories()
        {
            if (_context.Cities == null)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "categories is empty." });
            }
            var catecories = await _context.Categories.ToListAsync();

            if (catecories == null)
            {
                return Ok(new ApiResponse { Status = "failed", Message = "categories is empty." });
            }
            else if (catecories.Count() > 0)
            {
                return Ok(catecories);
            }
            else
            {
                return Ok(new ApiResponse { Status = "failed", Message = "categories is empty." });
            }
        }

     
        [HttpGet]
        [Route("filter")]
        public async Task<ActionResult<IEnumerable<Advertisement>>> GetAdvertisement(string title = "null", int minprice = -1,int maxprice = -1, int page = -1, int cityId = -1, int categoryId = -1, double minlatitude = -1, double minlongitude = -1, double maxlatitude = -1, double maxlongitude = -1)
        {
            if (_context.Advertisements == null)
            {
                return Problem("Oops... Something happened to the database");
            }
            else if (_context.Advertisements.Count() == 0)
            {
                return Problem("Oops... Something happened to the database");
            }
            else
            {
                IQueryable<Advertisement> query = _context.Advertisements.Include("Images");
                if (title != "null")
                {
                    query = query.Where(adv => adv.Title.Contains(title));

                }
                if (cityId > 0)
                {
                    query = query.Where(adv => adv.CityId == cityId);
                }
                if (minprice!=-1 )
                {
                    query = query.Where(adv => adv.Price > minprice );
                }
                if(maxprice != -1)
                {
                    query = query.Where(adv => adv.Price < maxprice);

                }
                if (categoryId > 0)
                {
                    query = query.Where(adv => adv.CategoryId == categoryId);
                }

                if (minlatitude != -1 && minlongitude != -1 && maxlatitude != -1 && maxlongitude != -1)
                {
                    query = query.Where(adv =>
                        adv.Latitude > minlatitude &&
                        adv.Latitude < maxlatitude &&
                        adv.Longitude > minlongitude &&
                        adv.Longitude < maxlongitude);
                }

                if (page > 1)
                {
                    query = query.Skip((page - 1) * 10);
                }

                var filter = await query.Take(10).ToListAsync();
                return filter;
            }
        

    }
        [HttpGet]
        [Route("filter/category")]
        public async Task<ActionResult<IEnumerable<Advertisement>>> FilterCategories(int categoryGroup = 1, int page =1)
        {
            if (categoryGroup < 1 || categoryGroup > 10)
            {
                return BadRequest("Category group must be between 1 and 10.");
            }

         
            IQueryable<Advertisement> query = _context.Advertisements.Include("Images");
            switch (categoryGroup)
            {
                
                case 1:
                    query = query.Where(adv => adv.CategoryId >= 150 && adv.CategoryId <= 153);
                    break;
                case 2:
                    query = query.Where(adv => adv.CategoryId >= 154 && adv.CategoryId <= 158);
                    break;
                case 3:
                    query = query.Where(adv => adv.CategoryId >= 159 && adv.CategoryId <= 166);
                    break;
                case 4:
                    query = query.Where(adv => adv.CategoryId >= 167 && adv.CategoryId <= 168);
                    break;
                case 5:
                    query = query.Where(adv => adv.CategoryId >= 169 && adv.CategoryId <= 175);
                    break;
                case 6:
                    query = query.Where(adv => adv.CategoryId >= 176 && adv.CategoryId <= 177);
                    break;
                case 7:
                    query = query.Where(adv => adv.CategoryId >= 178 && adv.CategoryId <= 181);
                    break;
                case 8:
                    query = query.Where(adv => adv.CategoryId >= 182 && adv.CategoryId <= 183);
                    break;
                case 9:
                    query = query.Where(adv => adv.CategoryId >= 184 && adv.CategoryId <= 184);
                    break;
                case 10:
                    query = query.Where(adv => adv.CategoryId >= 185 && adv.CategoryId <= 185);
                    break;

                default:
                    return BadRequest("Invalid category group.");
            }

        
            if (page > 1)
            {
                query = query.Skip((page - 1) * 10);
            }

            var filter = await query.Take(10).ToListAsync();
            return filter;
        }

        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<IEnumerable<Advertisement>>> GetAdvertisement(string title, int page = 0)
        {
            if (_context.Advertisements == null)
            {
                return Problem("Oops... Something happened to the database");
            }
            else if (_context.Advertisements.Count() == 0)
            {
                return Problem("Oops... Something happened to the database");
            }
            else
            {
                if (title.Length > 3)
                {
                    var filter = await _context.Advertisements.Include("Images").Where(
                        adv => adv.Title.Contains(title)).Skip(page * 10).Take(10).ToListAsync();

                    return filter;
                }
                else
                {
                    return Ok(new ApiResponse { Status = "failed", Message = "to many letter." });
                }
            }
        }


        private async Task<string[]> SaveImage(int advId, bool isMain, int index, byte[] image)
        {
            var uploads = "uploads";
            string FileName = "";

            if (isMain)
            {
                uploads = Path.Combine(uploads, "adv");
                FileName = Path.Combine(uploads, $"adv{advId}_{Guid.NewGuid()}.png");

            }
            else
            {
                uploads = Path.Combine(uploads, "advList");
                FileName = Path.Combine(uploads, $"adv{advId}_{index}_{Guid.NewGuid()}.png");
            }

            var fullUploads = Path.Combine(_environment.ContentRootPath, FileName);

            using (var fileStream = new FileStream
            (
               fullUploads,
               FileMode.Create, FileAccess.Write
            ))
            {
                await fileStream.WriteAsync(image, 0, image.Length);
            }

            if (isMain) return new string[] { $"api/advert/image/get?advId={advId}", FileName };
            else return new string[] { $"api/advert/image/list/get?advId={advId}&index={index}", FileName };
        }

        private byte[] GetUrlFromBase64(string image64)
        {
            byte[] bytes = Convert.FromBase64String(image64);
            return bytes;
        }

        private bool AdvertExists(int id)
        {
            return (_context.Advertisements?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
