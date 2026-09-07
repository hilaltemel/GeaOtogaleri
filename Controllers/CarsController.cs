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
        public IActionResult Index(int? brandId, decimal? minPrice, decimal? maxPrice, int? fuelType, int? transmissionType)
        {
            // 1. Tüm araçları sorgulanabilir şekilde al
            var cars = _context.Cars
                            .Include(c => c.Images)
                            .Include(c => c.Brand)
                            .Include(c => c.CarModel)
                            .AsQueryable();

            // 2. Filtreler dolu geldiyse sorguya şartları ekle
            if (brandId.HasValue)
                cars = cars.Where(c => c.BrandId == brandId.Value);
            
            if (minPrice.HasValue)
                cars = cars.Where(c => c.CurrentPrice >= minPrice.Value);
                
            if (maxPrice.HasValue)
                cars = cars.Where(c => c.CurrentPrice <= maxPrice.Value);

            if (fuelType.HasValue)
                cars = cars.Where(c => (int)c.FuelType == fuelType.Value);

            if (transmissionType.HasValue)
                cars = cars.Where(c => (int)c.TransmissionType == transmissionType.Value);

            // 3. Markaları filtreleme menüsü için sayfaya gönder
            ViewBag.Brands = _context.Brands.ToList();

            // 4. Sonuçları listeye çevir ve View'a gönder
            return View(cars.OrderByDescending(c => c.Id).ToList());
        }
    }
}