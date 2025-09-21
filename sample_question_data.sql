-- SAMPLE QUESTION DATA (FIXED)
-- This script inserts sample questions into the questions table

-- ==============================================
-- 1. INSERT SAMPLE QUESTIONS
-- ==============================================

-- Sample questions for different subjects and difficulty levels
INSERT INTO questions (
    "Content", 
    "QuestionSource", 
    "DifficultyLevelId", 
    "SubjectId", 
    "ChapterId", 
    "LessonId", 
    "TextbookId", 
    "CreatedByUserId", 
    "QuestionType", 
    "Image", 
    "TemplateQuestionId", 
    "IsCloned", 
    "ViewCount", 
    "AverageRating", 
    "IsActive", 
    "CreatedAt", 
    "UpdatedAt"
) VALUES 
-- Mathematics Questions
(
    'Tính giá trị của biểu thức: 2x + 3y khi x = 5 và y = 7',
    'Sách giáo khoa Toán 8',
    1, -- Easy
    1, -- Mathematics
    1, -- Chapter 1
    1, -- Lesson 1
    1, -- Textbook 1
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),
(
    'Giải phương trình: 3x - 7 = 2x + 5',
    'Sách bài tập Toán 8',
    2, -- Medium
    1, -- Mathematics
    1, -- Chapter 1
    1, -- Lesson 1
    1, -- Textbook 1
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),
(
    'Tìm giá trị của y trong hệ phương trình: 2x + y = 7 và x - y = 2',
    'Đề thi học kỳ Toán 8',
    3, -- Hard
    1, -- Mathematics
    1, -- Chapter 1
    1, -- Lesson 1
    1, -- Textbook 1
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),

-- Physics Questions
(
    'Một vật có khối lượng 2kg được nâng lên độ cao 5m. Tính công thực hiện (g = 10 m/s²)',
    'Sách giáo khoa Vật lý 8',
    1, -- Easy
    2, -- Physics
    2, -- Chapter 2
    2, -- Lesson 2
    2, -- Textbook 2
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),
(
    'Một con lắc đơn có chiều dài 1m dao động với chu kỳ T. Tính T (g = 9.8 m/s²)',
    'Sách bài tập Vật lý 8',
    2, -- Medium
    2, -- Physics
    2, -- Chapter 2
    2, -- Lesson 2
    2, -- Textbook 2
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),
(
    'Một electron chuyển động trong từ trường đều với vận tốc 10⁶ m/s. Tính bán kính quỹ đạo (B = 0.1 T, m = 9.1×10⁻³¹ kg, q = 1.6×10⁻¹⁹ C)',
    'Đề thi Olympic Vật lý',
    3, -- Hard
    2, -- Physics
    2, -- Chapter 2
    2, -- Lesson 2
    2, -- Textbook 2
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),

-- Chemistry Questions
(
    'Viết phương trình phản ứng giữa axit clohidric và natri hidroxit',
    'Sách giáo khoa Hóa học 8',
    1, -- Easy
    3, -- Chemistry
    3, -- Chapter 3
    3, -- Lesson 3
    3, -- Textbook 3
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),
(
    'Tính pH của dung dịch HCl 0.1M',
    'Sách bài tập Hóa học 8',
    2, -- Medium
    3, -- Chemistry
    3, -- Chapter 3
    3, -- Lesson 3
    3, -- Textbook 3
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),
(
    'Viết cấu hình electron của nguyên tử sắt (Fe, Z = 26)',
    'Đề thi học sinh giỏi Hóa học',
    3, -- Hard
    3, -- Chemistry
    3, -- Chapter 3
    3, -- Lesson 3
    3, -- Textbook 3
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),

-- Biology Questions
(
    'Bộ phận nào của cây thực hiện quá trình quang hợp?',
    'Sách giáo khoa Sinh học 8',
    1, -- Easy
    4, -- Biology
    4, -- Chapter 4
    4, -- Lesson 4
    4, -- Textbook 4
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),
(
    'Quá trình giảm phân có đặc điểm gì?',
    'Sách bài tập Sinh học 8',
    2, -- Medium
    4, -- Biology
    4, -- Chapter 4
    4, -- Lesson 4
    4, -- Textbook 4
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),
(
    'Trình bày quá trình tổng hợp protein từ gen',
    'Đề thi học sinh giỏi Sinh học',
    3, -- Hard
    4, -- Biology
    4, -- Chapter 4
    4, -- Lesson 4
    4, -- Textbook 4
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),

-- English Questions
(
    'Choose the correct form: "I _____ to school every day."',
    'English Grammar Book',
    1, -- Easy
    5, -- English
    5, -- Chapter 5
    5, -- Lesson 5
    5, -- Textbook 5
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),
(
    'Choose the correct form: "If I _____ you, I would study harder."',
    'English Practice Book',
    2, -- Medium
    5, -- English
    5, -- Chapter 5
    5, -- Lesson 5
    5, -- Textbook 5
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
),
(
    'What literary devices are used in this poem?',
    'English Literature Book',
    3, -- Hard
    5, -- English
    5, -- Chapter 5
    5, -- Lesson 5
    5, -- Textbook 5
    1, -- User 1
    'MultipleChoice',
    NULL,
    NULL,
    false,
    0,
    0.0,
    true,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
);

