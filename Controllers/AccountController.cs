using Microsoft.AspNetCore.Mvc;
using Car_Dealership.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Car_Dealership.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        // 1. Kayıt Sayfasını Ekrana Getir
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 2. Formdan Gelen Bilgilerle Kayıt İşlemini Yap
        [HttpPost]
        public async Task<IActionResult> Register(string userName, string password)
        {
            // KURAL 1: Kullanıcı adında boşluk olamaz
            if (userName.Contains(" "))
            {
                ModelState.AddModelError("UserName", "Kullanıcı adında boşluk olamaz. Lütfen bitişik yazın.");
                return View();
            }

            // KURAL 2: Kullanıcı adı daha önce alınmış mı?
            bool userExists = _context.Users.Any(u => u.UserName == userName);
            if (userExists)
            {
                ModelState.AddModelError("UserName", "Bu kullanıcı adı zaten kullanılıyor. Lütfen başka bir isim seçin.");
                return View();
            }

            // KURAL 3: Her şey uygunsa yeni kullanıcıyı yarat ve "Customer" rolünü ata
            var newUser = new User
            {
                UserName = userName,
                Password = password,
                Role = "Customer" // Standart müşteri rolü
            };

            // Veritabanına ekle ve kaydet
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Kayıt başarılıysa kullanıcıyı Login sayfasına yönlendir
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Admin");
            }
            
            return View();
        }
        // 3. FORMU YAKALAYAN METOT (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            // 1. Veritabanından kullanıcıyı bul
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName && u.Password == password);

            if (user != null) // Şifre doğruysa
            {
                // 2. Yaka kartına rolünü yaz
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.Role ?? "Customer") // Veritabanındaki Admin veya Customer rolü
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity));

                // 3. YÖNLENDİRME: Adminse Admin'e, Customer ise Ana Sayfaya
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // Şifre yanlışsa
            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            // Tarayıcıdaki cookie'yi sil ve unut
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Kullanıcıyı tekrar Login sayfasına yönlendir
            return RedirectToAction("Login", "Account");
        }
        // 1. Profil Sayfasını Getir
        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            // Sistemde giriş yapmış olan kullanıcının adını oku
            var currentUserName = User.Identity!.Name;
            
            // Veritabanından bu kullanıcıyı bulup sayfaya gönder
            var user = _context.Users.FirstOrDefault(u => u.UserName == currentUserName);
            
            if (user == null) return RedirectToAction("Logout");

            return View(user);
        }

        // 2. Profildeki Değişiklikleri Kaydet
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(string newUserName, string newPassword)
        {
            var currentUserName = User.Identity!.Name;
            var user = _context.Users.FirstOrDefault(u => u.UserName == currentUserName);

            if (user == null) return RedirectToAction("Logout");

            bool isChanged = false;

            // EĞER KULLANICI ADI DEĞİŞMİŞSE VE BOŞ DEĞİLSE
            if (!string.IsNullOrWhiteSpace(newUserName) && newUserName != currentUserName)
            {
                if (newUserName.Contains(" "))
                {
                    ModelState.AddModelError("newUserName", "Kullanıcı adında boşluk olamaz.");
                    return View(user);
                }
                if (_context.Users.Any(u => u.UserName == newUserName))
                {
                    ModelState.AddModelError("newUserName", "Bu kullanıcı adı zaten alınmış.");
                    return View(user);
                }
                
                user.UserName = newUserName;
                isChanged = true;
            }

            // EĞER ŞİFRE DEĞİŞMİŞSE (Alan boş bırakılmadıysa)
            if (!string.IsNullOrWhiteSpace(newPassword) && newPassword != user.Password)
            {
                user.Password = newPassword;
                isChanged = true;
            }

            // Eğer bir değişiklik yapıldıysa veritabanını güncelle
            if (isChanged)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                // KULLANICI DENEYİMİ (UX) VE GÜVENLİK DETAYI:
                // Bilgiler (özellikle kullanıcı adı) değiştiği için bilgiler geçersiz kalır.
                // Bu yüzden kullanıcıyı çıkışa yönlendirip yeni bilgileriyle girmesini istiyoruz.
                return RedirectToAction("Logout"); 
            }

            // Hiçbir şey değiştirilmeden butona basıldıysa aynı sayfada kal
            ViewBag.SuccessMessage = "Bilgileriniz güncel.";
            return View(user);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}