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
        public string? Color { get; set; }
        public TransmissionType TransmissionType { get; set; }
        public FuelType FuelType { get; set; }
        public decimal? FirstPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public ICollection<CarImage>? Images { get; set; }  // ICollection, EF Core'da Bire-Çok (One-to-Many) ilişkisini kurar; bir arabanın birden fazla resmini tutan bir fotoğraf albümü gibi çalışır.
        public bool IsShowcase { get; set; } // Vitrin Aracı Mı?
        public bool IsSecondHand { get; set; }
        public string? Series { get; set; } 
        public int Year { get; set; }
        public short EnginePower { get; set; }
        public short EngineCapacity { get; set; }
        public BodyType BodyType { get; set; }
        public Drivetrain Drivetrain { get; set; }
        public bool HasWarranty { get; set; }
        public int? Mileage { get; set; }
        public bool? HeavyDamageRecord { get; set; }
        public string? Plate { get; set; }
        public string? PaintAndChangedStatus { get; set; }
        public DateTime? InspectionDate { get; set; }
        public bool? IsTradeInEligible { get; set; }
    }

