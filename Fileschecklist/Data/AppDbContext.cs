using Fileschecklist.Controllers;
using Fileschecklist.Models;
using Microsoft.EntityFrameworkCore;
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
        public DbSet<Campus> Campuses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Schoolyear> Schoolyears { get; set; }
    }



}
