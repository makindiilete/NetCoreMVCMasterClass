using Microsoft.EntityFrameworkCore;

namespace Asp_Net_Core_Masterclass.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Michaelz Omoakin",
                    Department = DeptEnum.IT,
                    Email = "akindiileteforex@gmail.com"
                },
                new Employee
                {
                    Id = 2,
                    Name = "Omoakin D Marven",
                    Department = DeptEnum.HR,
                    Email = "oluwaferanmi@gmail.com"
                }
            );
        }
    }
}
