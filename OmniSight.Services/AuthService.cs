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
using System.Globalization;

namespace OmniSight.Services
{
    public class AuthService
    {
        private readonly OmniSightDbContext _db;
        private static readonly string SessionFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "OmniSight", "session.json");

        public User? CurrentUser { get; private set; }

        private readonly string? _clientId;
        private readonly string? _clientSecret;
        private readonly string _emailSender;
        private readonly string _emailPassword;

        public AuthService(OmniSightDbContext db, IConfiguration configuration)
        {
            _db = db;
            _clientId = configuration["GoogleAuth:ClientId"];
            _clientSecret = configuration["GoogleAuth:ClientSecret"];
            _emailSender = configuration["EmailSettings:Sender"] ?? "";
            _emailPassword = configuration["EmailSettings:AppPassword"] ?? "";
        }

        // --- 1. ĐĂNG KÝ (Email làm Username) ---
        public async Task<(bool success, string message, User? user)> RegisterAsync(string email, string password, string fullName)
        {
            if (await _db.Users.AnyAsync(u => u.Email == email))
            {
                return (false, "Email này đã được sử dụng.", null);
            }

            var user = new User
            {
                Email = email,
                Username = email, // Email chính là Username
                FullName = fullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsStudent = true,
                IsEmailConfirmed = false
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            await GenerateAndSendTokenAsync(user, TokenType.EmailVerification);

            return (true, string.Empty, user);
        }

        // --- 2. ĐĂNG NHẬP EMAIL/PASSWORD ---
        public async Task<(bool success, string message)> LoginWithEmailAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email || u.Username == email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                return (false, "Email hoặc mật khẩu không chính xác.");
            }

            // Kiểm tra pass "GOOGLE_AUTH" (trường hợp user chưa set pass)
            if (user.PasswordHash == "GOOGLE_AUTH")
            {
                return (false, "Tài khoản này cần đăng nhập qua Google hoặc thiết lập mật khẩu mới.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return (false, "Email hoặc mật khẩu không chính xác.");
            }

            if (!user.IsEmailConfirmed)
            {
                return (false, "Email chưa được xác thực. Vui lòng kiểm tra hộp thư.");
            }

            CurrentUser = user;
            SaveSession(user.UserId);
            return (true, string.Empty);
        }

        // --- 3. ĐĂNG NHẬP GOOGLE + LIÊN KẾT TÀI KHOẢN ---
        public async Task<bool> LoginWithGoogleAsync()
        {
            try
            {
                UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets { ClientId = _clientId, ClientSecret = _clientSecret },
                    new[] { "email", "profile" },
                    "user_" + Guid.NewGuid().ToString().Substring(0, 8),
                    CancellationToken.None
                );

                var payload = await GoogleJsonWebSignature.ValidateAsync(credential.Token.IdToken);

                // Kiểm tra theo GoogleId
                var user = await _db.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);

                if (user == null)
                {
                    // Nếu chưa có GoogleId, tìm theo Email để liên kết
                    user = await _db.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

                    if (user != null)
                    {
                        user.GoogleId = payload.Subject;
                        if (string.IsNullOrEmpty(user.AvatarUrl)) user.AvatarUrl = payload.Picture;
                        _db.Users.Update(user);
                    }
                    else
                    {
                        // Tạo mới hoàn toàn
                        user = new User
                        {
                            GoogleId = payload.Subject,
                            Email = payload.Email,
                            Username = payload.Email,
                            FullName = payload.Name,
                            AvatarUrl = payload.Picture,
                            IsEmailConfirmed = true,
                            PasswordHash = "GOOGLE_AUTH", // Đánh dấu chưa có mật khẩu thực
                            IsStudent = true
                        };
                        _db.Users.Add(user);
                    }
                    await _db.SaveChangesAsync();
                }

