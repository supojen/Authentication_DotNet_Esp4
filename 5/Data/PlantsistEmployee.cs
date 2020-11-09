using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _5.Data
{
    public class PlantsistEmployee : IdentityUser<Guid>
    {  
        public string Department { get; set; }
        public int Level { get; set; }  
    }
}
