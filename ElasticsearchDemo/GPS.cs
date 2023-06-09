using Elasticsearch.Net;
using Nest;
namespace ElasticsearchDemo {


    public class GPS {
        //[PropertyName("_id")]
        //public long ID { get; set; }
        [GeoPoint]
        public LAL LAL { get; set; }
        [Keyword]
        public string PlateNo { get; set; }
        
        public byte PlateColor { get; set; }
        [Date(Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime GpsTime { get; set; }
        [Date(Name = "create_time")]
        public DateTime CreateTime { get; set; }

        public string GpsTimeStr { get => GpsTime.ToString("yyyy-MM-dd HH:mm:ss"); }
    }

    public class LAL {
        public double lat { get; set; }
        public double lon { get; set; }
    }
}
