# Hệ Thống Quản Lý Bài Thi - OmniSight

## 📋 Tổng Quan Hệ Thống

Hệ thống quản lý bài thi gồm các thành phần chính:

### 1. **Phía Giáo Viên (Teacher)**
   - **Quản lý bài thi**: Tạo, sửa, xóa bài thi
   - **Quản lý câu hỏi**: Thêm, sửa, xóa câu hỏi trắc nghiệm (4 đáp án)
   - **Xem kết quả**: Theo dõi điểm số và thời gian làm bài của học sinh

### 2. **Phía Học Sinh (Student)**
   - **Xem danh sách bài thi**: Các bài thi của lớp học
   - **Làm bài thi**: Giao diện với bộ đếm thời gian
   - **Nộp bài**: Tự động tính điểm và lưu kết quả

---

## 🎯 Các Bước Sử Dụng

### **A. PHÍA GIÁO VIÊN**

#### 1️⃣ Mở Form Quản Lý Bài Thi
```
Settings → "Quản Lý Đề Thi" 
hoặc Menu → "🔧 Quản Lý Đề Thi"
```

#### 2️⃣ Tạo Bài Thi Mới
**Tab "Danh Sách Bài Thi":**
- Nhấp "➕ Tạo Bài Thi"
- Điền thông tin:
  - **Tên Bài Thi**: Ví dụ: "Kiểm tra Giữa Kì - Toán 10"
  - **Lớp Học**: Chọn từ dropdown
  - **Thời Gian**: Số phút tối đa (mặc định 60 phút)

#### 3️⃣ Thêm Câu Hỏi
**Tab "Chi Tiết Bài Thi":**
- Nhấp "➕ Thêm Câu"
- Điền nội dung:
  - **Nội Dung Câu Hỏi**: Văn bản câu hỏi
  - **Đáp án A, B, C, D**: 4 lựa chọn
  - **Đáp Án Đúng**: Chọn A, B, C hoặc D
- Nhấp "💾 Lưu"

#### 4️⃣ Sửa Hoặc Xóa
- **Sửa bài thi**: Chọn bài → "✏️ Sửa"
- **Xóa bài thi**: Chọn bài → "🗑️ Xóa" (cảnh báo: xóa cả kết quả)
- **Quản lý câu hỏi**: Trong Tab "Chi Tiết Bài Thi"
  - "✏️ Sửa Câu" - Sửa nội dung
  - "🗑️ Xóa Câu" - Xóa câu hỏi

#### 5️⃣ Xem Kết Quả Học Sinh
**Tab "Kết Quả Học Sinh":**
- **Lọc theo bài thi**: Chọn bài từ dropdown
- **Xem thông tin**:
  - Tên học sinh
  - Điểm số (0-10)
  - Thời gian bắt đầu
  - Thời gian kết thúc
  - Thời gian làm bài (tính bằng phút)

---

### **B. PHÍA HỌC SINH**

#### 1️⃣ Xem Danh Sách Bài Thi
```
Chọn lớp học → Tab "Đề Thi"
```

#### 2️⃣ Làm Bài Thi
- Nhấp "🚀 Làm Thi" trên bài thi cần làm
- **Giao diện làm bài**:
  - ⏱️ **Bộ đếm thời gian**: Hiển thị thời gian còn lại
  - 📋 **Câu hỏi hiện tại**: Nội dung câu hỏi
  - 🔘 **4 tùy chọn**: A, B, C, D (Radio button)
  - ⬅️ **Nút Previous**: Quay lại câu trước
  - ➡️ **Nút Next**: Sang câu tiếp theo
  - 💾 **Nút Submit**: Nộp bài

#### 3️⃣ Navigating Between Questions
- Danh sách câu hỏi ở bên cạnh:
  - 🟢 **Xanh lá**: Câu đã trả lời
  - ⚫ **Xám**: Câu chưa trả lời
- Nhấp số câu hỏi để chuyển nhanh

#### 4️⃣ Nộp Bài
- Nhấp "💾 Nộp Bài"
- **Xác nhận**: Hiện số câu đã trả lời
- Nhấp "Yes" để xác nhận nộp

#### 5️⃣ Kết Quả
Sau khi nộp bài, hiện thông báo:
```
✅ Nộp Bài Thành Công!
Kết Quả: X/Y câu đúng
Điểm: Z.Z/10
```

---

## 🗄️ Cấu Trúc Dữ Liệu

### **Database Entities**

#### **Exam** (Bài Thi)
```csharp
public class Exam
{
    public int ExamId { get; set; }           // ID bài thi
    public int ClassId { get; set; }          // ID lớp học
    public string Title { get; set; }         // Tên bài thi
    public int DurationMinutes { get; set; }  // Thời gian làm bài (phút)
    public DateTime CreatedAt { get; set; }   // Ngày tạo

    // Navigation
    public virtual Class Class { get; set; }
    public virtual ICollection<Question> Questions { get; set; }
    public virtual ICollection<ExamResult> ExamResults { get; set; }
}
```

