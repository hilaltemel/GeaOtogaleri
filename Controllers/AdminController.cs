using Car_Dealership.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Car_Dealership.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/Index
        // Parantez içine 'searchQuery' ekledim. Eğer kimse arama yapmamışsa bu değer null gelecek.
        public IActionResult Index(int? brandId, decimal? minPrice, decimal? maxPrice, int? fuelType, int? transmissionType, int? bodyType, int? drivetrain, bool? isTradeInEligible, bool? heavyDamageRecord)
        {
            var cars = _context.Cars
                            .Include(c => c.Images)
                            .Include(c => c.Brand)
                            .Include(c => c.CarModel)
                            .AsQueryable();

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
            
            // -- YENİ EKLENEN İKİNCİ EL FİLTRELERİ --
            if (bodyType.HasValue)
                cars = cars.Where(c => (int)c.BodyType == bodyType.Value);

            if (drivetrain.HasValue)
                cars = cars.Where(c => (int)c.Drivetrain == drivetrain.Value);

            if (isTradeInEligible.HasValue)
                cars = cars.Where(c => c.IsTradeInEligible == isTradeInEligible.Value);

            if (heavyDamageRecord.HasValue)
                cars = cars.Where(c => c.HeavyDamageRecord == heavyDamageRecord.Value);

            ViewBag.Brands = _context.Brands.ToList();

            return View(cars.OrderByDescending(c => c.Id).ToList());
        }
        // RANDEVULAR LİSTESİ (Sadece Admin Görebilir)
        // 1. RANDEVULAR LİSTESİ VE FİLTRELEME
        public IActionResult Appointments(string searchKeyword, DateTime? filterDate, string filterTime, long? filterCarId, string statusFilter)
        {
            var query = _context.Appointments
                .Include(a => a.Car)
                    .ThenInclude(c => c!.Brand)
                .Include(a => a.Car)
                    .ThenInclude(c => c!.CarModel)
                .AsQueryable();

            // -- ARAMA VE FİLTRELEME İŞLEMLERİ --

            // 1. İsim veya Telefona göre arama
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                var culture = new System.Globalization.CultureInfo("tr-TR");
                string lowerSearch = searchKeyword.ToLower(culture);

                query = query.Where(a => (a.FirstName != null && a.FirstName.ToLower().Contains(lowerSearch)) || 
                                        (a.LastName != null && a.LastName.ToLower().Contains(lowerSearch)) || 
                                        (a.PhoneNumber != null && a.PhoneNumber.ToLower().Contains(lowerSearch)));
            }

            // 2. Tarihe göre filtreleme
            if (filterDate.HasValue)
                query = query.Where(a => a.AppointmentDate.Date == filterDate.Value.Date);

            // 3. Saate göre filtreleme
            if (!string.IsNullOrEmpty(filterTime))
                query = query.Where(a => a.AppointmentTime == filterTime);

            // 4. İlgilenilen Araca göre filtreleme
            if (filterCarId.HasValue)
                query = query.Where(a => a.CarId == filterCarId.Value);

            // 5. Duruma göre filtreleme (Aktif, Geçmiş, İptal Edilmiş, Tümü)
            var today = DateTime.Today;

            if (statusFilter == "cancelled")
            {
                // Sadece iptal edilenler
                query = query.Where(a => a.IsCancelled == true);
            }
            else if (statusFilter == "all")
            {
                // Tümünü getir, hiçbir filtreleme yapma
            }
            else if (statusFilter == "past")
            {
                // Geçmiş Randevular: İptal EDİLMEMİŞ ve tarihi bugünden KÜÇÜK olanlar
                query = query.Where(a => a.IsCancelled == false && a.AppointmentDate.Date < today);
            }
            else
            {
                // Varsayılan (Aktif Randevular): İptal EDİLMEMİŞ ve tarihi bugünden BÜYÜK veya EŞİT olanlar
                query = query.Where(a => a.IsCancelled == false && a.AppointmentDate.Date >= today);
            }
            // View tarafındaki araçlar açılır menüsü (Select) için araba listesini gönderiyoruz
            ViewBag.Cars = _context.Cars.Include(c => c.Brand).Include(c => c.CarModel).ToList();
            
            // Filtrelenmiş sonuçları tarihe göre sıralayıp View'a gönder
            var appointments = query
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToList();

            return View(appointments);
        }

        // 2. RANDEVU İPTAL ETME METODU
        [HttpPost]
        public IActionResult CancelAppointment(int id) 
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null)
            {
                appointment.IsCancelled = true; // Veriyi silmiyoruz, iptal edildi olarak işaretliyoruz
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Randevu başarıyla iptal edildi.";
            }
            
            // İşlem bitince admini aynı sayfaya, filtrelerini bozmadan geri döndür
            return Redirect(Request.Headers["Referer"].ToString() ?? "/Admin/Appointments");
        }
        // GET: Admin/Details/5
        public IActionResult Details(int? id)
        {
            // Eğer linkte bir ID yoksa hata ver
            if (id == null)
            {
                return NotFound();
            }

            // Seçilen arabayı bulurken, Marka, Model ve Resimleri de Include ile sisteme dahil et
            var car = _context.Cars
                .Include(c => c.Images)
                .Include(c => c.Brand)
                .Include(c => c.CarModel)
                .FirstOrDefault(m => m.Id == id);

            // Eğer o ID'ye ait bir araba veritabanında yoksa hata ver
            if (car == null)
            {
                return NotFound();
            }

            // Arabayı buldum, şimdi HTML sayfasına gönderiyoruz
            return View(car);
        }// GET: Admin/Edit/5 (DÜZENLE SAYFASINI İLK AÇAN METOT)
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            // 1. Linkte ID yoksa hata ver
            if (id == null)
            {
                return NotFound();
            }

            // 2. Arabayı mevcut resimleriyle birlikte veritabanından bul
            var car = _context.Cars
                .Include(c => c.Images)
                .FirstOrDefault(c => c.Id == id);

            // Eğer araba bulunamazsa hata ver
            if (car == null)
            {
                return NotFound();
            }

            // 3. MARKALAR LİSTESİ: Tüm markaları gönderiyor ve arabanın mevcut markasını (car.BrandId) "Seçili" yapıyor
            ViewBag.Brands = new SelectList(_context.Brands.ToList(), "Id", "Name", car.BrandId);

            // 4. MODELLER LİSTESİ: Sadece bu arabanın markasına ait modelleri bulup, arabanın modelini "Seçili" yapıyor
            var carModels = _context.CarModels.Where(m => m.BrandId == car.BrandId).ToList();
            ViewBag.Models = new SelectList(carModels, "Id", "Name", car.CarModelId);

            // Arabanın tüm dolu verilerini View (HTML) tarafına yolluyor
            return View(car);
        }
        // POST: Admin/Edit/5 (FORM GÖNDERİLDİĞİNDE ÇALIŞACAK METOT)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Car car, List<IFormFile>? newImages)
        {
            ModelState.Remove("FirstPrice");
            // Güvenlik kontrolü: Linkteki ID ile formdan gelen gizli ID eşleşiyor mu?
            if (id != car.Id)
            {
                return NotFound();
            }

            // 1. Veritabanındaki orijinal arabayı mevcut resimleriyle birlikte bul
            var existingCar = await _context.Cars
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCar == null)
            {
                return NotFound();
            }

            // 2. Arabanın temel bilgilerini formdan gelen yeni bilgilerle güncelle
            existingCar.BrandId = car.BrandId;
            existingCar.CarModelId = car.CarModelId;
            existingCar.Color = car.Color;
            existingCar.TransmissionType = car.TransmissionType;
            existingCar.FuelType = car.FuelType;

            if (existingCar.FirstPrice == 0)
            {
                existingCar.FirstPrice = car.CurrentPrice; 
            }

            // Yöneticinin ekrandan girdiği yeni fiyatı güncelle
            existingCar.CurrentPrice = car.CurrentPrice;
            existingCar.IsOnSale = car.IsOnSale;
            existingCar.IsShowcase = car.IsShowcase;
            
            // --- YENİ EKLENEN ORTAK ÖZELLİKLER ---
            existingCar.IsUsedCar = car.IsUsedCar;
            existingCar.Series = car.Series;
            existingCar.Year = car.Year;
            existingCar.EnginePower = car.EnginePower;
            existingCar.EngineCapacity = car.EngineCapacity;
            existingCar.BodyType = car.BodyType;
            existingCar.Drivetrain = car.Drivetrain;
            existingCar.HasWarranty = car.HasWarranty;

            // --- YENİ EKLENEN İKİNCİ EL ÖZELLİKLERİ ---
            existingCar.Mileage = car.Mileage;
            existingCar.Plate = car.Plate;
            existingCar.PaintAndChangedStatus = car.PaintAndChangedStatus;
            existingCar.InspectionDate = car.InspectionDate;
            existingCar.HeavyDamageRecord = car.HeavyDamageRecord;
            existingCar.IsTradeInEligible = car.IsTradeInEligible;

            // 3. YENİ RESİM YÜKLENDİYSE İŞLE 
            if (newImages != null && newImages.Count > 0)
            {
                // A. ESKİ RESİMLERİ SİL
                if (existingCar.Images != null && existingCar.Images.Any())
                {
                    foreach (var oldImage in existingCar.Images.ToList())
                    {
                        // 1. GÜVENLİK: Sadece veritabanında gerçekten bir resim yolu varsa klasöre git
                        if (!string.IsNullOrEmpty(oldImage.ImagePath))
                        {
                            // Sunucudaki (wwwroot/images) fiziksel dosyayı bul ve sil
                            string oldImagePath = Path.Combine(_environment.WebRootPath, "images", oldImage.ImagePath);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        
                        // 2. Veritabanından da kaydı her halükarda sil
                        _context.Remove(oldImage);
                    }
                    // Arabanın mevcut resim listesini tamamen temizle
                    existingCar.Images.Clear();
                }

                // B. YENİ RESİMLERİ EKLE
                if (existingCar.Images == null)
                {
                    existingCar.Images = new List<CarImage>();
                }

                foreach (var file in newImages)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string imagePath = Path.Combine(_environment.WebRootPath, "images", fileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    existingCar.Images.Add(new CarImage 
                    { 
                        ImagePath = fileName,
                        CarId = existingCar.Id
                    });
                }
            }

            // 4. DEĞİŞİKLİKLERİ MÜHÜRLE
            await _context.SaveChangesAsync();

            // 5. İşlem başarıyla bitince listeye geri dön
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult GetModelsByBrand(int brandId)
        {
            // Veritabanından sadece seçilen markanın (brandId) modellerini bul
            var models = _context.CarModels
                .Where(m => m.BrandId == brandId)
                .Select(m => new { value = m.Id, text = m.Name }) // Sadece ID ve İsimlerini al ki veri hafif olsun
                .ToList();

            // Bu listeyi JavaScript'in anlayacağı JSON formatında paketleyip geri gönder
           return Json(models);
        }
        // GET: Admin/Create (SAYFAYI İLK AÇAN METOT)
        [HttpGet]
        public IActionResult Create()
        {
            var brands = _context.Brands.ToList();
            ViewBag.Brands = new SelectList(brands, "Id", "Name");
            ViewBag.Models = new SelectList(new List<CarModel>(), "Id", "Name");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Car car, List<IFormFile>? images)
        {
            // 1. ÖNCE ARABAYI KAYDET (ID Oluşması İçin)
            car.FirstPrice = car.CurrentPrice;
            _context.Cars.Add(car);
            _context.SaveChanges();
            //car.Id al

            // 2. EĞER RESİMLER GELDİYSE ONLARI İŞLE
            if (images != null && images.Count > 0)
            {
                foreach (var file in images)
                {
                    // Benzersiz isim oluştur
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string imagePath = Path.Combine(_environment.WebRootPath, "images", fileName);

                    // Resmi klasöre kopyala
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Resmi veritabanındaki CarImages tablosuna ekle ve Arabaya bağla
                    var carImage = new CarImage 
                    { 
                        ImagePath = fileName, 
                        CarId = car.Id 
                    };
                    _context.CarImages.Add(carImage);
                }
                
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        // 1. GET: Admin/Delete/5 (SİLME ONAY SAYFASINI AÇ)
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Kullanıcıya neyi sildiğini göstermek için Marka ve Model bilgisiyle aracı bul
            var car = await _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarModel)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // 2. POST: Admin/Delete/5 (SİLME İŞLEMİNİ GERÇEKLEŞTİRİR)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Silinecek aracı resimleriyle birlikte getir
            var car = await _context.Cars
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car != null)
            {
                // A. ÖNCE SUNUCUDAKİ FİZİKSEL RESİMLERİ TEMİZLE
                if (car.Images != null && car.Images.Any())
                {
                    foreach (var image in car.Images)
                    {
                        if (!string.IsNullOrEmpty(image.ImagePath))
                        {
                            string imagePath = Path.Combine(_environment.WebRootPath, "images", image.ImagePath);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }
                    }
                }

                // B. SONRA ARABAYI VERİTABANINDAN SİL
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }

            // İşlem bitince listeye dön
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult AddBrand(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Marka adı boş olamaz.");
            
            // Veritabanında bu marka adından var mı kontrolü (Büyük/küçük harf duyarsız)
            string formattedName = name.Trim();
            if (_context.Brands.Any(b => b.Name == formattedName))
            {
                return BadRequest("Bu marka sistemde zaten kayıtlı!");
            }
            
            var brand = new Brand { Name = name.Trim() };
            _context.Brands.Add(brand);
            _context.SaveChanges();
            
            return Json(new { id = brand.Id, name = brand.Name });
        }

        [HttpPost]
        public IActionResult AddModel(int brandId, string name)
        {
            if (string.IsNullOrWhiteSpace(name) || brandId <= 0) return BadRequest("Geçersiz veri.");
            
            // YENİ: Seçili marka altında bu modelden zaten var mı kontrolü
            string formattedModelName = name.Trim();

            if (_context.CarModels.Any(m => m.BrandId == brandId && m.Name == formattedModelName))
            {
                return BadRequest("Bu model, seçili marka altında zaten kayıtlı!");
            }
            
            var model = new CarModel { BrandId = brandId, Name = name.Trim() };
            _context.CarModels.Add(model);
            _context.SaveChanges();
            
            return Json(new { id = model.Id, name = model.Name });
        }
    }
}