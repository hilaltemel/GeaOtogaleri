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
    public enum BodyType
    {
        [Display(Name = "Sedan")]
        Sedan = 1,
        
        [Display(Name = "Hatchback")]
        Hatchback = 2,
        
        [Display(Name = "Station Wagon")]
        StationWagon = 3,
        
        [Display(Name = "SUV")]
        SUV = 4,
        
        [Display(Name = "Coupe")]
        Coupe = 5,
        
        [Display(Name = "Cabriolet")]
        Cabriolet = 6,
        
        [Display(Name = "Minivan / MPV")]
        Minivan = 7,
        
        [Display(Name = "Panelvan")]
        Panelvan = 8,
        
        [Display(Name = "Pick-up")]
        Pickup = 9
    }

    public enum Drivetrain
    {
        [Display(Name = "Önden Çekiş")]
        FrontWheelDrive = 1,
        
        [Display(Name = "Arkadan İtiş")]
        RearWheelDrive = 2,
        
        [Display(Name = "Dört Çeker (4x4)")]
        AllWheelDrive = 3
    }
}