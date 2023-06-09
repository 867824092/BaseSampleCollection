using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PluginTwo.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TwoController : ControllerBase {
        [HttpGet]
        public IActionResult Get() {

            using var produer = new Confluent.Kafka.ProducerBuilder<Null, string>(new Confluent.Kafka.ProducerConfig {
                BootstrapServers = "localhost:9092",
                Acks = Confluent.Kafka.Acks.All,
                EnableIdempotence = true
            }).Build();

            produer.Produce("topic1", new Confluent.Kafka.Message<Null, string> {
                Value = $"插件2:{DateTime.Now}发送了一条消息"
            });

            return Ok("PluginTwo");
        }
    }
}
