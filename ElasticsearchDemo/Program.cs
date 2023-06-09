using Elasticsearch.Net;
using ElasticsearchDemo;
using Nest;
using Nest.JsonNetSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using Yitter.IdGenerator;


var options = new IdGeneratorOptions(1);
// options.WorkerIdBitLength = 10; // WorkerIdBitLength 默认值6，支持的 WorkerId 最大值为2^6-1，若 WorkerId 超过64，可设置更大的 WorkerIdBitLength
// ...... 其它参数设置参考 IdGeneratorOptions 定义，一般来说，只要再设置 WorkerIdBitLength （决定 WorkerId 的最大值）。

// 保存参数（必须的操作，否则以上设置都不能生效）：
YitIdHelper.SetIdGenerator(options);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IElasticClient, ElasticClient>(provider => {
    var connectionString = new Nest.ConnectionSettings(new Uri("http://192.168.131.1:9200"));
    return new Nest.ElasticClient(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
string index = "vehicle_gps";

app.MapGet("/test",async  () => {
    await Task.Delay(100);
    return "123";
});

app.MapPost("/post", async (IElasticClient client) => {

    //using var settings = new ConnectionConfiguration(new Uri("http://192.168.131.1:9200"));

    //var client = new ElasticLowLevelClient(settings);
    //var gps = new GPS {
    //    ID = YitIdHelper.NextId(),
    //    LAL = new LAL {
    //        lat = 39.967157,
    //        lon = 116.322631
    //    },
    //    PlateNo = "豫A0001",
    //    PlateColor = 1
    //};
    //var result = client.Index<ReponseTest>(index, gps.ID.ToString(), PostData.String(JsonConvert.SerializeObject(gps)));
    //return result.ApiCall.Success;

    //using var connectionString = new Nest.ConnectionSettings(new Uri("http://192.168.131.1:9200"));
    //var client = new Nest.ElasticClient(connectionString);
    //var gps = new GPS {
    //    ID = YitIdHelper.NextId(),
    //    LAL = new LAL {
    //        lat = 39.967157,
    //        lon = 116.322631
    //    },
    //    PlateNo = "豫A0001",
    //    PlateColor = 1,
    //    GpsTime = DateTime.Now
    //};
    //var result = await client.IndexAsync(new IndexRequest<GPS>(gps, "vehicle_gps"));

    var result = await client.IndexManyAsync(Enumerable.Range(1, 10000).Select(u => new GPS {
        //ID = YitIdHelper.NextId(),
        LAL = new LAL {
            lat = 39.967157,
            lon = 116.322631
        },
        PlateNo = "豫A0001",
        PlateColor = 1,
        GpsTime = DateTime.Now
    }), "vehicle_gps");
    return "success";

});

app.MapGet("/get", async (IElasticClient client) => {
    //using var connectionString = new Nest.ConnectionSettings(new Uri("http://192.168.131.1:9200"));
    //var client = new Nest.ElasticClient(connectionString);
    var result = await client.SearchAsync<GPS>(s =>s.Index(Indices.Index("vehicle_gps")).Query(q=>q.MatchAll()));
    return result.Documents.ToList();
});



app.Run();

public class TestSear : IElasticsearchSerializer {
    public object Deserialize(Type type, Stream stream) {
        throw new NotImplementedException();
    }

    public T Deserialize<T>(Stream stream) {
        throw new NotImplementedException();
    }

    public Task<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }

    public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }

    public void Serialize<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None) {
       
    }

    public Task SerializeAsync<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None, CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
}

public class DefaultElasticClient {
    readonly IElasticClient _client;
    public DefaultElasticClient(IElasticClient client) {
        _client = client;
    }
}
