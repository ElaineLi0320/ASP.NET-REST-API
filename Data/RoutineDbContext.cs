using System;
using Microsoft.EntityFrameworkCore;
using Routine.Api.Entities;

namespace Routine.Api.Data
{
    public class RoutineDbContext: DbContext
    {
        // constructor
        public RoutineDbContext(DbContextOptions<RoutineDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        
        // Constraint definition within the data model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>().Property(x => x.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Company>().Property(x => x.Introduction).IsRequired().HasMaxLength(500);
            
            modelBuilder.Entity<Employee>()
                .Property(x => x.EmployeeNo).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Employee>()
                .Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Employee>()
                .Property(x => x.LastName).IsRequired().HasMaxLength(50);
            
            // Define relationship between 'Employee' and 'Company' entities
            // An Employee is associated with one Company, a Company can have many Employees.
            // Set the foreign key in 'Employee' for 'Company' and restrict deletion behavior
            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Company)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.Cascade);
            
            // Configures initial seed data for the 'Company' entity
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    Name = "Microsoft",
                    Introduction = "Great Company"
                },
                new Company
                {
                    Id = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    Name = "Google",
                    Introduction = "Don't be evil"
                },
                new Company
                {
                    Id = Guid.Parse("5efc910b-2f45-43df-afae-620d40542853"),
                    Name = "Alipapa",
                    Introduction = "Fubao Company"
                },
                new Company
                {
                    Id = Guid.Parse("bbdee09c-089b-4d30-bece-44df59237100"),
                    Name = "Tencent",
                    Introduction = "From Shenzhen"
                },
                new Company
                {
                    Id = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716400"),
                    Name = "Baidu",
                    Introduction = "From Beijing"
                },
                new Company
                {
                    Id = Guid.Parse("5efc910b-2f45-43df-afae-620d40542800"),
                    Name = "Adobe",
                    Introduction = "Photoshop?"
                },
                new Company
                {
                    Id = Guid.Parse("bbdee09c-089b-4d30-bece-44df59237111"),
                    Name = "SpaceX",
                    Introduction = "Wow"
                },
                new Company
                {
                    Id = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716411"),
                    Name = "AC Milan",
                    Introduction = "Football Club"
                },
                new Company
                {
                    Id = Guid.Parse("5efc910b-2f45-43df-afae-620d40542811"),
                    Name = "Suning",
                    Introduction = "From Jiangsu"
                },
                new Company
                {
                    Id = Guid.Parse("bbdee09c-089b-4d30-bece-44df59237122"),
                    Name = "Twitter",
                    Introduction = "Blocked"
                },
                new Company
                {
                    Id = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716422"),
                    Name = "Youtube",
                    Introduction = "Blocked"
                },
                new Company
                {
                    Id = Guid.Parse("5efc910b-2f45-43df-afae-620d40542822"),
                    Name = "360",
                    Introduction = "- -"
                },
                new Company
                {
                    Id = Guid.Parse("bbdee09c-089b-4d30-bece-44df59237133"),
                    Name = "Jingdong",
                    Introduction = "Brothers"
                },
                new Company
                {
                    Id = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716433"),
                    Name = "NetEase",
                    Introduction = "Music?",
                },
                new Company
                {
                    Id = Guid.Parse("5efc910b-2f45-43df-afae-620d40542833"),
                    Name = "Amazon",
                    Introduction = "Store"
                },
                new Company
                {
                    Id = Guid.Parse("bbdee09c-089b-4d30-bece-44df59237144"),
                    Name = "AOL",
                    Introduction = "Not Exists?"
                },
                new Company
                {
                    Id = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716444"),
                    Name = "Yahoo",
                    Introduction = "Who?"
                },
                new Company
                {
                    Id = Guid.Parse("5efc910b-2f45-43df-afae-620d40542844"),
                    Name = "Firefox",
                    Introduction = "Is it a company?"
                });

            
            // Configures initial seed data for the 'Employees' entity
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = Guid.Parse("4b501cb3-d168-4cc0-b375-48fb33f318a4"),
                    CompanyId = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    DateOfBirth = new DateTime(1976, 1, 2),
                    EmployeeNo = "MSFT231",
                    FirstName = "Nick",
                    LastName = "Carter",
                    Gender = Gender.Male
                },
                new Employee
                {
                    Id = Guid.Parse("7eaa532c-1be5-472c-a738-94fd26e5fad6"),
                    CompanyId = Guid.Parse("bbdee09c-089b-4d30-bece-44df5923716c"),
                    DateOfBirth = new DateTime(1981, 12, 5),
                    EmployeeNo = "MSFT245",
                    FirstName = "Vince",
                    LastName = "Carter",
                    Gender = Gender.Male
                },
                new Employee
                {
                    Id = Guid.Parse("72457e73-ea34-4e02-b575-8d384e82a481"),
                    CompanyId = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    DateOfBirth = new DateTime(1986, 11, 4),
                    EmployeeNo = "G003",
                    FirstName = "Mary",
                    LastName = "King",
                    Gender = Gender.Female
                },
                new Employee
                {
                    Id = Guid.Parse("7644b71d-d74e-43e2-ac32-8cbadd7b1c3a"),
                    CompanyId = Guid.Parse("6fb600c1-9011-4fd7-9234-881379716440"),
                    DateOfBirth = new DateTime(1977, 4, 6),
                    EmployeeNo = "G097",
                    FirstName = "Kevin",
                    LastName = "Richardson",
                    Gender = Gender.Male
                },
                new Employee
                {
                    Id = Guid.Parse("679dfd33-32e4-4393-b061-f7abb8956f53"),
                    CompanyId = Guid.Parse("5efc910b-2f45-43df-afae-620d40542853"),
                    DateOfBirth = new DateTime(1967, 1, 24),
                    EmployeeNo = "A009",
                    FirstName = "卡",
                    LastName = "里",
                    Gender = Gender.Female
                },
                new Employee
                {
                    Id = Guid.Parse("1861341e-b42b-410c-ae21-cf11f36fc574"),
                    CompanyId = Guid.Parse("5efc910b-2f45-43df-afae-620d40542853"),
                    DateOfBirth = new DateTime(1957, 3, 8),
                    EmployeeNo = "A404",
                    FirstName = "Not",
                    LastName = "Man",
                    Gender = Gender.Male
                });
        }
    }
}