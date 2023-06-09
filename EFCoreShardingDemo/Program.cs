// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using EFCoreShardingDemo;


//using (var context = new DynamicContext { UseIntProperty = true }) {
//    context.Entities.Add(new ConfigurableEntity { IntProperty = 44, StringProperty = "Aloha" });
//    context.SaveChanges();
//}

//using (var context = new DynamicContext { UseIntProperty = false }) {
//    context.Entities.Add(new ConfigurableEntity { IntProperty = 43, StringProperty = "Hola" });
//    context.SaveChanges();
//}

//using (var context = new DynamicContext { UseIntProperty = true }) {
//    var entity = context.Entities.Single();

//    // Writes 44 and an empty string
//    Console.WriteLine($"{entity.IntProperty} {entity.StringProperty}");
//}

//using (var context = new DynamicContext { UseIntProperty = false }) {
//    var entity = context.Entities.Single();

//    // Writes 0 and an "Hola"
//    Console.WriteLine($"{entity.IntProperty} {entity.StringProperty}");
//}

using (var context = new DynamicContext()) {
    context.Add(new ConfigurableEntity { IntProperty = 44, StringProperty = "Aloha" });
    context.Add(new ConfigurableEntity { IntProperty = 1, StringProperty = "Aloha" });
    context.Add(new ConfigurableEntity { IntProperty = 15, StringProperty = "Aloha" });
    //context.Entities.Add(new ConfigurableEntity { IntProperty = 44, StringProperty = "Aloha" });
    //context.SaveChanges();
}

Console.ReadLine();


public interface IShardingTableDbContext {
    string Tail { get; set; }
}

public interface IRouteTail {
    string GetRouteTailIdentity();
}

#region DynamicModel
public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory {
    public object Create(DbContext context) {
        var result = context is DynamicContext dynamicContext
            ? (context.GetType(), dynamicContext.UseIntProperty)
            : (object)context.GetType();
        return result;
    }   
}
#endregion
#region DynamicModelDesignTimeSupport
public class DynamicModelCacheKeyFactoryDesignTimeSupport : IModelCacheKeyFactory {
    public object Create(DbContext context, bool designTime)
        => context is DynamicContext dynamicContext
            ? (context.GetType(), dynamicContext.UseIntProperty, designTime)
            : (object)context.GetType();

    public object Create(DbContext context)
        => Create(context, false);
}
#endregion
#region Sharding
public class ShardingModelCacheKeyFactory : IModelCacheKeyFactory {
    public object Create(DbContext context) {
        return Create(context, false);
    }
    public object Create(DbContext context, bool designTime) {
        if (context is IShardingTableDbContext shardingTableDbContext) {
            return $"{context.GetType()}_{shardingTableDbContext.Tail}_{designTime}";
        }
        else {
            return (context.GetType(), designTime);
        }
    }
}

public class ShardingModelCustomizer : ModelCustomizer {
    public ShardingModelCustomizer(ModelCustomizerDependencies dependencies) : base(dependencies) {
    }
    public override void Customize(ModelBuilder modelBuilder, DbContext context) {
        base.Customize(modelBuilder, context);

        if (context is IShardingTableDbContext shardingTableDbContext) {
            //设置分表
            MappingToTable(modelBuilder, shardingTableDbContext.Tail);

        }
    }

    private void MappingToTable(ModelBuilder modelBuilder, string tail) {
        var entity = modelBuilder.Entity<ConfigurableEntity>();        
        entity.ToTable($"{nameof(ConfigurableEntity)}{tail}");
    }
}
#endregion
