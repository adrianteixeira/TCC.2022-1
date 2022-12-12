using StackExchange.Redis;
using System;

namespace redistest
{
    public class RedisConnectorHelper
    {
        static RedisConnectorHelper()
        {
            RedisConnectorHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            { 
                
                return ConnectionMultiplexer.Connect("redis-19584.c282.east-us-mz.azure.cloud.redislabs.com:19584,password=bHJoMuo7YMXrTNAIbCK5cRPPVY45bBYo");
            });
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }

}
