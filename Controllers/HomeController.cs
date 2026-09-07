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
    ViewBag.Showcase = showcaseCars;
ViewBag.NewArrivals = latestCars;
ViewBag.Campaigns = campaignCars;
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
}
