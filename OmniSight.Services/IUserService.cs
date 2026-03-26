using OmniSight.Core.Entities;

namespace OmniSight.Services
{
    public interface IUserService
    {
        Task<bool> UpdateProfileAsync(int userId, string fullName, string phone, bool isStudent, bool isTeacher);
    }
}