                CurrentUser = user;
                SaveSession(user.UserId);
                return true;
            }
            catch { return false; }
        }

        // --- CÁC HÀM HỖ TRỢ (SESSION, TOKEN, MAIL) ---

        private void SaveSession(int userId)
        {
            try
            {
                var directory = Path.GetDirectoryName(SessionFilePath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory!);
                var data = new { UserId = userId, LastActive = DateTime.UtcNow };
                File.WriteAllText(SessionFilePath, JsonSerializer.Serialize(data));
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        public void Logout()
        {
            CurrentUser = null;
            if (File.Exists(SessionFilePath)) File.Delete(SessionFilePath);
        }

        public async Task<bool> TryAutoLoginAsync()
        {
            if (!File.Exists(SessionFilePath)) return false;
            try
            {
                var json = await File.ReadAllTextAsync(SessionFilePath);
                var data = JsonSerializer.Deserialize<JsonElement>(json);
                if (data.TryGetProperty("UserId", out var idElement))
                {
                    int userId = idElement.GetInt32();
                    var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                    if (user != null) { CurrentUser = user; return true; }
                }
            }
            catch { }
            return false;
        }

        public async Task GenerateAndSendTokenAsync(User user, TokenType type)
        {
            var oldTokens = _db.AuthTokens.Where(t => t.UserId == user.UserId && t.Type == type);
            foreach (var t in oldTokens) t.IsUsed = true;

            var tokenValue = Guid.NewGuid().ToString("N");
            var token = new AuthToken { UserId = user.UserId, Token = tokenValue, Type = type, ExpiryDate = DateTime.UtcNow.AddMinutes(15) };
            _db.AuthTokens.Add(token);
            await _db.SaveChangesAsync();

            string subject = type == TokenType.EmailVerification ? "Xác nhận Email" : "Đặt lại mật khẩu";
            string actionUrl = $"https://lekhai0123.github.io/omnisight-email-bridge/open.html?token={tokenValue}&email={user.Email}";

            await SendMailKitAsync(user.Email!, subject, $"Link: {actionUrl}", $"Vui lòng mở link: {actionUrl}");
        }

        private async Task SendMailKitAsync(string to, string subject, string htmlBody, string textBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("OmniSight", _emailSender));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody, TextBody = textBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSender, _emailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task UpdateUserFaceDataAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
            SaveSession(user.UserId);
        }
        public async Task<(bool success, User? user)> VerifyTokenAsync(string tokenValue, TokenType type)
        {
            var nowUtc = DateTime.UtcNow;

            // Tìm token trong DB
            var token = await _db.AuthTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == tokenValue
                                       && t.Type == type
                                       && !t.IsUsed);

            if (token == null || nowUtc > token.ExpiryDate)
            {
                return (false, null);
            }

            // Đánh dấu token đã sử dụng
            token.IsUsed = true;

            // Nếu là xác thực Email thì cập nhật trạng thái User
            if (type == TokenType.EmailVerification)
            {
                token.User.IsEmailConfirmed = true;
            }

            await _db.SaveChangesAsync();
            return (true, token.User);
        }

        // --- HÀM KIỂM TRA TOKEN RESET MẬT KHẨU ---
        public async Task<bool> ValidateResetTokenAsync(string tokenValue)
        {
            var nowUtc = DateTime.UtcNow;
            var token = await _db.AuthTokens
                .FirstOrDefaultAsync(t => t.Token == tokenValue
                                       && t.Type == TokenType.PasswordReset
                                       && !t.IsUsed);

            if (token == null || nowUtc > token.ExpiryDate) return false;
            return true;
        }
        // 1. Hàm lấy tất cả User đã định danh khuôn mặt
        public async Task<List<User>> GetAllUsersWithFaceDataAsync()
        {
            return await _db.Users
                .Where(u => u.FaceEmbedding != null && u.FaceEmbedding != "")
                .ToListAsync();
        }

        // Thay thế hàm tính khoảng cách cũ bằng hàm tính độ tương đồng Cosine
        private double CalculateCosineSimilarity(float[] v1, float[] v2)
        {
            double dotProduct = 0, normA = 0, normB = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                dotProduct += v1[i] * v2[i];
                normA += v1[i] * v1[i];
                normB += v2[i] * v2[i];
            }
            return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }

        public async Task<(bool success, User? user)> LoginWithFaceAsync(float[] currentEmbedding)
        {
            try
            {
                var usersWithFace = await GetAllUsersWithFaceDataAsync();
                User? matchedUser = null;
                double maxSimilarity = -1;
                double threshold = 0.32; // Giảm xuống 0.32 để AI "dễ tính" hơn một chút

                foreach (var user in usersWithFace)
                {
                    var savedEmbedding = user.FaceEmbedding.Replace(',', '.')
                                             .Split(';')
                                             .Select(s => float.Parse(s, CultureInfo.InvariantCulture))
                                             .ToArray();

                    double similarity = CalculateCosineSimilarity(currentEmbedding, savedEmbedding);

                    // DÒNG NÀY ĐỂ DEBUG: Bạn nhìn vào cửa sổ Output trong VS sẽ thấy số này
                    System.Diagnostics.Debug.WriteLine($"So khớp với {user.FullName}: Độ tương đồng = {similarity:F4}");

                    if (similarity > maxSimilarity)
                    {
                        maxSimilarity = similarity;
                        matchedUser = user;
                    }
                }

                if (maxSimilarity >= threshold && matchedUser != null)
                {
                    CurrentUser = matchedUser;
                    SaveSession(matchedUser.UserId);
                    return (true, matchedUser);
                }
                return (false, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LỖI AI SO KHỚP: " + ex.Message);
                return (false, null);
            }
        }
    }
}