using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SWM.Models
{
    public class SwmContext : IdentityDbContext<SwmUser>
    {
        private IConfigurationRoot _config;
        private IHostingEnvironment _env;

        public SwmContext(IConfigurationRoot config, IHostingEnvironment env, DbContextOptions options) : base(options)
        {
            _config = config;
            _env = env;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if(_env.IsEnvironment("Production"))
                optionsBuilder.UseSqlServer(_config["dbstring_s"]);
            else
                optionsBuilder.UseSqlServer(_config["dbstring"]);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CropData>().HasKey(k => new { k.CropToUserId, k.DateTime });
            builder.Entity<MachineToUser>().HasKey(k => new { k.MachineId, k.UserID });
            builder.Entity<UserToSubscription>().HasKey(k => new { k.UserID, k.SubscriptionId });
        }

        public DbSet<SwmUser> SwmUsers { get; set; }
        public DbSet<UserRoleManager> UserRoleManagers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CropData> CropDatas { get; set; }
        public DbSet<CropInfo> CropInfos { get; set; }
        public DbSet<CropsToUser> CropToUsers { get; set; }
        public DbSet<FarmLocation> FarmLocations { get; set; }
        public DbSet<FarmLocationToMachine> FarmLocationToMachines { get; set; }
        public DbSet<MachineToUser> MachineToUsers { get; set; }
        public DbSet<PinNumber> PinNumbers { get; set; }
        public DbSet<SubscriptionType> SubscriptionTypes { get; set; }
        public DbSet<UserToSubscription> UserToSubscriptions { get; set; }
        public DbSet<OtherData> OtherDatas { get; set; }
        public DbSet<MachineId> MachineIds { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<WebEnvironment> WebEnvironments { get; set; }
    }
}
