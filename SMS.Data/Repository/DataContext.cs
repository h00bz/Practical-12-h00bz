using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SMS.Data.Entities;

namespace SMS.Data.Repository;

// internal accessibility means context is only accessible inside SMS.Data project
internal class DataContext : DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
     
    public DbSet<Module> Modules { get; set; }
    public DbSet<StudentModule> StudentModules { get; set; }
       
    public DbSet<User> Users { get; set; }
        
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionStringSqlite = "Filename=data.db";
        //var connectionStringMySql = "server=localhost; port=3306; database=XXX; user=XXX; password=XXX";
        //var connectionStringPostgres = "host=localhost; port=5432; database=XXX; username=XXX; password=XXX";
        //var connectionStringSqlServer = "Server=(localdb)\\mssqllocaldb;Database=XXX;Trusted_Connection=True;";
        
        optionsBuilder
        .UseSqlite(connectionStringSqlite)
        //.UseNpgsql(connectionStringPosrgres);  
        //.UseSqlServer(connectionStringSqlServer)
        //.UseMySql(connectionStringMySql,ServerVersion.AutoDetect(connectionStringMySql))             
        .LogTo(Console.WriteLine, LogLevel.Information)
        ;
    }

    // custom method used in development to keep database in sync with models
    public void Initialise() 
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }
    
}
 
