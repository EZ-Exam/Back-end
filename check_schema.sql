-- Script để kiểm tra schema hiện tại của database
-- Chạy script này trong Supabase SQL Editor để xem cấu trúc hiện tại

-- Kiểm tra cấu trúc bảng chapters
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns 
WHERE table_name = 'chapters' 
ORDER BY ordinal_position;

-- Kiểm tra cấu trúc bảng subjects
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns 
WHERE table_name = 'subjects' 
ORDER BY ordinal_position;

-- Kiểm tra cấu trúc bảng difficulty_levels
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns 
WHERE table_name = 'difficulty_levels' 
ORDER BY ordinal_position;

-- Kiểm tra cấu trúc bảng users
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns 
WHERE table_name = 'users' 
ORDER BY ordinal_position;

-- Kiểm tra cấu trúc bảng questions
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns 
WHERE table_name = 'questions' 
ORDER BY ordinal_position;
