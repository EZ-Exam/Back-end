# 🚀 EzExam – Back End API

### 🌐 English | 🇻🇳 Tiếng Việt

---

## 🧩 Overview | Tổng quan

**EzExam** is an intelligent backend system designed for a modern online examination and learning platform.  
It leverages the power of **AI**, **secure authentication**, and **cloud scalability** to provide a fast, secure, and extensible API infrastructure.

**EzExam** là hệ thống backend thông minh dành cho nền tảng thi và học trực tuyến hiện đại.  
Dự án tận dụng sức mạnh của **trí tuệ nhân tạo**, **xác thực bảo mật**, và **hạ tầng đám mây mở rộng** để cung cấp API nhanh, an toàn và dễ mở rộng.

---

## ⚙️ Core Technologies | Công nghệ cốt lõi

| Category | Technology Stack | Mô tả |
|-----------|------------------|--------|
| **Framework** | **ASP.NET Core API** | Nền tảng chính xây dựng hệ thống RESTful API hiệu năng cao, mạnh mẽ và dễ mở rộng. |
| **Database** | **Supabase (PostgreSQL)** | Cung cấp cơ sở dữ liệu quan hệ, realtime và authentication tích hợp. |
| **Storage** | **Supabase Storage** | Lưu trữ file tài liệu, đề thi, hình ảnh người dùng, ... một cách an toàn và nhanh chóng. |
| **Authentication** | **Google OAuth2, Firebase OAuth2** | Cho phép người dùng đăng nhập an toàn qua Google và Firebase. |
| **Encryption & Security** | **JWT + Bcrypt** | Bảo mật API với token JWT và mã hóa mật khẩu bằng Bcrypt. |
| **AI Integration** | **OpenAI API, Gemini API, DeepSeek R1 API** | Tích hợp đa mô hình AI cho gợi ý câu hỏi, chấm điểm tự động, tạo đề thi và phản hồi thông minh. |
| **Payment Gateway** | **PayOS** | Hỗ trợ thanh toán trực tiếp qua VietQR và API PayOS. |
| **Deployment** | **Docker + Linux Server + SSL + Domain** | Đóng gói, triển khai tự động, bảo mật với chứng chỉ SSL và domain riêng. |

---

## 🤖 AI-Powered Features | Tính năng hỗ trợ AI

- 🧠 **Automatic Question Generation** – Sinh câu hỏi từ văn bản, tài liệu hoặc chủ đề học tập.  
- 🧾 **AI Exam Creator** – AI tự động tạo đề thi dựa trên **lịch sử làm bài và mức độ năng lực của người học**.  
- 💬 **Smart Chat Tutor** – Trợ lý học tập sử dụng OpenAI, Gemini và DeepSeek R1.  
- 🧩 **Answer Explanation** – Giải thích đáp án tự động cho người học.  

---

## 🔐 Security Highlights | Bảo mật hệ thống

- JWT-based Authentication for every request.  
- Passwords hashed with **Bcrypt**.  
- OAuth2 for external login (Google, Firebase).  
- HTTPS enforced via SSL certificate.  
- Dockerized environment for process isolation and scaling.

Hệ thống bảo mật gồm:
- Xác thực bằng JWT cho từng request.  
- Mã hóa mật khẩu bằng Bcrypt.  
- Đăng nhập an toàn qua Google và Firebase.  
- Kết nối HTTPS bảo vệ dữ liệu bằng SSL.  
- Môi trường Docker độc lập, an toàn và dễ mở rộng.


---

## ☁️ Deployment Architecture | Kiến trúc triển khai

```plaintext
Client (Mobile / Web)
        │
        ▼
ASP.NET Core API ──► Supabase (DB + Storage)
        │
        ├──► OpenAI / Gemini / DeepSeek R1 (AI Services)
        ├──► PayOS (Payment)
        ├──► Firebase (Auth + Notification)
        ▼
Dockerized Linux Server (SSL + Domain)
```

## ✍️ Liên hệ | Contact

**EzExam Development Team**  
📧 **minhtrifptu@gmail.com** (Trí Nguyễn)  
📧 **nguyenkien264038@gmail.com** (Kiên)  

---

Made with ❤️ by the UnitedTeam  
© 2025 EzExam. All Rights Reserved.
