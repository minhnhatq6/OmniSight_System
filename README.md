Đây là file README.md đầy đủ và chuyên nghiệp dành cho đồ án OmniSight của bạn. Bạn có thể copy nội dung này vào một file tên là README.md ở thư mục gốc của Solution.

🎓 OmniSight - Hệ thống Giám sát Thi cử & Quản lý Lớp học AI

OmniSight là một ứng dụng Desktop mạnh mẽ được xây dựng trên nền tảng .NET WinForms, kết hợp với trí tuệ nhân tạo (AI) để tối ưu hóa việc quản lý lớp học và đảm bảo tính minh bạch trong các kỳ thi trực tuyến.

🏗 1. Kiến trúc Hệ thống (Architecture)

Dự án được thiết kế theo mô hình N-Tier Architecture (Kiến trúc đa tầng) để đảm bảo tính mở rộng và dễ bảo trì:

OmniSight.UI (Presentation Layer): Giao diện người dùng WinForms sử dụng MaterialSkin 2 để mang lại trải nghiệm hiện đại như Google Classroom.

OmniSight.Services (Business Logic Layer): Xử lý logic nghiệp vụ chính bao gồm xác thực Google OAuth2, gửi Email qua SMTP, và quản lý phiên làm việc (Session).

OmniSight.Data (Data Access Layer): Sử dụng Entity Framework Core để giao tiếp với SQL Server. Quản lý cấu hình Database và các ràng buộc dữ liệu phức tạp.

OmniSight.Core (Domain Layer): Chứa các thực thể (Entities), Enum và các hằng số dùng chung cho toàn bộ hệ thống.

OmniSight.AI (AI Core Layer): Tầng xử lý các thuật toán nhận diện khuôn mặt, bắt ánh mắt và âm thanh (Đang phát triển).

📂 2. Cấu trúc thư mục chi tiết
code
Text
download
content_copy
expand_less
OmniSight_System/
├── OmniSight.Core/             # Các lớp thực thể (Entities)
│   ├── Entities/               # User, Class, Exam, Assignment, AuthToken...
│   └── Helpers/                # Các lớp hỗ trợ logic nhỏ
├── OmniSight.Data/             # Kết nối Database
│   ├── OmniSightDbContext.cs   # Cấu hình EF Core & Fluent API
│   └── Migrations/             # Lịch sử cập nhật Database
├── OmniSight.Services/         # Logic xử lý nghiệp vụ
│   ├── AuthService.cs          # Đăng nhập Google, Email, Register...
│   └── EmailTemplates.cs       # Mẫu giao diện Email HTML
├── OmniSight.UI/               # Giao diện chính (WinForms)
│   ├── Forms/                  # Auth, Teacher, Student Dashboards
│   ├── appsettings.json        # Cấu hình ClientID, Chuỗi kết nối DB
│   └── Program.cs              # Cấu hình Dependency Injection (DI)
└── OmniSight.AI/               # Lõi xử lý AI
🛠 3. Công nghệ & Thư viện sử dụng (Tech Stack)
Backend & Core

.NET 8.0/6.0 WinForms: Nền tảng phát triển chính.

Entity Framework Core: Ánh xạ dữ liệu SQL Server.

Dependency Injection (Microsoft.Extensions.DependencyInjection): Quản lý vòng đời các đối tượng.

BCrypt.Net-Next: Mã hóa mật khẩu an toàn.

Xác thực & Giao tiếp

Google OAuth 2.0: Đăng nhập một chạm qua tài khoản Google.

MailKit & MimeKit: Hệ thống gửi Email xác thực và Reset mật khẩu.

System.Text.Json: Xử lý dữ liệu JSON cho Session lưu trữ cục bộ.

Giao diện (UI)

MaterialSkin.2: Bộ thư viện thiết kế Material Design cho WinForms.

🚀 4. Hướng dẫn cài đặt & Vận hành
Bước 1: Cài đặt các NuGet Packages

Mở Package Manager Console và cài đặt cho từng project:

Project UI:

code
Powershell
download
content_copy
expand_less
Install-Package MaterialSkin.2
Install-Package Microsoft.Extensions.Hosting
Install-Package Microsoft.Extensions.Configuration.Json

Project Data:

code
Powershell
download
content_copy
expand_less
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools

Project Services:

code
Powershell
download
content_copy
expand_less
Install-Package Google.Apis.Auth
Install-Package MailKit
Bước 2: Cấu hình appsettings.json

Cập nhật thông tin trong file OmniSight.UI/appsettings.json:


{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=OmniSightDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },

Bước 3: Khởi tạo Database

Chạy các lệnh sau trong Package Manager Console (Chọn Default project là OmniSight.Data):

Add-Migration InitialCreate -StartupProject OmniSight.UI

Update-Database -StartupProject OmniSight.UI

💡 5. Cách thức hoạt động của Chức năng Đăng nhập

Đăng nhập Google: Ứng dụng mở trình duyệt qua GoogleWebAuthorizationBroker. Sau khi xác thực thành công, Google trả về một Identity Token.

Xử lý dữ liệu: AuthService kiểm tra GoogleId trong Database.

Nếu chưa có: Tự động tạo tài khoản mới với vai trò mặc định là Student.

Nếu đã có: Lấy thông tin User hiện tại.

Quản lý Session: Sau khi đăng nhập, UserId được lưu vào một file session.json trong thư mục AppData/Local. Khi mở lại ứng dụng, TryAutoLoginAsync sẽ đọc file này để tự động đăng nhập mà không cần User thao tác lại.

Phòng chống lỗi Null: Các trường AI như FaceEmbedding, FaceVector được khởi tạo giá trị rỗng ("") để đảm bảo không vi phạm ràng buộc NOT NULL của Database khi User mới chưa thực hiện định danh khuôn mặt.

📝 6. Các tính năng sắp tới

AI Face Enrollment: Chụp ảnh gốc và trích xuất Vector đặc trưng khuôn mặt.

Classroom Manager: Tạo lớp học qua Join Code.

AI Monitoring: Cảnh báo quay cóp qua Head Pose và Eye Gaze.

Exam Sync: Đồng bộ dữ liệu Offline từ SQLite lên SQL Server trung tâm.

OmniSight Team - Smarter Management, Fairer Exams.