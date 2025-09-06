using CatalogoBackend.Data.IA;
using CatalogoBackend.Models.Entities;
using CatalogoBackend.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace CatalogoBackend.Data.DAOs{
    public class UserDao : IUserDao
    {
        private readonly AppDbContext _db;
        public UserDao(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User> CreateUser(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetById(int id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<User?> GetByUsername(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}