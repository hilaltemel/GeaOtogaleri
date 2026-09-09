using System.ComponentModel.DataAnnotations;

namespace Car_Dealership.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Tarih seçimi zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Saat seçimi zorunludur.")]
        public string? AppointmentTime { get; set; } // "09:00 - 10:00" gibi saat dilimlerini metin olarak tutacağız

        // Araç bilgisi zorunlu değil (Nullable int)
        public long? CarId { get; set; }
        
        // Navigation Property
        public Car? Car { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}