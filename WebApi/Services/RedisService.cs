using Newtonsoft.Json;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Services.Interface;

namespace WebApi.Services
{
    public class RedisService : IRedisService
    {
        private readonly StackExchange.Redis.IDatabase _cache = RedisConnectorHelper.Connection.GetDatabase();

        public User ReadData(int id)
        {
            var value = _cache.StringGet($"User:{id}");
            if (!value.IsNull)
            {
                return JsonConvert.DeserializeObject<User>(value);
            }
            return null;
        }

        public void SaveData(User user)
        {
            _cache.StringSet($"User:{user.Id}", JsonConvert.SerializeObject(user));
        }

        public void RemoveData(int id)
        {
            if (Exists(id))
            {
                _cache.KeyDelete($"User:{id}");
            }
        }

        public void UpdateData(User user)
        {
            if (Exists(user.Id))
            {
                RemoveData(user.Id);
                _cache.StringSet($"User:{user.Id}", JsonConvert.SerializeObject(user));
            }
        }

        public void Clear()
        {
            var endpoints = RedisConnectorHelper.Connection.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = RedisConnectorHelper.Connection.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }

        private bool Exists(int id)
        {
            return _cache.KeyExists($"User:{id}");
        }
    }
}
