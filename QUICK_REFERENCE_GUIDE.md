# 🎯 OmniSight Exam System - Quick Reference Guide

## 🚀 Quick Start (5 Minutes)

### **For Teachers - Create Your First Exam**

1. **Open Exam Management**
   ```
   Click: Settings Tab 
   → Look for "Quản Lý Đề Thi" / "Manage Exams" button
   → Opens FrmExamManagement
   ```

2. **Create Exam** (Tab 1)
   ```
   Click: ➕ Tạo Bài Thi
   Enter: 
     • Name: "Kiểm Tra Toán 10"
     • Class: Select from dropdown
     • Time: 60 minutes
   Click: 💾 Lưu Bài Thi
   ```

3. **Add Questions** (Tab 2)
   ```
   Click: ➕ Thêm Câu
   Enter:
     • Question: "What is 2+2?"
     • Option A: "2"
     • Option B: "4"  ← Correct
     • Option C: "6"
     • Option D: "8"
     • Correct Answer: "B"
   Click: 💾 Lưu
   ```

4. **Publish Exam**
   ```
   Click: 💾 Lưu Bài Thi
   Done! Students can now see it.
   ```

5. **View Results** (Tab 3)
   ```
   Select exam from ComboBox
   See all student scores, times, etc.
   ```

### **For Students - Take Your First Exam**

1. **Find Exam**
   ```
   Select your class
   → Click Tab: "Đề Thi"
   → See list of exams
   ```

2. **Start Exam**
   ```
   Click: 🚀 Làm Thi
   → FrmTakeExam opens with timer
   ```

3. **Answer Questions**
   ```
   Read question
   Select one option: ⊙ A / ⊙ B / ⊙ C / ⊙ D
   Click: ➡️ Next (or click question number)
   ```

4. **Submit**
   ```
   When done:
   Click: 💾 Submit
   Confirm: "Yes"
   See result with score!
   ```

---

## 🎮 UI Controls Map

### **FrmExamManagement - Top Buttons**

| Button | Function | Hotkey |
|--------|----------|--------|
| ➕ Tạo Bài Thi | Create new exam | Ctrl+N |
| ✏️ Sửa | Edit selected exam | Ctrl+E |
| 🗑️ Xóa | Delete selected exam | Del |
| 🔄 Tải Lại | Refresh list | F5 |

### **Question Management Buttons**

| Button | Function |
|--------|----------|
| ➕ Thêm Câu | Add new question |
| ✏️ Sửa Câu | Edit question |
| 🗑️ Xóa Câu | Delete question |

### **FrmTakeExam - Navigation**

| Button | Function |
|--------|----------|
| ⬅️ Previous | Go to previous question |
| ➡️ Next | Go to next question |
| [1][2][3]... | Jump to specific question |
| 💾 Submit | Submit exam & calculate score |

---

## 📊 Field Reference

### **Exam Form Fields**

```
Tên Bài Thi (Title)
├─ Required: Yes
├─ Type: Text
├─ Max Length: 200 characters
└─ Example: "Kiểm Tra Giữa Kì - Toán 10"

Lớp Học (Class)
├─ Required: Yes
├─ Type: Dropdown (ComboBox)
├─ Source: Your classes (teachers only)
└─ Example: "Toán 10A"

Thời Gian (Duration)
├─ Required: Yes
├─ Type: Number
├─ Min: 5 minutes
├─ Max: 180 minutes
├─ Default: 60 minutes
└─ Example: 60
```

### **Question Form Fields**

```
Nội Dung Câu Hỏi (Content)
├─ Required: Yes
├─ Type: Multiline Text
└─ Example: "Phương trình nào có tập nghiệm là ℝ?"

Đáp Án A (Option A)
├─ Required: Yes
├─ Type: Text
└─ Example: "x² - 4 = 0"

Đáp Án B (Option B)
├─ Required: Yes
├─ Type: Text
└─ Example: "x² + 1 = 0"

Đáp Án C (Option C)
├─ Required: Yes
├─ Type: Text
└─ Example: "x² + 0 = 0"

Đáp Án D (Option D)
├─ Required: Yes
├─ Type: Text
└─ Example: "x² = 0"

Đáp Án Đúng (Correct Answer)
├─ Required: Yes
├─ Type: Dropdown
├─ Options: A, B, C, D
└─ Example: C
```

---

## 💾 Data Storage

### **What Gets Saved Where?**

#### **When Creating Exam**
```
Exams Table:
├─ ExamId: Auto-generated ID
├─ ClassId: From dropdown selection
├─ Title: Your typed name
├─ DurationMinutes: Your entered time
└─ CreatedAt: Current timestamp
```

