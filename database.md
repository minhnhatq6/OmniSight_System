# Cấu trúc Database Project OmniSight_System

Tài liệu này liệt kê chi tiết các bảng và thuộc tính trong cơ sở dữ liệu của dự án.

## 1. Bảng Users (Người dùng)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | UserId | Integer | PK, Not Null | Khóa chính (Auto Increment) |
| 2 | Id | Guid | Not Null | Mã định danh duy nhất (Guid) |
| 3 | Username | String(50) | Required, Not Null | Tên đăng nhập |
| 4 | PasswordHash | String(255) | Required, Not Null | Mật khẩu đã mã hóa |
| 5 | FullName | String(100) | | Họ và tên đầy đủ |
| 6 | Email | String(100) | | Địa chỉ Email |
| 7 | AvatarUrl | String(255) | | Đường dẫn ảnh đại diện |
| 8 | IsStudent | Boolean | Default: True | Vai trò học sinh |
| 9 | IsTeacher | Boolean | Default: False | Vai trò giáo viên |
| 10 | Phone | String(15) | | Số điện thoại |
| 11 | FaceVector | String | | Vector khuôn mặt (dạng chuỗi) |
| 12 | FaceImageUrl | String(255) | | Đường dẫn ảnh khuôn mặt |
| 13 | GoogleId | String(100) | | ID đăng nhập Google |
| 14 | IsEmailConfirmed | Boolean | Default: False | Trạng thái xác thực Email |
| 15 | FaceEmbedding | String | | Vector nhúng khuôn mặt cho AI |

## 2. Bảng Subjects (Môn học)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | SubjectId | Integer | PK, Not Null | Khóa chính |
| 2 | SubjectName | String(100) | Required, Not Null | Tên môn học |

## 3. Bảng Classes (Lớp học)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | ClassId | Integer | PK, Not Null | Khóa chính |
| 2 | ClassName | String(100) | Required, Not Null | Tên lớp học |
| 3 | JoinCode | String(10) | | Mã tham gia lớp học |
| 4 | TeacherId | Integer | FK (Users), Not Null | ID giáo viên sở hữu |
| 5 | SubjectId | Integer | FK (Subjects), Not Null | ID môn học |

## 4. Bảng ClassMembers (Thành viên lớp)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | ClassId | Integer | PK, FK (Classes) | Mã lớp học |
| 2 | StudentId | Integer | PK, FK (Users) | Mã sinh viên |
| 3 | JoinedAt | DateTime | Not Null | Thời gian tham gia |

## 5. Bảng AuthTokens (Mã xác thực)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | Id | Integer | PK, Not Null | Khóa chính |
| 2 | UserId | Integer | FK (Users), Not Null | ID người dùng |
| 3 | Token | String | Required, Not Null | Chuỗi Token |
| 4 | Type | Enum | Required, Not Null | Loại Token (Email/Password) |
| 5 | ExpiryDate | DateTime | Not Null | Ngày hết hạn |
| 6 | IsUsed | Boolean | Default: False | Trạng thái đã sử dụng |

## 6. Bảng Streams (Bảng tin/Bài đăng)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | PostId | Integer | PK, Not Null | Khóa chính |
| 2 | ClassId | Integer | FK (Classes), Not Null | ID lớp học |
| 3 | AuthorId | Integer | FK (Users), Not Null | ID người đăng |
| 4 | Content | String | | Nội dung bài đăng |
| 5 | CreatedAt | DateTime | Not Null | Thời gian đăng |

## 7. Bảng Assignments (Bài tập)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | AssignmentId | Integer | PK, Not Null | Khóa chính |
| 2 | ClassId | Integer | FK (Classes), Not Null | ID lớp học |
| 3 | Title | String(200) | Required, Not Null | Tiêu đề bài tập |
| 4 | Description | String | | Mô tả bài tập |
| 5 | AttachmentUrl | String(255) | | Đường dẫn file đính kèm |
| 6 | DueDate | DateTime | Nullable | Hạn nộp bài |
| 7 | CreatedBy | Integer | FK (Users), Not Null | ID người tạo |

## 8. Bảng Submissions (Bài nộp)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | SubmissionId | Integer | PK, Not Null | Khóa chính |
| 2 | AssignmentId | Integer | FK (Assignments), Not Null | ID bài tập |
| 3 | StudentId | Integer | FK (Users), Not Null | ID sinh viên nộp |
| 4 | FileOrLinkUrl | String(255) | | Đường dẫn file hoặc link |
| 5 | SubmittedAt | DateTime | Nullable | Thời gian nộp |
| 6 | Score | Float | Nullable | Điểm số |
| 7 | Feedback | String | | Phản hồi từ giáo viên |

## 9. Bảng Exams (Kỳ thi)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | ExamId | Integer | PK, Not Null | Khóa chính |
| 2 | ClassId | Integer | FK (Classes), Not Null | ID lớp học |
| 3 | Title | String(200) | Required, Not Null | Tiêu đề kỳ thi |
| 4 | DurationMinutes | Integer | Not Null | Thời gian làm bài (phút) |
| 5 | CreatedAt | DateTime | Not Null | Thời gian tạo |

## 10. Bảng Questions (Câu hỏi)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | QuestionId | Integer | PK, Not Null | Khóa chính |
| 2 | ExamId | Integer | FK (Exams), Not Null | ID kỳ thi |
| 3 | Content | String | | Nội dung câu hỏi |
| 4 | OptionA | String | | Lựa chọn A |
| 5 | OptionB | String | | Lựa chọn B |
| 6 | OptionC | String | | Lựa chọn C |
| 7 | OptionD | String | | Lựa chọn D |
| 8 | CorrectOption | String(1) | | Đáp án đúng (A/B/C/D) |

## 11. Bảng ExamResults (Kết quả thi)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | ResultId | Integer | PK, Not Null | Khóa chính |
| 2 | ExamId | Integer | FK (Exams), Not Null | ID kỳ thi |
| 3 | StudentId | Integer | FK (Users), Not Null | ID sinh viên |
| 4 | Score | Float | Nullable | Điểm số đạt được |
| 5 | StartedAt | DateTime | Nullable | Thời gian bắt đầu |
| 6 | CompletedAt | DateTime | Nullable | Thời gian nộp bài |

## 12. Bảng ViolationLogs (Bản ghi vi phạm)
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | LogId | Integer | PK, Not Null | Khóa chính |
| 2 | ResultId | Integer | FK (ExamResults), Not Null | ID kết quả thi |
| 3 | ViolationType | String(50) | | Loại vi phạm (ví dụ: Rời tab, Quay mặt) |
| 4 | ImageUrl | String(255) | | Ảnh bằng chứng vi phạm |
| 5 | Timestamp | DateTime | Not Null | Thời gian ghi nhận |
| 6 | Status | String(50) | | Trạng thái xử lý |
