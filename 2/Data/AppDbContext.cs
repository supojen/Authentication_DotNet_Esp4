using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _1.Data
{
    public class AppDbContext : IdentityDbContext<PlantsistEmployee>
    {
        public AppDbContext(DbContextOptions opts)
            : base(opts)
        {}
    }
}
