using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace ApiForKwork.Models
{
    #region Data for Controller

    public class UserAvatar
    {
        public int Id { get; set; }
        public string Avatar { get; set; } = string.Empty;
    }

    public class UserAuth
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class UserReg
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class UserChange
    {
        public int Id { get; set; }
        public string OldPass { get; set; } = string.Empty;
        public string NewPass { get; set; } = string.Empty;
    }

    public class UserRecovery
    {
        public string Email { get; set; } = string.Empty;
    }

    public class AdvertisementAdd
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CityId { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public string MainImage { get; set; } = string.Empty;
        public double latitude { get; set; }
        public double longitude { get; set; }
        public List<AdvertisementImageAdd> Images { get; set; } = new List<AdvertisementImageAdd>();
    }

    public class AdvertisementUpd
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CityId { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public string MainImage { get; set; } = string.Empty;
          public double latitude { get; set; }
        public double longitude { get; set; }
        public List<AdvertisementImageAdd> Images { get; set; } = new List<AdvertisementImageAdd>();
    }

    public class AdvertisementImageAdd
    {
        public string Image { get; set; } = string.Empty;
    }

    public class MessageSend
    {
        public int SenderId { get; set; }
        public int ReciverId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }


    #endregion

    #region DbEntity
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ProfileImage? ProfileImage { get; set; }
        public List<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

    }

    public class ProfileImage
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Required foreign key property
        [JsonIgnore]
        public User User { get; set; } = null!; // Required reference navigation to principal
        [JsonIgnore]
        public string Data { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
    

    public class ApiResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class City
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

    }

    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Advertisement> Advertisements { get; set; } = new List<Advertisement>();

    }

    public class Advertisement
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int CityId { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        [JsonIgnore]
        public string MainImageData { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<AdvertisementImage> Images { get; set; } = new List<AdvertisementImage>();
    }

    public class AdvertisementImage
    {
        public int Id { get; set; }
        public int AdvertisementId { get; set; }
        [JsonIgnore]
        public int Index { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [JsonIgnore]
        public string ImageData { get; set; } = string.Empty;

    }
    public class UserMessageList
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReciverId { get; set; }
    }
        public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReciverId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsReaded { get; set; }
        public bool IsDeleted { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
    #endregion
}