-- ==============================================
-- 2. INSERT SAMPLE ANSWERS
-- ==============================================

-- Sample answers for the questions
INSERT INTO answers (
    "QuestionId",
    "AnswerKey",
    "Content",
    "IsCorrect",
    "Order",
    "CreatedAt",
    "UpdatedAt"
) VALUES 
-- Answers for Question 1 (Mathematics - Easy)
(1, 'A', '31', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(1, 'B', '25', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(1, 'C', '35', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(1, 'D', '41', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 2 (Mathematics - Medium)
(2, 'A', 'x = 12', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'B', 'x = 8', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'C', 'x = 15', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(2, 'D', 'x = 10', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 3 (Mathematics - Hard)
(3, 'A', 'y = 1', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'B', 'y = 3', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'C', 'y = 5', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(3, 'D', 'y = 7', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 4 (Physics - Easy)
(4, 'A', '100 J', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'B', '50 J', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'C', '200 J', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(4, 'D', '150 J', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 5 (Physics - Medium)
(5, 'A', '2π√(l/g) ≈ 2.0 s', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(5, 'B', '2π√(g/l) ≈ 6.3 s', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(5, 'C', '2π√(l) ≈ 6.3 s', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(5, 'D', '2π√(g) ≈ 6.3 s', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 6 (Physics - Hard)
(6, 'A', 'r = mv/(qB) ≈ 5.7×10⁻⁶ m', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(6, 'B', 'r = qB/(mv) ≈ 1.8×10⁵ m', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(6, 'C', 'r = mv²/(qB) ≈ 9.1×10⁻¹² m', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(6, 'D', 'r = qBv/(m) ≈ 1.8×10¹² m', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 7 (Chemistry - Easy)
(7, 'A', 'HCl + NaOH → NaCl + H₂O', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(7, 'B', 'HCl + NaOH → NaCl + H₂', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(7, 'C', 'HCl + NaOH → NaCl + O₂', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(7, 'D', 'HCl + NaOH → NaCl + H₂O₂', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 8 (Chemistry - Medium)
(8, 'A', 'pH = 1', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(8, 'B', 'pH = 7', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(8, 'C', 'pH = 13', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(8, 'D', 'pH = 0.1', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 9 (Chemistry - Hard)
(9, 'A', '1s² 2s² 2p⁶ 3s² 3p⁶ 4s² 3d⁶', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(9, 'B', '1s² 2s² 2p⁶ 3s² 3p⁶ 4s² 3d⁸', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(9, 'C', '1s² 2s² 2p⁶ 3s² 3p⁶ 4s² 3d⁴', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(9, 'D', '1s² 2s² 2p⁶ 3s² 3p⁶ 4s² 3d⁷', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 10 (Biology - Easy)
(10, 'A', 'Lá cây', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(10, 'B', 'Rễ cây', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(10, 'C', 'Thân cây', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(10, 'D', 'Hoa cây', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 11 (Biology - Medium)
(11, 'A', 'Giảm phân tạo ra 4 tế bào con với số lượng NST giảm một nửa', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(11, 'B', 'Giảm phân tạo ra 2 tế bào con với số lượng NST giảm một nửa', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(11, 'C', 'Giảm phân tạo ra 4 tế bào con với số lượng NST không đổi', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(11, 'D', 'Giảm phân tạo ra 2 tế bào con với số lượng NST tăng gấp đôi', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 12 (Biology - Hard)
(12, 'A', 'Gen mã hóa thông tin → mRNA → Protein', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(12, 'B', 'Protein mã hóa thông tin → mRNA → Gen', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(12, 'C', 'mRNA mã hóa thông tin → Gen → Protein', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(12, 'D', 'Gen mã hóa thông tin → Protein → mRNA', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 13 (English - Easy)
(13, 'A', 'go', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(13, 'B', 'goes', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(13, 'C', 'going', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(13, 'D', 'went', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 14 (English - Medium)
(14, 'A', 'were', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(14, 'B', 'am', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(14, 'C', 'was', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(14, 'D', 'be', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),

-- Answers for Question 15 (English - Hard)
(15, 'A', 'Metaphor, symbolism, and imagery', true, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(15, 'B', 'Only metaphor', false, 2, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(15, 'C', 'Only symbolism', false, 3, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
(15, 'D', 'Only imagery', false, 4, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- ==============================================
-- 3. VERIFY DATA
-- ==============================================

SELECT 'Sample questions inserted successfully!' as status;
SELECT COUNT(*) as total_questions FROM questions;
SELECT COUNT(*) as total_answers FROM answers;

-- Show sample questions by subject
SELECT 
    s."Name" as subject_name,
    dl."Name" as difficulty_level,
    COUNT(*) as question_count
FROM questions q
JOIN subjects s ON q."SubjectId" = s."Id"
JOIN difficulty_levels dl ON q."DifficultyLevelId" = dl."Id"
GROUP BY s."Name", dl."Name"
ORDER BY s."Name", dl."Name";

-- Show sample answers for first question
SELECT 
    q."Content" as question,
    a."AnswerKey",
    a."Content" as answer,
    a."IsCorrect"
FROM questions q
JOIN answers a ON q."Id" = a."QuestionId"
WHERE q."Id" = 1
ORDER BY a."Order";
