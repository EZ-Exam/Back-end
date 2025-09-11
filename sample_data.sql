-- =====================================================
-- EZEXAM SAMPLE DATA - CHƯƠNG TRÌNH CẤP 3
-- Dựa trên 3 bộ sách: Kết nối tri thức, Chân trời sáng tạo, Cánh diều
-- Database: PostgreSQL (Subabase)
-- =====================================================

-- 1. ROLES
-- =====================================================
INSERT INTO roles ("Id", "RoleName", "CreatedAt", "UpdatedAt") VALUES
(1, 'Admin', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Teacher', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Student', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'Moderator', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 2. GRADES
-- =====================================================
INSERT INTO grades ("Id", "Name", "CreatedAt", "UpdatedAt") VALUES
(1, 'Lớp 10', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Lớp 11', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Lớp 12', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 3. SEMESTERS
-- =====================================================
INSERT INTO semesters ("Id", "Name", "GradeId", "CreatedAt", "UpdatedAt") VALUES
(1, 'Học kỳ 1', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Học kỳ 2', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Học kỳ 1', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'Học kỳ 2', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(5, 'Học kỳ 1', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(6, 'Học kỳ 2', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 4. SUBJECTS
-- =====================================================
INSERT INTO subjects ("Id", "Name", "Code", "Description", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(1, 'Toán học', 'MATH', 'Môn Toán học cấp 3', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Vật lý', 'PHYS', 'Môn Vật lý cấp 3', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Hóa học', 'CHEM', 'Môn Hóa học cấp 3', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'Sinh học', 'BIO', 'Môn Sinh học cấp 3', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(5, 'Ngữ văn', 'LIT', 'Môn Ngữ văn cấp 3', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(6, 'Tiếng Anh', 'ENG', 'Môn Tiếng Anh cấp 3', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(7, 'Lịch sử', 'HIST', 'Môn Lịch sử cấp 3', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(8, 'Địa lý', 'GEO', 'Môn Địa lý cấp 3', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 5. DIFFICULTY LEVELS
-- =====================================================
INSERT INTO difficulty_levels ("Id", "Name", "Code", "Description", "Level", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(1, 'Dễ', 'EASY', 'Mức độ dễ - phù hợp với học sinh trung bình', 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Trung bình', 'MEDIUM', 'Mức độ trung bình - phù hợp với học sinh khá', 2, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Khó', 'HARD', 'Mức độ khó - phù hợp với học sinh giỏi', 3, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'Rất khó', 'VERY_HARD', 'Mức độ rất khó - phù hợp với học sinh xuất sắc', 4, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 6. TEXTBOOKS (3 bộ sách phổ biến)
-- =====================================================
-- Bộ sách "Kết nối tri thức với cuộc sống"
INSERT INTO textbooks ("Id", "Name", "GradeId", "CreatedAt", "UpdatedAt") VALUES
(1, 'Kết nối tri thức - Toán 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Kết nối tri thức - Vật lý 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Kết nối tri thức - Hóa học 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'Kết nối tri thức - Sinh học 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(5, 'Kết nối tri thức - Ngữ văn 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(6, 'Kết nối tri thức - Tiếng Anh 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(7, 'Kết nối tri thức - Lịch sử 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(8, 'Kết nối tri thức - Địa lý 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

(9, 'Kết nối tri thức - Toán 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(10, 'Kết nối tri thức - Vật lý 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(11, 'Kết nối tri thức - Hóa học 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(12, 'Kết nối tri thức - Sinh học 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(13, 'Kết nối tri thức - Ngữ văn 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(14, 'Kết nối tri thức - Tiếng Anh 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(15, 'Kết nối tri thức - Lịch sử 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(16, 'Kết nối tri thức - Địa lý 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

(17, 'Kết nối tri thức - Toán 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(18, 'Kết nối tri thức - Vật lý 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(19, 'Kết nối tri thức - Hóa học 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(20, 'Kết nối tri thức - Sinh học 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(21, 'Kết nối tri thức - Ngữ văn 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(22, 'Kết nối tri thức - Tiếng Anh 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(23, 'Kết nối tri thức - Lịch sử 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(24, 'Kết nối tri thức - Địa lý 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Bộ sách "Chân trời sáng tạo"
INSERT INTO textbooks ("Id", "Name", "GradeId", "CreatedAt", "UpdatedAt") VALUES
(25, 'Chân trời sáng tạo - Toán 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(26, 'Chân trời sáng tạo - Vật lý 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(27, 'Chân trời sáng tạo - Hóa học 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(28, 'Chân trời sáng tạo - Sinh học 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(29, 'Chân trời sáng tạo - Ngữ văn 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(30, 'Chân trời sáng tạo - Tiếng Anh 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(31, 'Chân trời sáng tạo - Lịch sử 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(32, 'Chân trời sáng tạo - Địa lý 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

(33, 'Chân trời sáng tạo - Toán 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(34, 'Chân trời sáng tạo - Vật lý 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(35, 'Chân trời sáng tạo - Hóa học 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(36, 'Chân trời sáng tạo - Sinh học 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(37, 'Chân trời sáng tạo - Ngữ văn 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(38, 'Chân trời sáng tạo - Tiếng Anh 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(39, 'Chân trời sáng tạo - Lịch sử 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(40, 'Chân trời sáng tạo - Địa lý 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

(41, 'Chân trời sáng tạo - Toán 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(42, 'Chân trời sáng tạo - Vật lý 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(43, 'Chân trời sáng tạo - Hóa học 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(44, 'Chân trời sáng tạo - Sinh học 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(45, 'Chân trời sáng tạo - Ngữ văn 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(46, 'Chân trời sáng tạo - Tiếng Anh 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(47, 'Chân trời sáng tạo - Lịch sử 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(48, 'Chân trời sáng tạo - Địa lý 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Bộ sách "Cánh diều"
INSERT INTO textbooks ("Id", "Name", "GradeId", "CreatedAt", "UpdatedAt") VALUES
(49, 'Cánh diều - Toán 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(50, 'Cánh diều - Vật lý 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(51, 'Cánh diều - Hóa học 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(52, 'Cánh diều - Sinh học 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(53, 'Cánh diều - Ngữ văn 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(54, 'Cánh diều - Tiếng Anh 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(55, 'Cánh diều - Lịch sử 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(56, 'Cánh diều - Địa lý 10', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

(57, 'Cánh diều - Toán 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(58, 'Cánh diều - Vật lý 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(59, 'Cánh diều - Hóa học 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(60, 'Cánh diều - Sinh học 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(61, 'Cánh diều - Ngữ văn 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(62, 'Cánh diều - Tiếng Anh 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(63, 'Cánh diều - Lịch sử 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(64, 'Cánh diều - Địa lý 11', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

(65, 'Cánh diều - Toán 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(66, 'Cánh diều - Vật lý 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(67, 'Cánh diều - Hóa học 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(68, 'Cánh diều - Sinh học 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(69, 'Cánh diều - Ngữ văn 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(70, 'Cánh diều - Tiếng Anh 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(71, 'Cánh diều - Lịch sử 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(72, 'Cánh diều - Địa lý 12', 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 7. CHAPTERS (Chương theo chương trình cấp 3)
-- =====================================================
-- Toán 10 - Học kỳ 1
INSERT INTO chapters ("Id", "Name", "SubjectId", "SemesterId", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(1, 'Mệnh đề và tập hợp', 1, 1, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Bất phương trình và hệ bất phương trình bậc nhất hai ẩn', 1, 1, 2, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Hàm số bậc hai và đồ thị', 1, 1, 3, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'Hệ thức lượng trong tam giác', 1, 1, 4, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Toán 10 - Học kỳ 2
INSERT INTO chapters ("Id", "Name", "SubjectId", "SemesterId", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(5, 'Vectơ', 1, 2, 5, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(6, 'Tích vô hướng của hai vectơ', 1, 2, 6, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(7, 'Phương pháp tọa độ trong mặt phẳng', 1, 2, 7, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(8, 'Thống kê', 1, 2, 8, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Vật lý 10 - Học kỳ 1
INSERT INTO chapters ("Id", "Name", "SubjectId", "SemesterId", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(9, 'Động học chất điểm', 2, 1, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(10, 'Động lực học chất điểm', 2, 1, 2, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(11, 'Cân bằng và chuyển động của vật rắn', 2, 1, 3, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(12, 'Các định luật bảo toàn', 2, 1, 4, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Vật lý 10 - Học kỳ 2
INSERT INTO chapters ("Id", "Name", "SubjectId", "SemesterId", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(13, 'Chất khí', 2, 2, 5, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(14, 'Cơ sở của nhiệt động lực học', 2, 2, 6, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(15, 'Chất rắn và chất lỏng. Sự chuyển thể', 2, 2, 7, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Hóa học 10 - Học kỳ 1
INSERT INTO chapters ("Id", "Name", "SubjectId", "SemesterId", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(16, 'Cấu tạo nguyên tử', 3, 1, 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(17, 'Bảng tuần hoàn các nguyên tố hóa học', 3, 1, 2, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(18, 'Liên kết hóa học', 3, 1, 3, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Hóa học 10 - Học kỳ 2
INSERT INTO chapters ("Id", "Name", "SubjectId", "SemesterId", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(19, 'Phản ứng oxi hóa - khử', 3, 2, 4, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(20, 'Nhóm halogen', 3, 2, 5, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(21, 'Nhóm oxi - lưu huỳnh', 3, 2, 6, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 8. LESSONS (Bài học chi tiết)
-- =====================================================
-- Toán 10 - Chương 1: Mệnh đề và tập hợp
INSERT INTO lessons ("Id", "Name", "ChapterId", "CreatedAt", "UpdatedAt") VALUES
(1, 'Mệnh đề', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Tập hợp', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Các phép toán trên tập hợp', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'Các tập hợp số', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(5, 'Số gần đúng và sai số', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Toán 10 - Chương 2: Bất phương trình
INSERT INTO lessons ("Id", "Name", "ChapterId", "CreatedAt", "UpdatedAt") VALUES
(6, 'Bất phương trình bậc nhất hai ẩn', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(7, 'Hệ bất phương trình bậc nhất hai ẩn', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(8, 'Áp dụng vào bài toán thực tế', 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Vật lý 10 - Chương 1: Động học chất điểm
INSERT INTO lessons ("Id", "Name", "ChapterId", "CreatedAt", "UpdatedAt") VALUES
(9, 'Chuyển động cơ', 9, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(10, 'Chuyển động thẳng đều', 9, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(11, 'Chuyển động thẳng biến đổi đều', 9, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(12, 'Sự rơi tự do', 9, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(13, 'Chuyển động tròn đều', 9, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Hóa học 10 - Chương 1: Cấu tạo nguyên tử
INSERT INTO lessons ("Id", "Name", "ChapterId", "CreatedAt", "UpdatedAt") VALUES
(14, 'Thành phần của nguyên tử', 16, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(15, 'Hạt nhân nguyên tử - Nguyên tố hóa học', 16, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(16, 'Đồng vị', 16, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(17, 'Cấu hình electron nguyên tử', 16, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 9. USERS (Người dùng mẫu)
-- =====================================================
INSERT INTO users ("Id", "Email", "PasswordHash", "FullName", "RoleId", "IsActive", "EmailVerifiedAt", "IsPremium", "LastLoginAt", "CreatedAt", "UpdatedAt", "Balance") VALUES
(1, 'admin@ezexam.com', '$2a$10$N9qo8uLOickgx2ZMRZoMye', 'Quản trị viên hệ thống', 1, true, CURRENT_TIMESTAMP, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 0),
(2, 'teacher1@ezexam.com', '$2a$10$N9qo8uLOickgx2ZMRZoMye', 'Nguyễn Văn An', 2, true, CURRENT_TIMESTAMP, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 0),
(3, 'teacher2@ezexam.com', '$2a$10$N9qo8uLOickgx2ZMRZoMye', 'Trần Thị Bình', 2, true, CURRENT_TIMESTAMP, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 0),
(4, 'student1@ezexam.com', '$2a$10$N9qo8uLOickgx2ZMRZoMye', 'Lê Văn Cường', 3, true, CURRENT_TIMESTAMP, false, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 100000),
(5, 'student2@ezexam.com', '$2a$10$N9qo8uLOickgx2ZMRZoMye', 'Phạm Thị Dung', 3, true, CURRENT_TIMESTAMP, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 50000),
(6, 'student3@ezexam.com', '$2a$10$N9qo8uLOickgx2ZMRZoMye', 'Hoàng Văn Em', 3, true, CURRENT_TIMESTAMP, false, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 0);

-- 10. EXAM TYPES
-- =====================================================
INSERT INTO exam_types ("Id", "Name", "CreatedAt", "UpdatedAt") VALUES
(1, 'Kiểm tra 15 phút', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Kiểm tra 45 phút', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Thi học kỳ', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'Thi thử THPT Quốc gia', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(5, 'Bài tập về nhà', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 11. SUBSCRIPTION TYPES
-- =====================================================
INSERT INTO subscription_types ("Id", "SubscriptionCode", "SubscriptionName", "SubscriptionPrice", "Description", "CreatedAt", "UpdatedAt") VALUES
(1, 'FREE', 'Gói miễn phí', 0, 'Gói cơ bản với các tính năng hạn chế', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'BASIC', 'Gói cơ bản', 99000, 'Gói cơ bản với đầy đủ tính năng học tập', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'PREMIUM', 'Gói cao cấp', 199000, 'Gói cao cấp với tất cả tính năng và hỗ trợ ưu tiên', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'STUDENT', 'Gói học sinh', 149000, 'Gói đặc biệt dành cho học sinh với giá ưu đãi', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 12. SAMPLE QUESTIONS (Câu hỏi mẫu)
-- =====================================================
-- Câu hỏi Toán 10 - Mệnh đề
INSERT INTO questions ("Id", "Content", "QuestionSource", "DifficultyLevelId", "SubjectId", "ChapterId", "LessonId", "TextbookId", "CreatedByUserId", "QuestionType", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(1, 'Trong các câu sau, câu nào là mệnh đề?', 'Sách Kết nối tri thức - Toán 10', 1, 1, 1, 1, 1, 2, 'MULTIPLE_CHOICE', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Mệnh đề "∀x ∈ ℝ, x² ≥ 0" có giá trị chân lý là:', 'Sách Kết nối tri thức - Toán 10', 2, 1, 1, 1, 1, 2, 'MULTIPLE_CHOICE', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Phủ định của mệnh đề "Tồn tại số nguyên x sao cho x² = 2" là:', 'Sách Kết nối tri thức - Toán 10', 3, 1, 1, 1, 1, 2, 'MULTIPLE_CHOICE', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Câu hỏi Vật lý 10 - Chuyển động cơ
INSERT INTO questions ("Id", "Content", "QuestionSource", "DifficultyLevelId", "SubjectId", "ChapterId", "LessonId", "TextbookId", "CreatedByUserId", "QuestionType", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(4, 'Chuyển động cơ là gì?', 'Sách Kết nối tri thức - Vật lý 10', 1, 2, 9, 9, 2, 2, 'MULTIPLE_CHOICE', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(5, 'Một vật chuyển động thẳng đều với vận tốc 20 m/s. Quãng đường vật đi được trong 5 giây là:', 'Sách Kết nối tri thức - Vật lý 10', 2, 2, 9, 10, 2, 2, 'MULTIPLE_CHOICE', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(6, 'Trong chuyển động thẳng biến đổi đều, gia tốc có đặc điểm gì?', 'Sách Kết nối tri thức - Vật lý 10', 2, 2, 9, 11, 2, 2, 'MULTIPLE_CHOICE', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Câu hỏi Hóa học 10 - Cấu tạo nguyên tử
INSERT INTO questions ("Id", "Content", "QuestionSource", "DifficultyLevelId", "SubjectId", "ChapterId", "LessonId", "TextbookId", "CreatedByUserId", "QuestionType", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(7, 'Nguyên tử được cấu tạo bởi những hạt nào?', 'Sách Kết nối tri thức - Hóa học 10', 1, 3, 16, 14, 3, 2, 'MULTIPLE_CHOICE', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(8, 'Số hiệu nguyên tử Z của một nguyên tố cho biết:', 'Sách Kết nối tri thức - Hóa học 10', 2, 3, 16, 15, 3, 2, 'MULTIPLE_CHOICE', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(9, 'Đồng vị là những nguyên tử của cùng một nguyên tố có:', 'Sách Kết nối tri thức - Hóa học 10', 2, 3, 16, 16, 3, 2, 'MULTIPLE_CHOICE', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 13. ANSWERS (Đáp án cho các câu hỏi)
-- =====================================================
-- Đáp án cho câu hỏi 1 (Toán - Mệnh đề)
INSERT INTO answers ("Id", "QuestionId", "AnswerKey", "Content", "IsCorrect", "Explanation", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(1, 1, 'A', 'Hôm nay trời đẹp quá!', false, 'Đây là câu cảm thán, không phải mệnh đề', 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 1, 'B', '2 + 3 = 5', true, 'Đây là mệnh đề vì có thể xác định được tính đúng sai', 2, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 1, 'C', 'Bạn có khỏe không?', false, 'Đây là câu hỏi, không phải mệnh đề', 3, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 1, 'D', 'Hãy làm bài tập!', false, 'Đây là câu mệnh lệnh, không phải mệnh đề', 4, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Đáp án cho câu hỏi 2 (Toán - Mệnh đề)
INSERT INTO answers ("Id", "QuestionId", "AnswerKey", "Content", "IsCorrect", "Explanation", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(5, 2, 'A', 'Đúng', true, 'Với mọi số thực x, ta luôn có x² ≥ 0', 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(6, 2, 'B', 'Sai', false, 'Mệnh đề này luôn đúng với mọi số thực', 2, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(7, 2, 'C', 'Không xác định', false, 'Mệnh đề này có thể xác định được tính đúng sai', 3, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Đáp án cho câu hỏi 4 (Vật lý - Chuyển động cơ)
INSERT INTO answers ("Id", "QuestionId", "AnswerKey", "Content", "IsCorrect", "Explanation", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(8, 4, 'A', 'Sự thay đổi vị trí của vật theo thời gian', true, 'Đây là định nghĩa chính xác của chuyển động cơ', 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(9, 4, 'B', 'Sự thay đổi tốc độ của vật', false, 'Đây chỉ là một khía cạnh của chuyển động', 2, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(10, 4, 'C', 'Sự thay đổi gia tốc của vật', false, 'Đây chỉ là một khía cạnh của chuyển động', 3, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(11, 4, 'D', 'Sự thay đổi khối lượng của vật', false, 'Khối lượng không thay đổi trong chuyển động cơ', 4, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Đáp án cho câu hỏi 5 (Vật lý - Chuyển động thẳng đều)
INSERT INTO answers ("Id", "QuestionId", "AnswerKey", "Content", "IsCorrect", "Explanation", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(12, 5, 'A', '100 m', true, 's = v.t = 20 × 5 = 100 m', 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(13, 5, 'B', '25 m', false, 'Sai công thức tính quãng đường', 2, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(14, 5, 'C', '4 m', false, 'Sai công thức tính quãng đường', 3, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(15, 5, 'D', '20 m', false, 'Đây là vận tốc, không phải quãng đường', 4, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- Đáp án cho câu hỏi 7 (Hóa học - Cấu tạo nguyên tử)
INSERT INTO answers ("Id", "QuestionId", "AnswerKey", "Content", "IsCorrect", "Explanation", "Order", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(16, 7, 'A', 'Proton và electron', false, 'Thiếu neutron', 1, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(17, 7, 'B', 'Proton, neutron và electron', true, 'Nguyên tử được cấu tạo bởi 3 loại hạt cơ bản', 2, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(18, 7, 'C', 'Chỉ có proton', false, 'Thiếu neutron và electron', 3, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(19, 7, 'D', 'Chỉ có electron', false, 'Thiếu proton và neutron', 4, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 14. SAMPLE EXAMS (Đề thi mẫu)
-- =====================================================
INSERT INTO exams ("Id", "Name", "Description", "SubjectId", "LessonId", "ExamTypeId", "CreatedByUserId", "TimeLimit", "TotalQuestions", "TotalMarks", "IsDeleted", "IsActive", "IsPublic", "CreatedAt", "UpdatedAt") VALUES
(1, 'Kiểm tra 15 phút - Mệnh đề và tập hợp', 'Đề kiểm tra 15 phút chương Mệnh đề và tập hợp - Toán 10', 1, 1, 1, 2, 15, 5, 10, false, true, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'Kiểm tra 45 phút - Chuyển động cơ', 'Đề kiểm tra 45 phút chương Động học chất điểm - Vật lý 10', 2, 9, 2, 2, 45, 20, 100, false, true, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'Thi học kỳ 1 - Hóa học 10', 'Đề thi học kỳ 1 môn Hóa học lớp 10', 3, 14, 3, 2, 90, 40, 100, false, true, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 15. EXAM QUESTIONS (Câu hỏi trong đề thi)
-- =====================================================
INSERT INTO exam_questions ("Id", "ExamId", "QuestionId", "Order", "CreatedAt") VALUES
(1, 1, 1, 1, CURRENT_TIMESTAMP),
(2, 1, 2, 2, CURRENT_TIMESTAMP),
(3, 1, 3, 3, CURRENT_TIMESTAMP),
(4, 2, 4, 1, CURRENT_TIMESTAMP),
(5, 2, 5, 2, CURRENT_TIMESTAMP),
(6, 2, 6, 3, CURRENT_TIMESTAMP),
(7, 3, 7, 1, CURRENT_TIMESTAMP),
(8, 3, 8, 2, CURRENT_TIMESTAMP),
(9, 3, 9, 3, CURRENT_TIMESTAMP);

-- 16. USER SUBSCRIPTIONS (Đăng ký gói của người dùng)
-- =====================================================
INSERT INTO user_subscriptions ("Id", "UserId", "SubscriptionTypeId", "Amount", "PaymentStatus", "StartDate", "EndDate", "IsActive", "CreatedAt", "UpdatedAt") VALUES
(1, 4, 1, 0, 'Completed', CURRENT_TIMESTAMP, NULL, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 5, 3, 199000, 'Completed', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + INTERVAL '1 year', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 6, 1, 0, 'Completed', CURRENT_TIMESTAMP, NULL, true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 17. SAMPLE SOLUTIONS (Lời giải mẫu)
-- =====================================================
INSERT INTO solutions ("Id", "QuestionId", "CreatedByUserId", "Content", "Explanation", "IsApproved", "IsDeleted", "CreatedAt", "UpdatedAt") VALUES
(1, 1, 2, 'Câu B là đáp án đúng', 'Mệnh đề là một khẳng định có thể xác định được tính đúng sai. Câu "2 + 3 = 5" là một khẳng định đúng, do đó là mệnh đề.', true, false, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 2, 2, 'Mệnh đề có giá trị chân lý là ĐÚNG', 'Với mọi số thực x, ta luôn có x² ≥ 0. Điều này luôn đúng vì bình phương của một số thực luôn không âm.', true, false, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 5, 2, 'Quãng đường = 100 m', 'Áp dụng công thức: s = v × t = 20 × 5 = 100 m', true, false, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- 18. SAMPLE CHATBOT CONVERSATIONS
-- =====================================================
INSERT INTO chatbots ("Id", "UserId", "UserMessage", "BotResponse", "MessageType", "Context", "QuestionId", "SubjectId", "IsHelpful", "CreatedAt") VALUES
(1, 4, 'Mệnh đề là gì?', 'Mệnh đề là một khẳng định có thể xác định được tính đúng sai. Ví dụ: "2 + 3 = 5" là mệnh đề đúng, "Hôm nay trời đẹp" không phải mệnh đề vì không thể xác định tính đúng sai một cách khách quan.', 'TEXT', 'MATH', 1, 1, true, CURRENT_TIMESTAMP),
(2, 5, 'Công thức tính quãng đường trong chuyển động thẳng đều?', 'Trong chuyển động thẳng đều, quãng đường được tính bằng công thức: s = v × t, trong đó v là vận tốc (m/s) và t là thời gian (s).', 'TEXT', 'PHYSICS', 5, 2, true, CURRENT_TIMESTAMP),
(3, 6, 'Nguyên tử được cấu tạo bởi những hạt nào?', 'Nguyên tử được cấu tạo bởi 3 loại hạt cơ bản: proton (mang điện tích dương), neutron (không mang điện) và electron (mang điện tích âm).', 'TEXT', 'CHEMISTRY', 7, 3, true, CURRENT_TIMESTAMP);

-- =====================================================
-- KẾT THÚC DATA MẪU
-- =====================================================
-- Tổng cộng đã tạo:
-- - 4 roles (Admin, Teacher, Student, Moderator)
-- - 3 grades (Lớp 10, 11, 12)
-- - 6 semesters (2 học kỳ cho mỗi lớp)
-- - 8 subjects (các môn học cấp 3)
-- - 4 difficulty levels (Dễ, Trung bình, Khó, Rất khó)
-- - 72 textbooks (3 bộ sách × 8 môn × 3 lớp)
-- - 21 chapters (các chương theo chương trình)
-- - 17 lessons (các bài học chi tiết)
-- - 6 users (1 admin, 2 teacher, 3 student)
-- - 5 exam types (các loại kiểm tra)
-- - 4 subscription types (các gói đăng ký)
-- - 9 questions (câu hỏi mẫu)
-- - 19 answers (đáp án cho câu hỏi)
-- - 3 exams (đề thi mẫu)
-- - 9 exam_questions (câu hỏi trong đề thi)
-- - 3 user_subscriptions (đăng ký gói)
-- - 3 solutions (lời giải mẫu)
-- - 3 chatbot conversations (hội thoại mẫu)
-- =====================================================
