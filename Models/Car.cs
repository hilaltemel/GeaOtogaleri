using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Dealership.Models;
    public class Car
    {
        public long Id { get; set; }
        public bool IsOnSale { get; set; }
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }
        public int CarModelId { get; set; }
        public CarModel? CarModel { get; set; }
        public string? Features { get; set; }
        public string? Color { get; set; }
        public TransmissionType TransmissionType { get; set; }
        public FuelType FuelType { get; set; }
        public decimal? FirstPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public ICollection<CarImage>? Images { get; set; }  // ICollection, EF Core'da Bire-Çok (One-to-Many) ilişkisini kurar; bir arabanın birden fazla resmini tutan bir fotoğraf albümü gibi çalışır.
        public bool IsShowcase { get; set; } // Vitrin Aracı Mı?
    }