#### **When Adding Questions**
```
Questions Table:
├─ QuestionId: Auto-generated ID
├─ ExamId: Links to the exam
├─ Content: Your question text
├─ OptionA/B/C/D: Your options
└─ CorrectOption: Your selected answer (A/B/C/D)
```

#### **When Student Takes Exam**
```
ExamResults Table - DURING exam:
├─ ResultId: Auto-generated
├─ ExamId: From exam selection
├─ StudentId: Current user ID
├─ Score: NULL (not yet)
├─ StartedAt: Current timestamp
└─ CompletedAt: NULL (not yet)

ExamResults Table - AFTER submission:
├─ Score: Calculated (0-10)
└─ CompletedAt: Current timestamp
```

---

## 🔢 Score Calculation

### **Formula**
```
Score = (Correct Answers / Total Questions) × 10

Examples:
  10/10 correct → 10.0 score
   8/10 correct → 8.0 score
   5/10 correct → 5.0 score
   0/10 correct → 0.0 score
```

### **Display Format**
```
Score displays with 1 decimal place:
  10.0
  8.5
  7.3
  5.1
  0.0
```

---

## ⏱️ Timer Mechanics

### **FrmTakeExam Timer**

```
Format: MM:SS (Minutes:Seconds)

Example Timeline:
  00:00 → Exam started, timer shows: 60:00
  10:30 → Student answered Q1, timer shows: 49:30
  30:00 → Student answered Q5, timer shows: 30:00
  59:45 → Almost done, timer shows: 00:15
  60:00 → Time's up! Auto-submit (future feature)
```

### **What Affects Timer?**
```
✅ Counting down (tick every 1 second)
✅ Timer starts when FrmTakeExam opens
✅ Timer stops when student submits
✅ Navigating questions does NOT pause timer

⏳ Future (Part 5):
   - Auto-submit when time = 0
   - Visual warning at 5 minutes
   - Display as red when < 5 minutes
```

---

## 📋 Common Workflows

### **Workflow 1: Create & Publish Exam**
```
1. Click: Settings → Quản Lý Đề Thi
2. Tab 1: Click ➕ Tạo Bài Thi
3. Enter exam info → Click 💾 Lưu
4. Tab 2: Click ➕ Thêm Câu
5. Enter Q1 → Click 💾 Lưu
6. Repeat step 4-5 for more questions
7. Tab 1: Verify exam appears in list
✅ DONE - Exam visible to students
```

### **Workflow 2: Student Takes Exam**
```
1. Select class
2. Click Tab: Đề Thi
3. See list of exams
4. Click exam name → 🚀 Làm Thi
5. Read question
6. Select answer: ⊙ A/B/C/D
7. Click ➡️ Next or click question number
8. Repeat 5-7 for all questions
9. Click 💾 Submit
10. Confirm: Yes
11. See score: "Điểm: X.X/10"
✅ DONE - Score saved to database
```

### **Workflow 3: Teacher Reviews Results**
```
1. Click: Settings → Quản Lý Đề Thi
2. Tab 3: Kết Quả Học Sinh
3. Select exam from ComboBox
4. See DataGridView with:
   - Student names
   - Scores
   - Start/End times
   - Duration
5. Analyze results
✅ DONE - Can export/report (future)
```

---

## 🔐 Access Control

### **Who Can Do What?**

| Feature | Teacher | Student |
|---------|---------|---------|
| Create Exam | ✅ | ❌ |
| Edit Exam | ✅ | ❌ |
| Delete Exam | ✅ | ❌ |
| Add Questions | ✅ | ❌ |
| Edit Questions | ✅ | ❌ |
| Delete Questions | ✅ | ❌ |
| View Exam Results | ✅ | 🔽* |
| Take Exam | ❌ | ✅ |
| See Own Score | ❌ | ✅ |
| See Other Scores | ❌ | ❌ |

*🔽 = Students see their own score only (future feature)

---

## 🐛 Troubleshooting

### **Problem: "Chưa có câu hỏi cho đề thi này"**

**Cause**: Exam created but no questions added

**Solution**:
```
1. Open FrmExamManagement
2. Tab 2: Click on exam
3. Click ➕ Thêm Câu
4. Add at least 1 question
5. Try again
```

### **Problem: Exam doesn't appear in student view**

**Cause**: Exam not saved properly

**Solution**:
```
1. Check Tab 1: Exam shows in list?
   - No → Tab 2: Click 💾 Lưu Bài Thi
   - Yes → Continue
2. Check student is in the class
   - Ask: Did you join the class first?
3. Refresh: Tab 1: Click 🔄 Tải Lại
```

