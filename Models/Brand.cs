using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Dealership.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<CarModel>? Models { get; set; }   // Bire-Çok İlişkisi: Bir markanın birden fazla modeli (CarModel) olabilir
    }
}