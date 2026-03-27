using Microsoft.EntityFrameworkCore;
using OmniSight.Data;

namespace OmniSight.Services
{
    public class UserService : IUserService
    {
        private readonly OmniSightDbContext _db;

        public UserService(OmniSightDbContext db)
        {
            _db = db;
        }

        public async Task<bool> UpdateProfileAsync(int userId, string fullName, string phone, bool isStudent, bool isTeacher)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return false;

            // Cập nhật thông tin
            user.FullName = fullName;
            user.Phone = phone;

            // Cập nhật Role
            user.IsStudent = isStudent;
            user.IsTeacher = isTeacher;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UpdateFaceEmbeddingAsync(int userId, string embedding)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;

            user.FaceEmbedding = embedding;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}