#### **Question** (Câu Hỏi)
```csharp
public class Question
{
    public int QuestionId { get; set; }     // ID câu hỏi
    public int ExamId { get; set; }         // ID bài thi
    public string Content { get; set; }     // Nội dung câu hỏi
    public string OptionA { get; set; }     // Đáp án A
    public string OptionB { get; set; }     // Đáp án B
    public string OptionC { get; set; }     // Đáp án C
    public string OptionD { get; set; }     // Đáp án D
    public string CorrectOption { get; set; } // Đáp án đúng (A/B/C/D)

    public virtual Exam Exam { get; set; }
}
```

#### **ExamResult** (Kết Quả)
```csharp
public class ExamResult
{
    public int ResultId { get; set; }           // ID kết quả
    public int ExamId { get; set; }             // ID bài thi
    public int StudentId { get; set; }          // ID học sinh
    public float? Score { get; set; }           // Điểm (0-10)
    public DateTime? StartedAt { get; set; }    // Thời gian bắt đầu
    public DateTime? CompletedAt { get; set; }  // Thời gian nộp bài

    // Navigation
    public virtual Exam Exam { get; set; }
    public virtual User Student { get; set; }
    public virtual ICollection<ViolationLog> ViolationLogs { get; set; }
}
```

---

## 📊 Luồng Xử Lý Dữ Liệu

### **1. Tạo Bài Thi**
```
FrmExamManagement
  ↓
ExamService.CreateExamAsync()
  ↓
Database: INSERT INTO Exams
  ↓
Hiện thông báo thành công
```

### **2. Thêm Câu Hỏi**
```
FrmQuestionEditor
  ↓
ExamService.CreateQuestionAsync(question)
  ↓
Database: INSERT INTO Questions
  ↓
Cập nhật danh sách câu hỏi
```

### **3. Bắt Đầu Làm Bài (Student)**
```
UcClassDetail.CreateExamCard() → "🚀 Làm Thi"
  ↓
ExamService.StartExamAsync(examId, studentId)
  ↓
Database: INSERT INTO ExamResults 
  (StartedAt = NOW, Score = NULL)
  ↓
Open FrmTakeExam(exam, examResult, examService)
```

### **4. Làm Bài Thi**
```
FrmTakeExam.LoadQuestionsAsync()
  ↓
ExamService.GetQuestionsByExamIdAsync(examId)
  ↓
Display câu hỏi
  ↓
User chọn đáp án
  ↓
Lưu vào Dictionary<QuestionId, SelectedOption>
```

### **5. Nộp Bài Thi**
```
FrmTakeExam.SubmitExamAsync()
  ↓
Tính điểm: 
  correctCount = 0
  FOREACH answer IN _answers:
    IF answer.Value == question.CorrectOption:
      correctCount++
  score = (correctCount * 10) / totalQuestions
  ↓
ExamResult.Score = score
ExamResult.CompletedAt = NOW
  ↓
ExamService.UpdateExamResultAsync(examResult)
  ↓
Database: UPDATE ExamResults SET Score=?, CompletedAt=?
  ↓
Hiện kết quả & đóng form
```

### **6. Xem Kết Quả (Teacher)**
```
FrmExamManagement Tab "Kết Quả Học Sinh"
  ↓
Select bài thi từ ComboBox
  ↓
ExamService.GetExamResultsAsync(examId)
  ↓
Database: SELECT * FROM ExamResults 
  WHERE ExamId = ? AND Score IS NOT NULL
  JOIN User (để lấy FullName)
  ↓
Hiển thị DataGridView:
  - StudentName
  - Score (0-10)
  - StartedAt
  - CompletedAt
  - Duration (phút)
```

---

## ⚙️ Service Methods

### **ExamService**

#### **Tạo/Cập Nhật**
```csharp
// Tạo bài thi
Task<Exam> CreateExamAsync(int classId, string title, int durationMinutes)

// Cập nhật bài thi
Task<Exam> UpdateExamAsync(Exam exam)

// Tạo câu hỏi
Task<Question> CreateQuestionAsync(Question question)

// Cập nhật câu hỏi
Task<Question> UpdateQuestionAsync(Question question)

// Cập nhật kết quả
Task<ExamResult> UpdateExamResultAsync(ExamResult examResult)
```

#### **Lấy Dữ Liệu**
```csharp
// Lấy bài thi theo lớp
Task<List<Exam>> GetExamsByClassIdAsync(int classId)

// Lấy bài thi của giáo viên
Task<List<Exam>> GetExamsForTeacherAsync(int teacherId)

// Lấy bài thi của học sinh
Task<List<Exam>> GetExamsForStudentAsync(int studentId)

// Lấy câu hỏi theo bài thi
Task<List<Question>> GetQuestionsByExamIdAsync(int examId)

// Lấy kết quả bài thi
Task<List<ExamResult>> GetExamResultsAsync(int examId)

// Bắt đầu làm bài (tạo ExamResult record)
Task<ExamResult> StartExamAsync(int examId, int studentId)
```

#### **Xóa**
```csharp
// Xóa bài thi (tất cả kết quả cũng bị xóa)
Task DeleteExamAsync(int examId)

// Xóa câu hỏi
Task DeleteQuestionAsync(int questionId)
```

