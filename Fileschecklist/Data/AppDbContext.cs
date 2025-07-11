using Microsoft.EntityFrameworkCore;
using Fileschecklist.Models;
using static Fileschecklist.Models.Checklistmodel;

namespace Fileschecklist.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {

        }
        public DbSet<Studentfiles> Studentfiles { get; set; }
        public DbSet<Campus> Campus { get; set; }
        public DbSet<User> Users { get; set; }


    }

   

}
