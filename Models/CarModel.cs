using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Dealership.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int BrandId { get; set; }    // FOREIGN KEY
        public Brand? Brand { get; set; }    // FOREIGN KEY
        public ICollection<Car>? Cars { get; set; }  // Bire-Çok İlişkisi: Bir modelin birden fazla arabası (ilanı) olabilir
    }
}