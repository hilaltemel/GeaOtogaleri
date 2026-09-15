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
                .Where(c => c.IsOnSale && !c.IsUsedCar) // SIFIR ARAÇ KONTROLÜ
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

            // --- FAVORİ KONTROLÜ (Giriş yapan kullanıcının favori araba ID'lerini bulup ViewBag'e atıyoruz) ---
            List<long> userFavIds = new List<long>();
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdStr))
                {
                    long currentUserId = long.Parse(userIdStr);
                    userFavIds = _context.Favorites
                        .Where(f => f.UserId == currentUserId)
                        .Select(f => f.CarId)
                        .ToList();
                }
            }
            ViewBag.UserFavIds = userFavIds;
            return View(cars);
        }
        public IActionResult UsedCars(int? brandId, decimal? minPrice, decimal? maxPrice, int? fuelType, int? transmissionType, int? bodyType, int? drivetrain, bool? isTradeInEligible, bool? heavyDamageRecord)
        {
            // Sadece Satışta olan ve İKİNCİ EL olan araçları çek
            var query = _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarModel)
                .Include(c => c.Images)
                .Where(c => c.IsOnSale && c.IsUsedCar)
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
            
            List<long> userFavIds = new List<long>();
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdStr))
                {
                    long currentUserId = long.Parse(userIdStr);
                    userFavIds = _context.Favorites
                        .Where(f => f.UserId == currentUserId)
                        .Select(f => f.CarId)
                        .ToList();
                }
            }
            ViewBag.UserFavIds = userFavIds;

            return View(cars);
        }
       [HttpPost]
        public IActionResult ToggleFavorite(long carId)
        {
            // 1. Kullanıcı giriş yapmış mı kontrol et
            if (!User.Identity!.IsAuthenticated)
            {
                return Json(new { success = false, message = "Favoriye eklemek için lütfen giriş yapın." });
            }

            // 2. Yaka kartındaki ID'yi al
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdString))
            {
                return Json(new { success = false, message = "Kullanıcı kimliği doğrulanamadı." });
            }

            long currentUserId = long.Parse(userIdString);

            // 3. Veritabanı işlemleri
            var existingFavorite = _context.Favorites
                .FirstOrDefault(f => f.CarId == carId && f.UserId == currentUserId);

            bool isAdded = false;

            if (existingFavorite != null)
            {
                _context.Favorites.Remove(existingFavorite);
            }
            else
            {
                var newFavorite = new Favorite
                {
                    CarId = carId,
                    UserId = currentUserId,
                    AddedAt = DateTime.Now
                };
                _context.Favorites.Add(newFavorite);
                isAdded = true;
            }

            _context.SaveChanges();

            return Json(new { success = true, isFavorite = isAdded });
        }
        public IActionResult Favorites()
        {
            // 1. Kullanıcı giriş yapmış mı kontrol et, yapmadıysa Login sayfasına yönlendir
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Oturum açan kullanıcının ID'sini yaka kartından (Claim) al
            var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Account");
            }
            long currentUserId = long.Parse(userIdString);

            // 3. TABLOLARI BAĞLAMA (JOIN MANTIĞI) VE FİLTRELEME:
            // "Favorites tablosuna git, sadece giriş yapan kullanıcıya ait (UserId == currentUserId) olan kayıtları seç. 
            // Bu kayıtların içindeki arabaları (Car), onların Markasını, Modelini ve Resimlerini de yanına al."
            var favoriteCars = _context.Favorites
                .Where(f => f.UserId == currentUserId)
                .Include(f => f.Car)
                    .ThenInclude(c => c!.Brand)
                .Include(f => f.Car)
                    .ThenInclude(c => c!.CarModel)
                .Include(f => f.Car)
                    .ThenInclude(c => c!.Images)
                .Select(f => f.Car) // Sadece araba objelerini seçip listeye dönüştürüyoruz
                .Where(c => c != null && c.IsOnSale) // Satışta olanları filtrele
                .ToList();

            // 4. Bulunan favori araçları HTML sayfasına (View) gönder
            return View(favoriteCars);
        }
        [HttpGet]
        public IActionResult Search(string searchQuery)
        {
            // 1. Eğer kullanıcı boş arama yaparsa, onu geldiği gibi ana sayfaya geri gönder
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return RedirectToAction("Index", "Home");
            }

            // 2. Gelen kelimeyi küçük harfe çevir (Büyük/Küçük harf hassasiyetini yok etmek için)
            var lowerSearch = searchQuery.ToLower();

            // 3. Marka adında VEYA Model adında bu kelime geçen arabaları bul
            var searchResults = _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarModel)
                .Include(c => c.Images)
                .Where(c => 
                    (c.Brand != null && c.Brand.Name!.ToLower().Contains(lowerSearch)) ||
                    (c.CarModel != null && c.CarModel.Name!.ToLower().Contains(lowerSearch))
                )
                .ToList();

            // 4. FAVORİ KALPLERİ İÇİN HAFIZA KODUMUZ (Eski metotlardan aynısı)
            List<long> userFavIds = new List<long>();
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdStr))
                {
                    long currentUserId = long.Parse(userIdStr);
                    userFavIds = _context.Favorites
                        .Where(f => f.UserId == currentUserId)
                        .Select(f => f.CarId)
                        .ToList();
                }
            }
            ViewBag.UserFavIds = userFavIds;
            
            // Ekranda "Şunu aradınız" yazabilmek için arama kelimesini ViewBag ile HTML'e gönder
            ViewBag.SearchQuery = searchQuery; 

            return View(searchResults);
        }
    }
}