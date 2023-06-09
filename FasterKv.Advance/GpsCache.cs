using FasterKv.Cache.Core.Abstractions;
using FasterKv.Cache.Core.Configurations;
using FasterKv.Cache.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FasterKv.Cache.MessagePack;

namespace FasterKv.Advance {
    public class GpsCache {
        public static void Run() {
            var cache = new FasterKvCache<GpsPointStruct>("MyTCache",
    new DefaultSystemClock(),
    new FasterKvCacheOptions(),
    new IFasterKvCacheSerializer[]
    {
        new MessagePackFasterKvCacheSerializer
        {
            Name = "MyTCache"
        }
    },
    null);
        }
    }
}
