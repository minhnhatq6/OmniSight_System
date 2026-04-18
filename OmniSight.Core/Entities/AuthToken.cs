using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniSight.Core.Entities
{
    public enum TokenType { EmailVerification, PasswordReset }

    [Table("TokenXacThuc")]
    public class AuthToken
    {
        [Key]
        [Column("MaToken")]
        public int Id { get; set; }

        [Required]
        [Column("MaNguoiDung")]
        public int UserId { get; set; }

        [Required]
        [Column("ChuoiToken")]
        public string Token { get; set; } = string.Empty;

        [Required]
        [Column("LoaiToken")]
        public TokenType Type { get; set; }

        [Column("NgayHetHan")]
        public DateTime ExpiryDate { get; set; }

        [Column("DaSuDung")]
        public bool IsUsed { get; set; } = false;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}