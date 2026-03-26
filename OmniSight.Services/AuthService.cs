// Trong OmniSight.Core/Services/AuthService.cs
using BCrypt.Net;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;

using OmniSight.Core.Entities;
using OmniSight.Data;
using System.Text.Json;

namespace OmniSight.Services
{
public class AuthService
{
    private readonly OmniSightDbContext _db;
    private static readonly string SessionFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "OmniSight", "session.json");
    public User? CurrentUser
    {
        get; private set;
    }

    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _emailSender;
    private readonly string _emailPassword;

    // Sửa Constructor để nhận IConfiguration từ Dependency Injection
    public AuthService(OmniSightDbContext db, IConfiguration configuration)
    {
        _db = db;
        _clientId = configuration["GoogleAuth:ClientId"];
        _clientSecret = configuration["GoogleAuth:ClientSecret"];
        _emailSender = configuration["EmailSettings:Sender"] ?? "";
        _emailPassword = configuration["EmailSettings:AppPassword"] ?? "";
    }
    // --- ĐĂNG KÝ ---
    public async Task<(bool success, string message, User? user)> RegisterAsync(string email, string password, string fullName)
    {
        // SỬA DÒNG NÀY: Phải trả về đủ 3 giá trị cho trường hợp lỗi
        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            return (false, "Email này đã được sử dụng.", null);
        }

