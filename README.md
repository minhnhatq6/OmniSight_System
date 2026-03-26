Chúc mừng bạn! Sau một quá trình nỗ lực, dự án OmniSight của bạn hiện đã có một bộ khung cực kỳ chuyên nghiệp, từ kiến trúc phần mềm (N-Tier), bảo mật (Mã hóa, xác thực email) cho đến trải nghiệm người dùng (Google OAuth, Deep Linking, Auto-login).

Dưới đây là file README.md đầy đủ và chuyên nghiệp nhất, tổng hợp toàn bộ các tính năng và công nghệ mà chúng ta đã cùng nhau xây dựng. Bạn hãy lưu nội dung này vào file README.md ở thư mục gốc của dự án.

🎓 OmniSight - AI-Powered Exam Supervision & Classroom Management

OmniSight là một hệ thống Desktop toàn diện được xây dựng trên nền tảng .NET 8 WinForms, ứng dụng kiến trúc đa tầng (N-Tier) và trí tuệ nhân tạo để quản lý lớp học trực tuyến và giám sát thi cử công bằng.

🏗 Kiến trúc Hệ thống (N-Tier Architecture)

Dự án được thiết kế chuẩn hóa để dễ dàng mở rộng và bảo trì:

OmniSight.UI: Tầng giao diện người dùng (Material Design 2). Quản lý Form, điều hướng và xử lý Deep Linking.

OmniSight.Services: Tầng nghiệp vụ (Business Logic). Xử lý xác thực Google, gửi Email SMTP, mã hóa mật khẩu và quản lý phiên (Session).

OmniSight.Data: Tầng truy cập dữ liệu (Data Access). Sử dụng Entity Framework Core để giao tiếp với SQL Server.

OmniSight.Core: Tầng lõi (Domain). Định nghĩa các thực thể (Entities), Enum và các hằng số dùng chung.

OmniSight.AI: Tầng xử lý trí tuệ nhân tạo (Đang phát triển nhận diện khuôn mặt và hành vi qua OpenCV).

🌟 Các tính năng đã hoàn thiện
1. Hệ thống Xác thực Đa phương thức (Authentication)

Đăng ký/Đăng nhập truyền thống: Sử dụng Email làm Username, mật khẩu được mã hóa chuẩn BCrypt.

Google OAuth 2.0: Đăng nhập một chạm bằng tài khoản Google.

Liên kết tài khoản: Tự động liên kết tài khoản Google với tài khoản Email đã đăng ký trước đó nếu trùng địa chỉ Email.

Thiết lập mật khẩu: Yêu cầu người dùng đăng nhập qua Google lần đầu thiết lập mật khẩu để có thể dùng song song cả hai cách đăng nhập.

2. Bảo mật & Xác thực (Security)

Xác thực Email: Gửi mã Token xác nhận qua Gmail ngay khi đăng ký.

Deep Linking (Custom Protocol): Hỗ trợ link omnisight:// trong Email. Khi bấm nút "Xác thực" trên trình duyệt, ứng dụng sẽ tự động bật lên và kích hoạt tài khoản.

SMTP App Password: Cấu hình gửi mail bảo mật qua Google App Password.

3. Trải nghiệm Người dùng (UX)

Auto-Login (Session): Ghi nhớ trạng thái đăng nhập vào file cục bộ (session.json), tự động vào Dashboard khi mở App lần sau.

Đăng xuất an toàn: Chức năng Application.Restart() giúp xóa sạch phiên làm việc và quay lại màn hình đăng nhập.

Material UI 2: Giao diện phẳng hiện đại, hỗ trợ Drawer (Menu trượt bên trái) giống Google Classroom.

4. Quản lý Hồ sơ & Phân quyền (Profile & Roles)

Flexible Roles: Một người dùng có thể vừa là Giáo viên, vừa là Học sinh (Bật/tắt linh hoạt trong Profile).

Dashboard thông minh: Menu và chức năng (Tạo lớp/Tham gia lớp) tự động ẩn/hiện dựa trên Role người dùng.

Account Menu: Tên người dùng hiển thị ở góc phải thanh tiêu đề, hỗ trợ Context Menu để Đăng xuất nhanh.

🛠 Công nghệ & Thư viện sử dụng (Tech Stack)
Thành phần	Công nghệ
Ngôn ngữ	C# / .NET 8.0 (Windows Forms)
Database	Microsoft SQL Server
ORM	Entity Framework Core 8.0
Giao diện	MaterialSkin.2
Xác thực	Google.Apis.Auth (OAuth 2.0)
Email	MailKit & MimeKit (SMTP)
Mã hóa	BCrypt.Net-Next
Deep Link	Windows Registry (Custom Protocol)
🚀 Hướng dẫn Cài đặt
1. Cấu hình file appsettings.json

Tạo file appsettings.json trong project OmniSight.UI với nội dung sau:

code
JSON
download
content_copy
expand_less
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=OmniSightDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "GoogleAuth": {
    "ClientId": "YOUR_CLIENT_ID.apps.googleusercontent.com",
    "ClientSecret": "YOUR_CLIENT_SECRET"
  },
  "EmailSettings": {
    "Sender": "your_email@gmail.com",
    "AppPassword": "xxxx xxxx xxxx xxxx"
  }
}

Lưu ý: Chuột phải vào file chọn Properties -> Copy to Output Directory -> Copy if newer.

2. Cài đặt NuGet Packages

Mở Package Manager Console và cài đặt:

code
Powershell
download
content_copy
expand_less
# Project Data
Install-Package Microsoft.EntityFrameworkCore.SqlServer

# Project Services
Install-Package Google.Apis.Auth
Install-Package MailKit
Install-Package BCrypt.Net-Next

# Project UI
Install-Package MaterialSkin.2
Install-Package Microsoft.Extensions.Hosting
Install-Package System.Web.HttpUtility
3. Khởi tạo Database
code
Powershell
download
content_copy
expand_less
Add-Migration InitialCreate -StartupProject OmniSight.UI
Update-Database -StartupProject OmniSight.UI
📂 Cấu trúc thư mục chính
code
Text
download
content_copy
expand_less
OmniSight_System/
├── OmniSight.Core/        # Entities (User, Class, AuthToken...)
├── OmniSight.Data/        # DbContext, Migrations
├── OmniSight.Services/    # AuthService, UserService
└── OmniSight.UI/
    ├── Forms/
    │   ├── Auth/          # FrmLogin, FrmRegister, FrmSetPassword
    │   └── MainForm.cs    # Dashboard chính (Google Classroom Style)
    ├── Program.cs         # Khởi tạo App & Deep Linking
    └── appsettings.json   # Cấu hình hệ thống
📝 Các mục tiêu tiếp theo

Tích hợp OpenCvSharp4 để xử lý Camera.

Xây dựng hệ thống FaceID Enrollment (Định danh khuôn mặt).

Logic Tạo lớp học/Tham gia lớp học qua Join Code.

Hệ thống thi trắc nghiệm thời gian thực.

OmniSight Team - Smarter Management, Fairer Exams.