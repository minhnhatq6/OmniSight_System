using Microsoft.EntityFrameworkCore;
using OmniSight.Core.Entities;
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

        public async Task<bool> UpdateProfileAsync(int userId, string fullName, string phone)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return false;

            // Cập nhật thông tin
            user.FullName = fullName;
            user.Phone = phone;

            

            _db.Users.Update(user);
            // Sửa dòng này: Thay _context bằng _db và chỉ giữ 1 dòng SaveChanges
            return await _db.SaveChangesAsync() > 0;
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<bool> UpdateFaceEmbeddingAsync(int userId, string embedding)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;

            user.FaceEmbedding = embedding;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _db.Users.AsNoTracking().OrderByDescending(u => u.UserId).ToListAsync();
        }

        public async Task<bool> GrantTeacherRoleAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsTeacher = true; // Cấp quyền GV
                user.IsStudent = false; // Tùy chọn: Hủy quyền học sinh (nếu muốn tách biệt hẳn)
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // --- SỬA HÀM NÀY TRONG UserService.cs ---
        public async Task<(bool success, string message)> CreateTeacherAccountAsync(string email, string fullName, string password)
        {
            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                return (false, "Email này đã tồn tại trong hệ thống!");
            }

            var newTeacher = new User
            {
                Email = email,
                Username = email,
                FullName = fullName,
                IsTeacher = true,
                IsStudent = false,
                IsEmailConfirmed = true,
                // Băm mật khẩu do Admin nhập vào
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _db.Users.Add(newTeacher);
            await _db.SaveChangesAsync();

            return (true, "Tạo tài khoản Giáo viên thành công!");
        }
    }
}