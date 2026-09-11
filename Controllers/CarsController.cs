using Microsoft.AspNetCore.Mvc;
using Car_Dealership.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Car_Dealership.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Details(int id)
        {
            var car = _context.Cars
                            .Include(c => c.Images)
                            .Include(c => c.Brand)
                            .Include(c => c.CarModel)
                            .FirstOrDefault(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // Müşterilerin göreceği "Araçlar" listesi
        // 1. Parametrelere int? bodyType ve int? drivetrain eklendi
        public IActionResult Index(int? brandId, decimal? minPrice, decimal? maxPrice, int? fuelType, int? transmissionType, int? bodyType, int? drivetrain)
        {
            // Temel Sorgu: Sadece SIFIR ve Satışta olan araçları getir
            var query = _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarModel)
                .Include(c => c.Images)
                .Where(c => c.IsOnSale && !c.IsSecondHand) // SIFIR ARAÇ KONTROLÜ
                .AsQueryable();

            // --- MEVCUT FİLTRELER ---
            if (brandId.HasValue)
                query = query.Where(c => c.BrandId == brandId.Value);

            if (minPrice.HasValue)
                query = query.Where(c => c.CurrentPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(c => c.CurrentPrice <= maxPrice.Value);

            if (fuelType.HasValue)
                query = query.Where(c => (int)c.FuelType == fuelType.Value);

            if (transmissionType.HasValue)
                query = query.Where(c => (int)c.TransmissionType == transmissionType.Value);

            // --- YENİ EKLENEN FİLTRELER BURAYA GELECEK ---
            if (bodyType.HasValue)
                query = query.Where(c => (int)c.BodyType == bodyType.Value);

            if (drivetrain.HasValue)
                query = query.Where(c => (int)c.Drivetrain == drivetrain.Value);

            ViewBag.Brands = _context.Brands.ToList();
            
            // Sonuçları sırala ve View'a gönder
            var cars = query.OrderByDescending(c => c.Id).ToList();

            return View(cars);
        }
        public IActionResult UsedCars(int? brandId, decimal? minPrice, decimal? maxPrice, int? fuelType, int? transmissionType, int? bodyType, int? drivetrain, bool? isTradeInEligible, bool? heavyDamageRecord)
        {
            // Sadece Satışta olan ve İKİNCİ EL olan araçları çek
            var query = _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarModel)
                .Include(c => c.Images)
                .Where(c => c.IsOnSale && c.IsSecondHand)
                .AsQueryable();

            // -- MEVCUT FİLTRELER --
            if (brandId.HasValue)
                query = query.Where(c => c.BrandId == brandId.Value);

            if (minPrice.HasValue)
                query = query.Where(c => c.CurrentPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(c => c.CurrentPrice <= maxPrice.Value);

            if (fuelType.HasValue)
                query = query.Where(c => (int)c.FuelType == fuelType.Value);

            if (transmissionType.HasValue)
                query = query.Where(c => (int)c.TransmissionType == transmissionType.Value);

            // -- YENİ EKLENEN İKİNCİ EL FİLTRELERİ --
            if (bodyType.HasValue)
                query = query.Where(c => (int)c.BodyType == bodyType.Value);

            if (drivetrain.HasValue)
                query = query.Where(c => (int)c.Drivetrain == drivetrain.Value);

            if (isTradeInEligible.HasValue)
                query = query.Where(c => c.IsTradeInEligible == isTradeInEligible.Value);

            if (heavyDamageRecord.HasValue)
                query = query.Where(c => c.HeavyDamageRecord == heavyDamageRecord.Value);

            ViewBag.Brands = _context.Brands.ToList();
            
            // Sonuçları fiyata göre veya eklenme tarihine göre sıralayabilirsin
            var cars = query.OrderByDescending(c => c.Id).ToList();

            return View(cars);
        }
    }
}