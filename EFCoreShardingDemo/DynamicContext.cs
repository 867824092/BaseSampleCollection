using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EFCoreShardingDemo {

    public class DynamicContext : DbContext , IShardingTableDbContext {
        public bool UseIntProperty { get; set; }
        public readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        public DbSet<ConfigurableEntity> Entities { get; set; }
        public string Tail { get ; set ; }


        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
            //.UseInMemoryDatabase("DynamicContext")
            .UseSqlite("Data Source=F:\\Samples\\Net\\EfCoreDemo\\EFCoreShardingDemo\\mydb.db")
            .UseLoggerFactory(MyLoggerFactory)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
        //.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>();
        //.ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactoryDesignTimeSupport>()
            .ReplaceService<IModelCacheKeyFactory, ShardingModelCacheKeyFactory>()
            .ReplaceService<IModelCustomizer, ShardingModelCustomizer>();
        #endregion

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //if (UseIntProperty) {
            //    modelBuilder.Entity<ConfigurableEntity>().Ignore(e => e.StringProperty);
            //}
            //else {
            //    modelBuilder.Entity<ConfigurableEntity>().Ignore(e => e.IntProperty);
            //}
            modelBuilder.Entity<ConfigurableEntity>().ToTable(nameof(ConfigurableEntity));
        }
        #endregion


        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity) {
            if(entity is ConfigurableEntity type) {
                if(type.IntProperty < 10) {
                    var dbContext = new DynamicContext() { Tail = "" };
                    var entry = dbContext.Entry(entity);
                    entry.State = EntityState.Added;
                    dbContext.SaveChanges();
                    return entry;
                }
                else if(type.IntProperty >= 10 && type.IntProperty <= 20) {
                    var dbContext = new DynamicContext() { Tail = "_01" };
                    var entry = dbContext.Entry(entity);
                    entry.State = EntityState.Added;
                    dbContext.SaveChanges();
                    return entry;
                }
                else if(type.IntProperty > 20) {
                    var dbContext = new DynamicContext() { Tail = "_02" };
                    var entry = dbContext.Entry(entity);
                    entry.State = EntityState.Added;
                    dbContext.SaveChanges();
                    return entry;
                }
            }
            return base.Add(entity);
        }
    }
}
