using WebApi.Entities;

namespace WebApi.Services.Interface
{
    public interface IRedisService
    {
        public User ReadData(int id);

        public void SaveData(User user);

        public void RemoveData(int id);

        public void UpdateData(User user);

        public void Clear();
    }
}
