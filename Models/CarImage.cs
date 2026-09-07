using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Dealership.Models
{
    public class CarImage
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; }  // Resmin klasördeki adını tutacak
        public long CarId { get; set; }  // FOREIGN KEY: Bu resim hangi arabaya ait?
        public Car? Car { get; set; }   // EF Core'un tabloları birbirine bağlaması için gereken sihirli köprü
    }
}