### **Problem: Timer showing wrong time**

**Cause**: Exam duration not set correctly

**Solution**:
```
1. Edit exam: Tab 2
2. Check "Thời Gian (phút)" field
3. Should be 5-180 minutes
4. Save: Click 💾 Lưu Bài Thi
5. Restart exam
```

### **Problem: Score not saved**

**Cause**: Database connection issue

**Solution**:
```
1. Check: SQL Server running?
2. Check: Connection string correct?
3. Try: Restart application
4. Check: ExamResults table exists?
5. Contact: Developer if persists
```

---

## 📱 Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| Ctrl+N | Create new exam (when FrmExamManagement open) |
| Ctrl+E | Edit selected exam |
| Ctrl+S | Save (in form) |
| Ctrl+Q | Quit/Close form |
| Tab | Next field (in forms) |
| Shift+Tab | Previous field (in forms) |
| Enter | Submit (in dialogs) |
| Esc | Cancel/Close |
| F5 | Refresh list |
| ← → | Navigate questions (in FrmTakeExam) |
| Space | Select radio button |

---

## 📊 Result Statistics

### **Understanding Result Display**

```
Teacher Result View:
┌──────────────────────────────────┐
│ Student Name │ Score │ Time Info │
├──────────────────────────────────┤
│ Nguyễn Văn A │  8.5  │ 2:15 mins │
│ Trần Thị B   │  10.0 │ 1:45 mins │
│ Hoàng Văn C  │  5.3  │ 3:30 mins │
│ Phạm Thị D   │  7.0  │ 2:40 mins │
└──────────────────────────────────┘

Score Distribution:
  10.0 - Excellent    ██  (2 students)
   7-9  - Good        ███ (3 students)
   5-6  - Average    ██  (2 students)
  0-4   - Poor        █   (1 student)
```

---

## 🎓 Best Practices

### **For Teachers**

1. **Question Quality**
   - Use clear, unambiguous language
   - Ensure one clear correct answer
   - Avoid trick questions
   - Mix difficulty levels

2. **Time Allocation**
   - 2-3 minutes per question
   - 60 min exam = 20-30 questions
   - Add buffer time
   - Consider cheating detection time

3. **Question Order**
   - Start with easier questions
   - Gradually increase difficulty
   - Mix question types
   - End with hardest questions

4. **Results Analysis**
   - Look for patterns in wrong answers
   - Identify weak areas for students
   - Adjust teaching accordingly
   - Provide feedback

### **For Students**

1. **Before Exam**
   - Read all questions first (1-2 min)
   - Plan time allocation
   - Identify easy vs hard questions

2. **During Exam**
   - Answer easier questions first
   - Don't spend too long on one question
   - Use the question list (🟢/⚫ status)
   - Skip hard ones, come back later

3. **Time Management**
   - With 10 minutes left: finish up
   - With 5 minutes left: review answers
   - With 1 minute left: submit

4. **After Exam**
   - Note score immediately
   - Review wrong answers (if provided)
   - Ask teacher for clarification

---

## 🚨 Important Notes

### **Data Integrity**
- ⚠️ Deleting exam deletes ALL results
- ⚠️ Deleting question removes from exam
- ⚠️ No undo for deletions
- ✅ Always backup before major changes

### **Performance**
- ✅ Supports 100+ exams per teacher
- ✅ Supports 1000+ questions per exam
- ✅ Real-time score calculation
- ⏳ Large result sets may be slow (future optimization)

### **Security (Part 5)**
- 🔒 Anti-cheating monitoring coming
- 🔒 Face detection enabled
- 🔒 Eye-gaze tracking enabled
- 🔒 Violation logging enabled

---

## 📞 Support Contact

**Issue**: Form won't open
**Check**: User is teacher? → Settings button visible?

**Issue**: Can't add questions
**Check**: Exam saved first? → Tab 2 showing exam?

**Issue**: Scores seem wrong
**Check**: All questions have correct answers? → Calculation formula

**Issue**: Database error
**Check**: SQL Server running? → Connection string?

---

## ✅ Verification Checklist

After setup, verify:

- [ ] Can create exam (Tab 1)
- [ ] Can add questions (Tab 2)
- [ ] Students see exam in their class
- [ ] Student can click "🚀 Làm Thi"
- [ ] Timer counts down correctly
- [ ] Can answer all questions
- [ ] Can submit exam
- [ ] Score displays after submit
- [ ] Teacher sees result in Tab 3
- [ ] Score matches calculation

---

**Quick Reference v1.0**
**For: OmniSight Exam System**
**Last Updated: 2025**
