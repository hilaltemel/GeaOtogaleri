using System;
using System.ComponentModel.DataAnnotations;

namespace Car_Dealership.Models
{
    public class Favorite
    {
        [Key]
        public long Id { get; set; }

        // Hangi kullanıcı favoriye ekledi?
        public long UserId { get; set; }
        public User? User { get; set; }

        // Hangi arabayı favoriye ekledi?
        public long CarId { get; set; }
        public Car? Car { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}