using System.ComponentModel.DataAnnotations;

namespace OmniSight.Core.Entities
{
    // Thêm Enum này để phân loại Token
    public enum TokenType
    {
        EmailVerification,
        PasswordReset
    }

    public class AuthToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public TokenType Type { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsUsed { get; set; } = false;

        // Liên kết ngược lại bảng User
        public virtual User User { get; set; } = null!;
    }
}