using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreShardingDemo {
    //[Table("ConfigurableEntity")]
    public class ConfigurableEntity {
        public int Id { get; set; }
        public int? IntProperty { get; set; }
        public string? StringProperty { get; set; }
    }
}