---

## 🔐 Chuẩn Bị Cho Mục 5: Chống Gian Lận

Cấu trúc hiện tại đã chuẩn bị cho:

### **ViolationLog** (Sẵn sàng trong ExamResult)
```csharp
public class ViolationLog
{
    public int ViolationId { get; set; }
    public int ResultId { get; set; }
    public ViolationType Type { get; set; }  
    // Types: EyeMovement, HeadTurn, MultipleObjects, 
    //        ScreenCapture, AltTab, etc.
    public DateTime DetectedAt { get; set; }
    public float Confidence { get; set; }    // 0-1

    public virtual ExamResult ExamResult { get; set; }
}

public enum ViolationType
{
    EyeMovement,      // Con mắt không nhìn vào màn hình
    HeadTurn,         // Quay đầu
    MultipleObjects,  // Phát hiện nhiều người
    ScreenCapture,    // Chụp màn hình
    AltTab,          // Chuyển ứng dụng
    CopyPaste        // Copy/Paste text
}
```

### **FaceAiService** (Sẵn sàng)
- ✅ Detect faces
- ✅ Recognize individuals
- ✅ Track eye gaze direction
- ✅ Sẵn sàng để thêm:
  - Eye movement detection
  - Head position tracking
  - Object/Person count
  - Anomaly detection

### **Mục 5 - Tính Năng Chống Gian Lận (Chưa Implement)**
1. **Real-time Monitoring** trong FrmTakeExam
2. **ViolationLog Recording** khi phát hiện lỗi
3. **Warning System** để cảnh báo người dùng
4. **Automatic Submission** nếu gian lận nghiêm trọng
5. **Report Dashboard** cho giáo viên xem vi phạm

---

## 📱 Forms Chính

### **FrmExamManagement** (Giáo Viên)
- **Tab 1**: Danh sách bài thi (tạo, sửa, xóa)
- **Tab 2**: Chi tiết bài thi (quản lý câu hỏi)
- **Tab 3**: Kết quả học sinh (xem điểm)

### **FrmQuestionEditor** (Giáo Viên)
- Modal form để thêm/sửa câu hỏi
- 4 text fields cho đáp án
- Dropdown chọn đáp án đúng

### **FrmTakeExam** (Học Sinh)
- Bộ đếm thời gian
- Hiển thị câu hỏi + 4 đáp án (Radio buttons)
- Navigation buttons (Previous/Next)
- List câu hỏi bên cạnh (tracking trạng thái)
- Submit button

---

## 🎨 UI/UX Notes

### **Color Scheme**
- ✅ Green (#4CAF50): Action buttons (Create, Submit)
- 🔧 Blue (#2196F3): Edit buttons
- 🗑️ Red (#F44336): Delete buttons
- ⚫ Gray (#9E9E9E): Neutral/Disabled states
- 🟢 Green labels: Answered questions
- ⚫ Gray labels: Unanswered questions

### **Material Design Elements**
- MaterialButton: Primary actions
- MaterialTextBox: Input fields
- MaterialLabel: Headers & info
- MaterialTabControl: Main navigation
- DataGridView: Result display
- NumericUpDown: Duration input
- ComboBox: Dropdown selections

---

## ✅ Checklist Hoàn Thành

### **Core Exam System**
- ✅ ExamService với đầy đủ CRUD operations
- ✅ FrmExamManagement (giáo viên quản lý)
- ✅ FrmQuestionEditor (quản lý câu hỏi)
- ✅ FrmTakeExam (học sinh làm bài)
- ✅ Tự động tính điểm
- ✅ Lưu kết quả vào database
- ✅ Hiển thị kết quả cho giáo viên

### **Integration**
- ✅ Tab "Đề Thi" trong UcClassDetail
- ✅ MainForm → btnOpenExamManager
- ✅ Role-based access (Teacher only)
- ✅ Pass ExamService through dependency injection

### **Database**
- ✅ Exam entity & table
- ✅ Question entity & table
- ✅ ExamResult entity & table
- ✅ ViolationLog entity (ready for Mục 5)

### **Next Steps (Mục 5)**
- ⏳ Real-time violation detection
- ⏳ Eye-gaze tracking
- ⏳ Violation logging
- ⏳ Teacher dashboard for violations
- ⏳ Offline persistence to SQLite

---

## 📞 Support & Troubleshooting

### **Lỗi: "Chưa có câu hỏi cho đề thi này"**
- Kiểm tra: Bài thi có được tạo trong database không?
- Giải pháp: Thêm câu hỏi qua FrmQuestionEditor

### **Lỗi: "Kết nối database thất bại"**
- Kiểm tra: Connection string trong appsettings.json
- Kiểm tra: SQL Server đang chạy?

### **Điểm không được lưu**
- Kiểm tra: ExamService.UpdateExamResultAsync() được gọi?
- Kiểm tra: User có quyền cập nhật ExamResults table?

---

**Version**: 1.0
**Last Updated**: 2025
**Status**: ✅ Complete (Core Features)
