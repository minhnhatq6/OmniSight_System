using OmniSight.Core.Entities;

namespace OmniSight.Services
{
    public interface IUserService
    {
        Task<bool> UpdateProfileAsync(int userId, string fullName, string phone);
        Task<bool> UpdateFaceEmbeddingAsync(int userId, string embedding);
        Task<User> GetUserByIdAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> GrantTeacherRoleAsync(int userId);
        Task<(bool success, string message)> CreateTeacherAccountAsync(string email, string fullName, string password);
    }
}