        var user = new User
        {
            Email = email,
            FullName = fullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Student"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await GenerateAndSendTokenAsync(user, TokenType.EmailVerification);

        // Dòng này của bạn đã đúng logic, giờ kiểu trả về ở trên đã khớp
        return (true, string.Empty, user);
    }
    // Lưu phiên đăng nhập
    private void SaveSession(int userId)
    {
        try
        {
            var directory = Path.GetDirectoryName(SessionFilePath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory!);

            var data = new { UserId = userId, LastActive = DateTime.UtcNow };
            var json = JsonSerializer.Serialize(data);

            // Ghi đè và đóng file ngay lập tức
            File.WriteAllText(SessionFilePath, json);
        }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Session Save Error: {ex.Message}"); }
    }
    public void Logout()
    {
        CurrentUser = null;
        if (File.Exists(SessionFilePath))
        {
            File.Delete(SessionFilePath); // Xóa file để hủy phiên làm việc
        }
    }
    public async Task<bool> TryAutoLoginAsync()
    {
        if (!File.Exists(SessionFilePath)) return false;

        try
        {
            // Đọc file với quyền truy cập chia sẻ để tránh lỗi "File in use" khi Restart
            using var stream = new FileStream(SessionFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(json)) return false;

            var data = JsonSerializer.Deserialize<JsonElement>(json);

            // Kiểm tra xem thuộc tính UserId có tồn tại không trước khi lấy
            if (data.TryGetProperty("UserId", out var idElement))
            {
                int userId = idElement.GetInt32();
                var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user != null)
                {
                    CurrentUser = user;
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            // Log lỗi thầm lặng để không làm sập App
            System.Diagnostics.Debug.WriteLine($"AutoLogin Error: {ex.Message}");
        }
        return false;
    }

    // --- TẠO VÀ GỬI TOKEN ---
    public async Task GenerateAndSendTokenAsync(User user, TokenType type)
    {
        // Vô hiệu hóa token cũ cùng loại
        var oldTokens = _db.AuthTokens.Where(t => t.UserId == user.UserId && t.Type == type);
        foreach (var t in oldTokens) t.IsUsed = true;

        var tokenValue = Guid.NewGuid().ToString("N");
        var token = new AuthToken
        {
            UserId = user.UserId,
            Token = tokenValue,
            Type = type,
            ExpiryDate = DateTime.UtcNow.AddMinutes(15)
        };

        _db.AuthTokens.Add(token);
        await _db.SaveChangesAsync();

        string subject = type == TokenType.EmailVerification
            ? "Xác nhận Email OmniSight"
            : "Đặt lại mật khẩu OmniSight";

        string path = type == TokenType.EmailVerification ? "verify-email" : "reset-password";

        // ✅ Link https về web/backend (khuyến nghị)
        string emailParam = Uri.EscapeDataString(user.Email!);
        string deepLink = $"omnisight://{path}?token={tokenValue}&email={emailParam}";

        // Nếu dùng Bridge GitHub (như bạn đang dùng):
        string pagesBase = "https://lekhai0123.github.io/omnisight-email-bridge";
        string actionUrl = $"{pagesBase}/open.html?path={path}&token={tokenValue}&email={emailParam}";

        string fullName = string.IsNullOrWhiteSpace(user.FullName) ? "bạn" : user.FullName;

        string htmlBody = EmailTemplates.BuildActionEmailHtml(
            appName: "OmniSight",
            title: type == TokenType.EmailVerification ? "Xác nhận email" : "Đặt lại mật khẩu",
            greetingName: fullName,
            messageLine1: type == TokenType.EmailVerification
                ? "Cảm ơn bạn đã đăng ký OmniSight. Vui lòng xác nhận email để hoàn tất."
                : "Chúng tôi nhận được yêu cầu đặt lại mật khẩu OmniSight của bạn.",
            buttonText: type == TokenType.EmailVerification ? "Xác nhận email" : "Đặt lại mật khẩu",
            actionUrl: actionUrl,
            expiryMinutes: 15,
            fallbackUrl: actionUrl,
            deepLink: deepLink
        );

        string textBody =
    $@"{subject}

Xin chào {fullName},

Vui lòng mở link sau (hết hạn sau 15 phút):
{actionUrl}

Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.

— OmniSight";

        await SendMailKitAsync(user.Email!, subject, htmlBody, textBody);
    }


    // --- XÁC THỰC TOKEN ---
    public async Task<(bool success, User? user)> VerifyTokenAsync(string tokenValue, TokenType type)
    {
        // 1. Dùng UtcNow để đồng bộ hoàn toàn với lúc tạo
        var nowUtc = DateTime.UtcNow;

        var token = await _db.AuthTokens.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == tokenValue
                                   && t.Type == type
                                   && !t.IsUsed);

        // 2. So sánh cùng hệ giờ Utc
        if (token == null || nowUtc > token.ExpiryDate)
        {
            return (false, null);
        }

        token.IsUsed = true;
        if (type == TokenType.EmailVerification)
        {
            token.User.IsEmailConfirmed = true;
        }

        await _db.SaveChangesAsync();
        return (true, token.User);
    }
    public async Task<bool> ValidateResetTokenAsync(string tokenValue)
    {
        var nowUtc = DateTime.UtcNow;

        // Chỉ kiểm tra sự tồn tại và hạn sử dụng
        var token = await _db.AuthTokens
            .FirstOrDefaultAsync(t => t.Token == tokenValue
                                   && t.Type == TokenType.PasswordReset
                                   && !t.IsUsed);

        if (token == null || nowUtc > token.ExpiryDate)
        {
            return false; // Token sai hoặc hết hạn
        }

        return true; // Token hợp lệ, cho phép hiện giao diện nhập pass
    }
    public async Task<(bool success, string message)> LoginWithEmailAsync(string email, string password)
    {
        // 1. Tìm user theo Email
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return (false, "Email hoặc mật khẩu không chính xác.");
        }

