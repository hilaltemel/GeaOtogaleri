using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Car_Dealership.Models;

    public class User
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
    }
