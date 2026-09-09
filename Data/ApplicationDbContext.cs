using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Car_Dealership.Models;
public class ApplicationDbContext : DbContext
{
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
{
}
public DbSet<Car> Cars { get; set; }
public DbSet<CarImage> CarImages { get; set; }
public DbSet<Brand> Brands { get; set; }
public DbSet<CarModel> CarModels { get; set; }
public DbSet<User> Users { get; set; }
public DbSet<Appointment> Appointments { get; set; }
protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.User>().HasData(
            new Models.User { Id = 1, UserName = "Hilal Temel", Role = "Admin", Password = "hilal123" },
            new Models.User { Id = 2, UserName = "Ahmet Yılmaz", Role = "Customer", Password = "ahmet456" },
            new Models.User { Id = 3, UserName = "Ayşe Demir", Role = "Customer", Password = "ayse789" },
            new Models.User { Id = 4, UserName = "Mehmet Kaya", Role = "Customer", Password = "mehmet321" },
            new Models.User { Id = 5, UserName = "Zeynep Çelik", Role = "Customer", Password = "zeynep654" },
            new Models.User { Id = 6, UserName = "Can Arslan", Role = "Customer", Password = "can987" },
            new Models.User { Id = 7, UserName = "Elif Şahin", Role = "Customer", Password = "elif147" },
            new Models.User { Id = 8, UserName = "Burak Koç", Role = "Customer", Password = "burak258" },
            new Models.User { Id = 9, UserName = "Deniz Aydın", Role = "Customer", Password = "deniz369" },
            new Models.User { Id = 10, UserName = "Cemre Yıldız", Role = "Customer", Password = "cemre159" }
        );
        modelBuilder.Entity<Car>()
        .HasOne(c => c.CarModel)
        .WithMany(m => m.Cars)
        .HasForeignKey(c => c.CarModelId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Car>()
            .HasOne(c => c.Brand)
            .WithMany()
            .HasForeignKey(c => c.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        // SİSTEME 10 POPÜLER MARKA EKLİYORUZ
    modelBuilder.Entity<Brand>().HasData(
        new Brand { Id = 1, Name = "Toyota" }, //
        new Brand { Id = 2, Name = "Volkswagen" }, //
        new Brand { Id = 3, Name = "Hyundai" }, //
        new Brand { Id = 4, Name = "Ford" }, //
        new Brand { Id = 5, Name = "Honda" }, //
        new Brand { Id = 6, Name = "BMW" }, //
        new Brand { Id = 7, Name = "Mercedes-Benz" }, //
        new Brand { Id = 8, Name = "Audi" }, //
        new Brand { Id = 9, Name = "Nissan" }, //
        new Brand { Id = 10, Name = "Porsche" } //
    );

    // HER MARKA İÇİN 5 POPÜLER MODEL (Toplam 50 Model)
    modelBuilder.Entity<CarModel>().HasData(
        // Toyota (BrandId: 1)
        new CarModel { Id = 1, Name = "Corolla", BrandId = 1 },
        new CarModel { Id = 2, Name = "RAV4", BrandId = 1 },
        new CarModel { Id = 3, Name = "Camry", BrandId = 1 },
        new CarModel { Id = 4, Name = "Yaris", BrandId = 1 },
        new CarModel { Id = 5, Name = "Hilux", BrandId = 1 },

        // VW (BrandId: 2)
        new CarModel { Id = 6, Name = "Golf", BrandId = 2 },
        new CarModel { Id = 7, Name = "Passat", BrandId = 2 },
        new CarModel { Id = 8, Name = "Polo", BrandId = 2 },
        new CarModel { Id = 9, Name = "Tiguan", BrandId = 2 },
        new CarModel { Id = 10, Name = "T-Roc", BrandId = 2 },

        // Hyundai (BrandId: 3)
        new CarModel { Id = 11, Name = "Tucson", BrandId = 3 },
        new CarModel { Id = 12, Name = "i20", BrandId = 3 },
        new CarModel { Id = 13, Name = "Elantra", BrandId = 3 },
        new CarModel { Id = 14, Name = "Santa Fe", BrandId = 3 },
        new CarModel { Id = 15, Name = "Bayon", BrandId = 3 },

        // Ford (BrandId: 4)
        new CarModel { Id = 16, Name = "Focus", BrandId = 4 },
        new CarModel { Id = 17, Name = "Fiesta", BrandId = 4 },
        new CarModel { Id = 18, Name = "Puma", BrandId = 4 },
        new CarModel { Id = 19, Name = "Kuga", BrandId = 4 },
        new CarModel { Id = 20, Name = "Mustang", BrandId = 4 },

        // Honda (BrandId: 5)
        new CarModel { Id = 21, Name = "Civic", BrandId = 5 },
        new CarModel { Id = 22, Name = "City", BrandId = 5 },
        new CarModel { Id = 23, Name = "CR-V", BrandId = 5 },
        new CarModel { Id = 24, Name = "HR-V", BrandId = 5 },
        new CarModel { Id = 25, Name = "Accord", BrandId = 5 },

        // BMW (BrandId: 6)
        new CarModel { Id = 26, Name = "3 Serisi", BrandId = 6 },
        new CarModel { Id = 27, Name = "5 Serisi", BrandId = 6 },
        new CarModel { Id = 28, Name = "1 Serisi", BrandId = 6 },
        new CarModel { Id = 29, Name = "X1", BrandId = 6 },
        new CarModel { Id = 30, Name = "X5", BrandId = 6 },

        // Mercedes-Benz (BrandId: 7)
        new CarModel { Id = 31, Name = "G-Serisi (G 63 AMG)", BrandId = 7 },
        new CarModel { Id = 32, Name = "E-Serisi", BrandId = 7 },
        new CarModel { Id = 33, Name = "C-Serisi", BrandId = 7 },
        new CarModel { Id = 34, Name = "S-Serisi", BrandId = 7 },
        new CarModel { Id = 35, Name = "GLC", BrandId = 7 },

        // Audi (BrandId: 8)
        new CarModel { Id = 36, Name = "A3", BrandId = 8 },
        new CarModel { Id = 37, Name = "A4", BrandId = 8 },
        new CarModel { Id = 38, Name = "A6", BrandId = 8 },
        new CarModel { Id = 39, Name = "Q3", BrandId = 8 },
        new CarModel { Id = 40, Name = "Q5", BrandId = 8 },

        // Nissan (BrandId: 9)
        new CarModel { Id = 41, Name = "Qashqai", BrandId = 9 },
        new CarModel { Id = 42, Name = "Juke", BrandId = 9 },
        new CarModel { Id = 43, Name = "X-Trail", BrandId = 9 },
        new CarModel { Id = 44, Name = "Micra", BrandId = 9 },
        new CarModel { Id = 45, Name = "Leaf", BrandId = 9 },

        // Porsche (BrandId: 10)
        new CarModel { Id = 46, Name = "Macan", BrandId = 10 },
        new CarModel { Id = 47, Name = "Cayenne", BrandId = 10 },
        new CarModel { Id = 48, Name = "Panamera", BrandId = 10 },
        new CarModel { Id = 49, Name = "Taycan", BrandId = 10 },
        new CarModel { Id = 50, Name = "911", BrandId = 10 }
    );
    }
}