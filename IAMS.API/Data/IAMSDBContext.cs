using Microsoft.EntityFrameworkCore;

using IAMS.Model;

namespace IAMS.API.Data
{
    public class IAMSDBContext:DbContext
    {

        public IAMSDBContext(DbContextOptions<IAMSDBContext> options):base(options)
        {
            
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Country>().ToTable("Country");
        //    modelBuilder.Entity<State>().ToTable("State");
        //    modelBuilder.Entity<AddressFormat>().ToTable("AddressFormat");
        //    modelBuilder.Entity<Address>().ToTable("Address");
        //    base.OnModelCreating(modelBuilder);
        //}
      
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<AddressFormat> AddressFormats{ get; set; }
        public DbSet<Address> Addresses{ get; set; }
    }
}
