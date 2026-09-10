using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Car_Dealership.Models;
using Microsoft.EntityFrameworkCore;

namespace Car_Dealership.Controllers;

public class HomeController : Controller
{
   private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context; // Veritabanı değişkenini tanımlıyoruz

    // Constructor içine ApplicationDbContext'i ekleyip _context'e atıyoruz
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult Campaigns()
    {
        // Sadece CurrentPrice değeri FirstPrice değerinden küçük olanları alıyoruz
        var campaignCars = _context.Cars
                                .Include(c => c.Images)
                                .Include(c => c.Brand)
                                .Include(c => c.CarModel)
                                .Where(c => c.CurrentPrice < c.FirstPrice)
                                .ToList();

        return View(campaignCars);
    }

    public IActionResult Index()
    {
        var showcaseCars = _context.Cars
                                .Include(c => c.Images)
                                .Include(c => c.Brand)
                                .Include(c => c.CarModel)
                                .Where(c => c.IsShowcase)
                                .ToList();
        
        var latestCars = _context.Cars
                                .Include(c => c.Images)
                                .Include(c => c.Brand)
                                .Include(c => c.CarModel)
                                .OrderByDescending(c => c.Id)
                                .Take(4)
                                .ToList();

        var campaignCars = _context.Cars
                                .Include(c => c.Brand)
                                .Include(c => c.CarModel)
                                .Include(c => c.Images)
                                .Where(c => c.FirstPrice != null && c.FirstPrice.Value > c.CurrentPrice) 
                                .OrderByDescending(c => c.FirstPrice!.Value - c.CurrentPrice) 
                                .Take(4)
                                .ToList();
        var usedCars = _context.Cars
                                .Include(c => c.Images)
                                .Include(c => c.Brand)
                                .Include(c => c.CarModel)
                                .Where(c => c.IsSecondHand == true) // Sadece ikinci el olanlar
                                .OrderByDescending(c => c.Id)
                                .Take(4)
                                .ToList();

        // 2. ViewBag Atamaları (Mevcutlara ek olarak)
        ViewBag.Showcase = showcaseCars;
        ViewBag.NewArrivals = latestCars;
        ViewBag.Campaigns = campaignCars;
        ViewBag.UsedCars = usedCars; // Yeni eklediğimiz kısım
        ViewData["LatestCars"] = latestCars;
        
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    // 1. RANDEVUYU VERİTABANINA KAYDETME METODU
    [HttpPost]
    public IActionResult BookAppointment(Appointment model)
    {
        var secilenArabaId = Request.Form["CarId"].ToString();
        if (!string.IsNullOrEmpty(secilenArabaId))
        {
            model.CarId = Convert.ToInt64(secilenArabaId);
        }
        if (ModelState.IsValid)
        {
            // Çifte Randevu Kontrolü: Aynı tarihte ve aynı saatte başka bir randevu var mı?
            bool isBooked = _context.Appointments.Any(a => 
                a.AppointmentDate.Date == model.AppointmentDate.Date && 
                a.AppointmentTime == model.AppointmentTime);

            if (isBooked)
            {
                TempData["ErrorMessage"] = "Seçtiğiniz tarih ve saat için maalesef randevu doludur. Lütfen başka bir saat seçiniz.";
                return Redirect(Request.Headers["Referer"].ToString()); // Kullanıcıyı olduğu sayfada tutar
            }

            // Randevu müsaitse kaydet
            _context.Appointments.Add(model);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Randevunuz başarıyla oluşturuldu! Müşteri temsilcilerimiz sizinle iletişime geçecektir.";
            return Redirect(Request.Headers["Referer"].ToString());
        }
        
        TempData["ErrorMessage"] = "Lütfen formdaki zorunlu alanları eksiksiz doldurunuz.";
        return Redirect(Request.Headers["Referer"].ToString());
    }

    // 2. JAVASCRIPT İÇİN DOLU SAATLERİ GETİREN API METODU
    [HttpGet]
    public IActionResult GetBookedTimes(DateTime date)
    {
        // Seçilen tarihteki dolu olan saatlerin listesini döndürür
        var bookedTimes = _context.Appointments
            .Where(a => a.AppointmentDate.Date == date.Date)
            .Select(a => a.AppointmentTime)
            .ToList();

        return Json(bookedTimes);
    }
}
