using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asp_Net_Core_Masterclass.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser> //we added <ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
//here we declare all the types we have in the app currently we have only one type which is Employee...
//Each model/type/domain class declared here will turn to a table in the database... EF Core will create a table for each domain class declare here
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();

            //Here we are implementing NO ACTION ON DELETE so users cannot delete a column (role) that is a another table depend on e.g. A role cannot be deleted once we have users inside the role unless those users are first remove
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
