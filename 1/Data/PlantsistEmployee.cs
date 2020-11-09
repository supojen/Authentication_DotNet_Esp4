using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _1.Data
{
    public class PlantsistEmployee : IdentityUser
    {
        public string Department { get; set; }
        public int level { get; set; }  
    }
}
