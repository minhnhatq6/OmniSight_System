# OmniSight System 👁️✨

**OmniSight** là một hệ thống ứng dụng máy tính dành cho giáo dục (Desktop App), cho phép quản lý lớp học tương tác và hỗ trợ điểm danh hoặc xác thực bảo mật bằng công nghệ **Nhận diện khuôn mặt (Face ID) tiên tiến**. Ứng dụng được xây dựng trên nền tảng **.NET 8** sử dụng giao diện **Windows Forms** kết hợp với thư viện thiết kế **MaterialSkin**.

---

## 🚀 Tính năng chính

- **Xác thực người dùng (Authentication):**
  - Đăng ký và Đăng nhập.
  - Quản lý phiên đăng nhập an toàn, lưu trữ hồ sơ người dùng.
- **Tích hợp AI Nhận diện khuôn mặt (Face ID):**
  - Hỗ trợ thiết lập Face ID trực tiếp qua Camera.
  - Sử dụng các mô hình trích xuất và nhận diện khuôn mặt **ONNX** tích hợp sẵn qua `Emgu.CV` (YUNET cho Detection & SFACE cho Recognition).
- **Phân quyền linh hoạt:**
  - Hỗ trợ đa vai trò: **Giáo viên** và **Học sinh**.
- **Quản lý không gian học tập:**
  - Thiết lập và Tạo mới Lớp học (Dành cho giáo viên).
  - Tham gia Lớp học bằng mã mời (Dành cho học sinh).
  - Khám phá thông tin chi tiết Lớp học qua `UcClassDetail`.
- **Giao diện hiện đại, thân thiện:**
  - Tích hợp chuẩn **Material Design** với chế độ hiển thị Drawer Tab cho phép điều hướng mượt mà, trực quan.

---

## 🛠️ Công nghệ sử dụng

- **Framework:** .NET 8.
- **Giao diện (UI):** Windows Forms (WinForms) + MaterialSkin.2.
- **Cơ sở dữ liệu:** Entity Framework Core (EF Core) lưu trữ qua SQL Server / SQLite.
- **Xử lý Ảnh & AI:** Emgu.CV (Wrapper OpenCV cho .NET) cùng với ONNX (Open Neural Network Exchange).
- **Kiến trúc mã nguồn:** Phân tách nhiều tầng rõ ràng (Clean Structure).

---

## 📂 Cấu trúc dự án (Project Structure)

Dự án được chia làm 4 thư mục/dự án con (Modules) chính:

1. **`OmniSight.Core`**: Nơi định nghĩa các thực thể (Entities/Models) cốt lõi của bài toán, ví dụ như `User`, `Class`, `Stream`.
2. **`OmniSight.Data`**: Chứa Cấu hình Database, DbContext (`OmniSightDbContext`) và quản lý các bản ghi Migration của quá trình phát triển thông qua Entity Framework.
3. **`OmniSight.Services`**: Nơi chứa logic nghiệp vụ và các tính toán xử lý dữ liệu phức tạp. Có các dịch vụ nổi bật như `AuthService` (Xử lý đăng nhập), `FaceAiService` (Tương tác Camera & AI), `IUserService` (Xử lý thông tin người dùng).
4. **`OmniSight.UI`**: Lớp hiển thị (View), chứa giao diện người dùng và Controller. Điển hình là các Form như `MainForm`, `FrmLogin`, `FrmCreateClass`, cấu hình App `appsettings.json`,...

---

## ⚙️ Hướng dẫn cài đặt và khởi chạy

### Yêu cầu hệ thống:
- Visual Studio 2022 (Phiên bản v17.8+ hỗ trợ .NET 8).
- Cài đặt SDK .NET 8.0.
- Camera / Webcam trên máy tính hoạt động ổn định.

### Các bước cài đặt:

1. **Clone repository về máy:**
```bash
git clone https://github.com/<your-account>/OmniSight.git
```

2. **Mở giải pháp trong Visual Studio:**
   - Mở file `OmniSight.sln` trong thư mục vừa clone.

3. **Cài đặt các gói NuGet cần thiết:**
   - Mở **Package Manager Console** và nhập lần lượt các lệnh sau:
     ```powershell
     # Cài đặt cho Entity Framework Core với SQL Server
     Install-Package Microsoft.EntityFrameworkCore.SqlServer
     
     # Cài đặt cho Google Authentication
     Install-Package Google.Apis.Auth
     
     # Cài đặt cho MailKit sử dụng SMTP
     Install-Package MailKit
     
     # Cài đặt BCrypt cho mã hóa mật khẩu
     Install-Package BCrypt.Net-Next
     
     # Cài đặt MaterialSkin cho giao diện
     Install-Package MaterialSkin.2
     
     # Cài đặt hỗ trợ Hosting và các tiện ích khác
     Install-Package Microsoft.Extensions.Hosting
     Install-Package System.Web.HttpUtility
     ```

4. **Cấu hình kết nối cơ sở dữ liệu và các dịch vụ bên ngoài:**
   - Tạo file `appsettings.json` trong thư mục `OmniSight.UI` với cấu trúc như sau:
     ```json
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
     ```
   - Lưu ý: Thay thế các giá trị `YOUR_SERVER`, `YOUR_CLIENT_ID`, `YOUR_CLIENT_SECRET` bằng thông tin thật của bạn. Đối với trường `AppPassword`, bạn cần tạo một mật khẩu ứng dụng mới trong tài khoản Google của mình để tăng cường bảo mật.

5. **Chạy ứng dụng:**
   - Nhấn `F5` hoặc chọn **Debug -> Start Debugging** trong Visual Studio để biên dịch và chạy ứng dụng.
   - Kiểm tra kết nối cơ sở dữ liệu và các tính năng đăng nhập, đăng ký người dùng.

---

Chúc bạn thành công và có những trải nghiệm tuyệt vời cùng OmniSight! Hệ thống sẽ liên tục được cập nhật và nâng cấp các tính năng mới trong tương lai. Hãy theo dõi và đóng góp ý kiến để dự án ngày càng hoàn thiện hơn nữa.

OmniSight Team - Smarter Management, Fairer Exams.