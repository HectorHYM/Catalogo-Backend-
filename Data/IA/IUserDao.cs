using CatalogoBackend.Models.Entities;
using CatalogoBackend.Models.Responses;

namespace CatalogoBackend.Data.IA{
    public interface IUserDao
    {
        Task<User> CreateUser(User user);
        Task<User?> GetById(int id);
        Task<User?> GetByUsername(string username);
        Task<User?> GetByEmail(string email);
    }
}