        // 2. Kiểm tra mật khẩu bằng BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return (false, "Email hoặc mật khẩu không chính xác.");
        }

        // 3. Kiểm tra đã xác nhận Email chưa
        if (!user.IsEmailConfirmed)
        {
            return (false, "Email chưa được xác thực. Vui lòng kiểm tra hộp thư.");
        }

        CurrentUser = user;
        SaveSession(user.UserId);
        return (true, string.Empty);
    }

    public async Task<bool> ResendVerificationEmailAsync(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsEmailConfirmed);
        if (user != null)
        {
            await GenerateAndSendTokenAsync(user, TokenType.EmailVerification);
            return true;
        }
        return false;
    }

    // --- GỬI MAIL THỰC TẾ ---
    private async Task SendMailKitAsync(string to, string subject, string htmlBody, string textBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("OmniSight", _emailSender));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = textBody
        };

        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_emailSender, _emailPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
    // 1. Gửi yêu cầu Reset mật khẩu
    public async Task<(bool success, string message)> RequestPasswordResetAsync(string email)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return (false, "Email không tồn tại trong hệ thống.");

        // Gửi Token loại PasswordReset
        await GenerateAndSendTokenAsync(user, TokenType.PasswordReset);
        return (true, "Vui lòng kiểm tra email để đặt lại mật khẩu.");
    }

    // 2. Thực hiện đổi mật khẩu mới bằng Token
    public async Task<(bool success, string message)> ResetPasswordWithTokenAsync(string tokenValue, string newPassword)
    {
        var nowUtc = DateTime.UtcNow;
        var token = await _db.AuthTokens.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == tokenValue
                                   && t.Type == TokenType.PasswordReset
                                   && !t.IsUsed);

        if (token == null || nowUtc > token.ExpiryDate)
            return (false, "Mã xác thực không hợp lệ hoặc đã hết hạn.");

        // Cập nhật mật khẩu mới (Băm bằng BCrypt)
        token.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        token.IsUsed = true; // Đánh dấu token đã dùng

        await _db.SaveChangesAsync();
        return (true, "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.");
    }

    public async Task<bool> LoginWithGoogleAsync()
    {
        try
        {
            // 1. Cấu hình luồng đăng nhập
            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets { ClientId = _clientId, ClientSecret = _clientSecret },
                new[] { "email", "profile" },
                "user_new_" + Guid.NewGuid().ToString().Substring(0, 8), // Dùng ID mới để ép Google login lại
                CancellationToken.None
            );
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                // Set clock tolerance if needed, or leave empty for default settings.
                // Example: ExpirationTimeClockTolerance = TimeSpan.FromMinutes(5)
            };

            // 2. Lấy thông tin chi tiết từ Token
            var payload = await GoogleJsonWebSignature.ValidateAsync(credential.Token.IdToken, settings);

            // 3. Kiểm tra User trong Database (Local SQLite trước)
            var user = await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);

                if (user == null)
                {
                    user = new User
                    {
                        GoogleId = payload.Subject,
                        Email = payload.Email,
                        FullName = payload.Name,
                        AvatarUrl = payload.Picture,
                        IsEmailConfirmed = true,
                        Role = "Student",

                        // THÊM CÁC DÒNG NÀY ĐỂ TRÁNH LỖI NULL DATABASE:
                        Username = payload.Email,         // Dùng tạm Email làm Username
                        PasswordHash = "GOOGLE_AUTH",     // Đánh dấu đây là user Google
                        FaceVector = "",                  // Mặc định rỗng
                        FaceEmbedding = "",               // Mặc định rỗng (ĐÂY LÀ CHỖ GÂY LỖI)
                        FaceImageUrl = ""                 // Mặc định rỗng
                    };
                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();
                }

                CurrentUser = user;
            SaveSession(user.UserId);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("LỖI ĐĂNG NHẬP: " + ex.Message);
            if (ex.InnerException != null)
                System.Diagnostics.Debug.WriteLine("CHI TIẾT: " + ex.InnerException.Message);

            return false;
        }
    }
    public async Task UpdateUserFaceDataAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllUsersWithFaceDataAsync()
    {
        // Lấy tất cả user đã có dữ liệu khuôn mặt
        return await _db.Users
            .Where(u => u.FaceEmbedding != null && u.FaceEmbedding != "")
            .ToListAsync();
    }
    public void SetCurrentUser(User user)
    {
        CurrentUser = user;
        SaveSession(user.UserId);
    }
}
}
