using Microsoft.EntityFrameworkCore;
using ProductExperiences.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductExperiences.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base (options)
        {

        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Experience> Experiences { get; set; }
    }
}
