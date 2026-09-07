using System.ComponentModel.DataAnnotations;
namespace Car_Dealership.Models
{
    public enum FuelType
    {
        [Display(Name = "Benzin")]
        Benzin = 1,
        [Display(Name = "Dizel")]
        Dizel = 2,
        [Display(Name = "Hybrid")]
        Hybrid = 3,
        [Display(Name = "Elektrikli")]
        Elektrikli = 4,
        [Display(Name = "LPG")]
        LPG = 5
    }
    public enum TransmissionType
    {
        [Display(Name ="Manuel")]
        Manuel = 1,
        [Display(Name ="Otomatik")]
        Otomatik = 2,
        [Display(Name ="Yarı Otomatik")]
        YariOtomatik = 3
    }
}