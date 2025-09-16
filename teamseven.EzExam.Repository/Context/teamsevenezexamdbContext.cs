using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Repository.Context
{
    public partial class teamsevenezexamdbContext : DbContext
    {
        public teamsevenezexamdbContext()
        {
        }

        public teamsevenezexamdbContext(DbContextOptions<teamsevenezexamdbContext> options)
            : base(options)
        {
        }

        // Core tables
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Semester> Semesters { get; set; }

        // Subject and curriculum tables
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<DifficultyLevel> DifficultyLevels { get; set; }
        public virtual DbSet<TextBook> TextBooks { get; set; }
        public virtual DbSet<Chapter> Chapters { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }

        // User management tables
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSocialProvider> UserSocialProviders { get; set; }

        // Question system tables
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<QuestionComment> QuestionComments { get; set; }
        public virtual DbSet<QuestionReport> QuestionReports { get; set; }

        // Exam system tables
        public virtual DbSet<ExamType> ExamTypes { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }
        public virtual DbSet<ExamHistory> ExamHistories { get; set; }

        // Solution system tables
        public virtual DbSet<Solution> Solutions { get; set; }
        public virtual DbSet<SolutionReport> SolutionReports { get; set; }

        // Chatbot system tables
        public virtual DbSet<Chatbot> Chatbots { get; set; }

        // Subscription system tables
        public virtual DbSet<SubscriptionType> SubscriptionTypes { get; set; }
        public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }
        public virtual DbSet<UserUsageTracking> UserUsageTrackings { get; set; }
        public virtual DbSet<UserUsageHistory> UserUsageHistories { get; set; }

        public static string GetConnectionString(string connectionStringName)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            string connectionString = config.GetConnectionString(connectionStringName);
            return connectionString ?? throw new InvalidOperationException("Connection string not found.");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(GetConnectionString("DefaultConnection"),
                options => options.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), null))
                             .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Roles
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_roles");
                entity.ToTable("roles", "public");
                entity.HasIndex(e => e.RoleName, "uq_roles_role_name").IsUnique();
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.RoleName).HasColumnName("RoleName").IsRequired().HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Grades
            modelBuilder.Entity<Grade>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_grades");
                entity.ToTable("grades", "public");
                entity.HasIndex(e => e.Name, "uq_grades_name").IsUnique();
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Semesters
            modelBuilder.Entity<Semester>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_semesters");
                entity.ToTable("semesters", "public");
                entity.HasIndex(e => e.GradeId, "ix_semesters_grade_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(10);
                entity.Property(e => e.GradeId).HasColumnName("GradeId");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Grade).WithMany(p => p.Semesters)
                    .HasForeignKey(d => d.GradeId)
                    .HasConstraintName("fk_semesters_grade_id");
            });

            // Subjects
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_subjects");
                entity.ToTable("subjects", "public");
                entity.HasIndex(e => e.Code, "uq_subjects_code").IsUnique();
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).HasColumnName("Code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Difficulty Levels
            modelBuilder.Entity<DifficultyLevel>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_difficulty_levels");
                entity.ToTable("difficulty_levels", "public");
                entity.HasIndex(e => e.Code, "uq_difficulty_levels_code").IsUnique();
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Code).HasColumnName("Code").IsRequired().HasMaxLength(10);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500);
                entity.Property(e => e.Level).HasColumnName("Level");
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // TextBooks
            modelBuilder.Entity<TextBook>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_textbooks");
                entity.ToTable("textbooks", "public");
                entity.HasIndex(e => new { e.Name, e.GradeId }, "uq_textbooks_name_grade").IsUnique();
                entity.HasIndex(e => e.GradeId, "ix_textbooks_grade_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.GradeId).HasColumnName("GradeId");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Grade).WithMany(p => p.TextBooks)
                    .HasForeignKey(d => d.GradeId)
                    .HasConstraintName("fk_textbooks_grade_id");
            });

            // Chapters
            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_chapters");
                entity.ToTable("chapters", "public");
                entity.HasIndex(e => e.SubjectId, "ix_chapters_subject_id");
                entity.HasIndex(e => e.SemesterId, "ix_chapters_semester_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(200);
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.SemesterId).HasColumnName("SemesterId");
                entity.Property(e => e.Order).HasColumnName("Order").HasDefaultValue(1);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Subject).WithMany(p => p.Chapters)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_chapters_subject_id");
                entity.HasOne(d => d.Semester).WithMany(p => p.Chapters)
                    .HasForeignKey(d => d.SemesterId)
                    .HasConstraintName("fk_chapters_semester_id");
            });

            // Lessons
            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_lessons");
                entity.ToTable("lessons", "public");
                entity.HasIndex(e => e.ChapterId, "ix_lessons_chapter_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.ChapterId).HasColumnName("ChapterId");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Chapter).WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.ChapterId)
                    .HasConstraintName("fk_lessons_chapter_id");
            });

            // Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_users");
                entity.ToTable("users", "public");
                entity.HasIndex(e => e.Email, "uq_users_email").IsUnique();
                entity.HasIndex(e => e.RoleId, "ix_users_role_id");
                entity.HasIndex(e => e.UpdatedBy, "ix_users_updated_by");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Email).HasColumnName("Email").IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).HasColumnName("PasswordHash").HasMaxLength(255);
                entity.Property(e => e.FullName).HasColumnName("FullName").HasMaxLength(255);
                entity.Property(e => e.AvatarUrl).HasColumnName("AvatarUrl").HasMaxLength(1024);
                entity.Property(e => e.PhoneNumber).HasColumnName("PhoneNumber").HasMaxLength(20);
                entity.Property(e => e.RoleId).HasColumnName("RoleId");
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.EmailVerifiedAt).HasColumnName("EmailVerifiedAt");
                entity.Property(e => e.IsPremium).HasColumnName("IsPremium");
                entity.Property(e => e.LastLoginAt).HasColumnName("LastLoginAt");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedBy).HasColumnName("UpdatedBy");
                entity.Property(e => e.Balance).HasColumnName("Balance");
                entity.HasOne(d => d.Role).WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("fk_users_role_id");
                entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InverseUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("fk_users_updated_by");
            });

            // User Social Providers
            modelBuilder.Entity<UserSocialProvider>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_user_social_providers");
                entity.ToTable("user_social_providers", "public");
                entity.HasIndex(e => e.UserId, "ix_user_social_providers_user_id");
                entity.HasIndex(e => new { e.ProviderName, e.ProviderId }, "uq_user_social_providers_provider").IsUnique();
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.ProviderName).HasColumnName("ProviderName").IsRequired().HasMaxLength(50);
                entity.Property(e => e.ProviderId).HasColumnName("ProviderId").IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).HasColumnName("Email").HasMaxLength(255);
                entity.Property(e => e.ProfileUrl).HasColumnName("ProfileUrl").HasMaxLength(2048);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.User).WithMany(p => p.UserSocialProviders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user_social_providers_user_id");
            });

            // Questions
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_questions");
                entity.ToTable("questions", "public");
                entity.HasIndex(e => e.CreatedByUserId, "ix_questions_created_by_user_id");
                entity.HasIndex(e => e.SubjectId, "ix_questions_subject_id");
                entity.HasIndex(e => e.DifficultyLevelId, "ix_questions_difficulty_level_id");
                entity.HasIndex(e => e.ChapterId, "ix_questions_chapter_id");
                entity.HasIndex(e => e.LessonId, "ix_questions_lesson_id");
                entity.HasIndex(e => e.TextbookId, "ix_questions_textbook_id");
                entity.HasIndex(e => e.TemplateQuestionId, "ix_questions_template_question_id");
                entity.HasIndex(e => e.QuestionType, "ix_questions_question_type");
                entity.HasIndex(e => e.IsActive, "ix_questions_is_active");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Content).HasColumnName("Content").IsRequired().HasMaxLength(5000);
                entity.Property(e => e.QuestionSource).HasColumnName("QuestionSource").HasMaxLength(500);
                entity.Property(e => e.DifficultyLevelId).HasColumnName("DifficultyLevelId");
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.ChapterId).HasColumnName("ChapterId");
                entity.Property(e => e.LessonId).HasColumnName("LessonId");
                entity.Property(e => e.TextbookId).HasColumnName("TextbookId");
                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId");
                entity.Property(e => e.QuestionType).HasColumnName("QuestionType").HasMaxLength(20).HasDefaultValue("MULTIPLE_CHOICE");
                entity.Property(e => e.Image).HasColumnName("Image").HasMaxLength(5000);
                entity.Property(e => e.TemplateQuestionId).HasColumnName("TemplateQuestionId");
                entity.Property(e => e.IsCloned).HasColumnName("IsCloned").HasDefaultValue(false);
                entity.Property(e => e.ViewCount).HasColumnName("ViewCount").HasDefaultValue(0);
                entity.Property(e => e.AverageRating).HasColumnName("AverageRating").HasColumnType("decimal(3,2)").HasDefaultValue(0);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Questions)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("fk_questions_created_by_user_id");
                entity.HasOne(d => d.Subject).WithMany(p => p.Questions)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_questions_subject_id");
                entity.HasOne(d => d.DifficultyLevel).WithMany(p => p.Questions)
                    .HasForeignKey(d => d.DifficultyLevelId)
                    .HasConstraintName("fk_questions_difficulty_level_id");
                entity.HasOne(d => d.Chapter).WithMany(p => p.Questions)
                    .HasForeignKey(d => d.ChapterId)
                    .HasConstraintName("fk_questions_chapter_id");
                entity.HasOne(d => d.Lesson).WithMany(p => p.Questions)
                    .HasForeignKey(d => d.LessonId)
                    .HasConstraintName("fk_questions_lesson_id");
                entity.HasOne(d => d.Textbook).WithMany(p => p.Questions)
                    .HasForeignKey(d => d.TextbookId)
                    .HasConstraintName("fk_questions_textbook_id");
                entity.HasOne(d => d.TemplateQuestion).WithMany(p => p.ClonedQuestions)
                    .HasForeignKey(d => d.TemplateQuestionId)
                    .HasConstraintName("fk_questions_template_question_id");
            });

            // Answers
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_answers");
                entity.ToTable("answers", "public");
                entity.HasIndex(e => e.QuestionId, "ix_answers_question_id");
                entity.HasIndex(e => e.IsCorrect, "ix_answers_is_correct");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.AnswerKey).HasColumnName("AnswerKey").IsRequired().HasMaxLength(10);
                entity.Property(e => e.Content).HasColumnName("Content").IsRequired().HasMaxLength(5000);
                entity.Property(e => e.IsCorrect).HasColumnName("IsCorrect").HasDefaultValue(false);
                entity.Property(e => e.Explanation).HasColumnName("Explanation").HasMaxLength(5000);
                entity.Property(e => e.Order).HasColumnName("Order").HasDefaultValue(1);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_answers_question_id");
            });

            // Question Comments
            modelBuilder.Entity<QuestionComment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_question_comments");
                entity.ToTable("question_comments", "public");
                entity.HasIndex(e => e.QuestionId, "ix_question_comments_question_id");
                entity.HasIndex(e => e.UserId, "ix_question_comments_user_id");
                entity.HasIndex(e => e.ParentCommentId, "ix_question_comments_parent_comment_id");
                entity.HasIndex(e => e.IsApproved, "ix_question_comments_is_approved");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.Content).HasColumnName("Content").IsRequired().HasMaxLength(2000);
                entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentId");
                entity.Property(e => e.Rating).HasColumnName("Rating").HasDefaultValue(0);
                entity.Property(e => e.IsHelpful).HasColumnName("IsHelpful").HasDefaultValue(false);
                entity.Property(e => e.IsApproved).HasColumnName("IsApproved").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Question).WithMany(p => p.QuestionComments)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_question_comments_question_id");
                entity.HasOne(d => d.User).WithMany(p => p.QuestionComments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_question_comments_user_id");
                entity.HasOne(d => d.ParentComment).WithMany(p => p.Replies)
                    .HasForeignKey(d => d.ParentCommentId)
                    .HasConstraintName("fk_question_comments_parent_comment_id");
            });

            // Question Reports
            modelBuilder.Entity<QuestionReport>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_question_reports");
                entity.ToTable("question_reports", "public");
                entity.HasIndex(e => e.QuestionId, "ix_question_reports_question_id");
                entity.HasIndex(e => e.ReportedByUserId, "ix_question_reports_reported_by_user_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.ReportedByUserId).HasColumnName("ReportedByUserId");
                entity.Property(e => e.Reason).HasColumnName("Reason").IsRequired().HasMaxLength(1000);
                entity.Property(e => e.ReportDate).HasColumnName("ReportDate").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsHandled).HasColumnName("IsHandled").HasDefaultValue(false);
                entity.HasOne(d => d.Question).WithMany(p => p.QuestionReports)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_question_reports_question_id");
                entity.HasOne(d => d.ReportedByUser).WithMany(p => p.QuestionReports)
                    .HasForeignKey(d => d.ReportedByUserId)
                    .HasConstraintName("fk_question_reports_reported_by_user_id");
            });

            // Exam Types
            modelBuilder.Entity<ExamType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_exam_types");
                entity.ToTable("exam_types", "public");
                entity.HasIndex(e => e.Name, "uq_exam_types_name").IsUnique();
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Exams
            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_exams");
                entity.ToTable("exams", "public");
                entity.HasIndex(e => e.CreatedByUserId, "ix_exams_created_by_user_id");
                entity.HasIndex(e => e.ExamTypeId, "ix_exams_exam_type_id");
                entity.HasIndex(e => e.SubjectId, "ix_exams_subject_id");
                entity.HasIndex(e => e.LessonId, "ix_exams_lesson_id");
                entity.HasIndex(e => e.IsDeleted, "ix_exams_is_deleted");
                entity.HasIndex(e => e.IsActive, "ix_exams_is_active");
                entity.HasIndex(e => e.IsPublic, "ix_exams_is_public");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(1000);
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.LessonId).HasColumnName("LessonId");
                entity.Property(e => e.ExamTypeId).HasColumnName("ExamTypeId");
                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId");
                entity.Property(e => e.TimeLimit).HasColumnName("TimeLimit").HasDefaultValue(60);
                entity.Property(e => e.TotalQuestions).HasColumnName("TotalQuestions").HasDefaultValue(0);
                entity.Property(e => e.TotalMarks).HasColumnName("TotalMarks").HasDefaultValue(100);
                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted").HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.IsPublic).HasColumnName("IsPublic").HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Exams)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("fk_exams_created_by_user_id");
                entity.HasOne(d => d.ExamType).WithMany(p => p.Exams)
                    .HasForeignKey(d => d.ExamTypeId)
                    .HasConstraintName("fk_exams_exam_type_id");
                entity.HasOne(d => d.Subject).WithMany(p => p.Exams)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_exams_subject_id");
                entity.HasOne(d => d.Lesson).WithMany(p => p.Exams)
                    .HasForeignKey(d => d.LessonId)
                    .HasConstraintName("fk_exams_lesson_id");
            });

            // Exam Questions
            modelBuilder.Entity<ExamQuestion>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_exam_questions");
                entity.ToTable("exam_questions", "public");
                entity.HasIndex(e => e.ExamId, "ix_exam_questions_exam_id");
                entity.HasIndex(e => e.QuestionId, "ix_exam_questions_question_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.ExamId).HasColumnName("ExamId");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.Order).HasColumnName("Order");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Exam).WithMany(p => p.ExamQuestions)
                    .HasForeignKey(d => d.ExamId)
                    .HasConstraintName("fk_exam_questions_exam_id");
                entity.HasOne(d => d.Question).WithMany(p => p.ExamQuestions)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_exam_questions_question_id");
            });

            // Exam Histories
            modelBuilder.Entity<ExamHistory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_exam_histories");
                entity.ToTable("exam_histories", "public");
                entity.HasIndex(e => e.ExamId, "ix_exam_histories_exam_id");
                entity.HasIndex(e => e.ActionByUserId, "ix_exam_histories_action_by_user_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.ExamId).HasColumnName("ExamId");
                entity.Property(e => e.ActionByUserId).HasColumnName("ActionByUserId");
                entity.Property(e => e.Action).HasColumnName("Action").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500);
                entity.Property(e => e.ActionDate).HasColumnName("ActionDate").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Exam).WithMany(p => p.ExamHistories)
                    .HasForeignKey(d => d.ExamId)
                    .HasConstraintName("fk_exam_histories_exam_id");
                entity.HasOne(d => d.ActionByUser).WithMany(p => p.ExamHistories)
                    .HasForeignKey(d => d.ActionByUserId)
                    .HasConstraintName("fk_exam_histories_action_by_user_id");
            });

            // Solutions
            modelBuilder.Entity<Solution>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_solutions");
                entity.ToTable("solutions", "public");
                entity.HasIndex(e => e.QuestionId, "ix_solutions_question_id");
                entity.HasIndex(e => e.CreatedByUserId, "ix_solutions_created_by_user_id");
                entity.HasIndex(e => e.OriginalSolutionId, "ix_solutions_original_solution_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.CreatedByUserId).HasColumnName("CreatedByUserId");
                entity.Property(e => e.Content).HasColumnName("Content").IsRequired().HasMaxLength(5000);
                entity.Property(e => e.Explanation).HasColumnName("Explanation").HasMaxLength(10000);
                entity.Property(e => e.PythonScript).HasColumnName("PythonScript").HasMaxLength(10000);
                entity.Property(e => e.Mp4Url).HasColumnName("Mp4Url").HasMaxLength(1000);
                entity.Property(e => e.IsApproved).HasColumnName("IsApproved").HasDefaultValue(true);
                entity.Property(e => e.IsDeleted).HasColumnName("IsDeleted").IsRequired(false);
                entity.Property(e => e.IsMp4Generated).HasColumnName("IsMp4Generated").HasDefaultValue(false);
                entity.Property(e => e.IsMp4Reused).HasColumnName("IsMp4Reused").HasDefaultValue(false);
                entity.Property(e => e.OriginalSolutionId).HasColumnName("OriginalSolutionId");
                entity.Property(e => e.VideoData).HasColumnName("VideoData");
                entity.Property(e => e.VideoContentType).HasColumnName("VideoContentType").HasMaxLength(255);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Question).WithMany(p => p.Solutions)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_solutions_question_id");
                entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Solutions)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("fk_solutions_created_by_user_id");
                entity.HasOne(d => d.OriginalSolution).WithMany(p => p.ReusedSolutions)
                    .HasForeignKey(d => d.OriginalSolutionId)
                    .HasConstraintName("fk_solutions_original_solution_id");
            });

            // Solution Reports
            modelBuilder.Entity<SolutionReport>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_solution_reports");
                entity.ToTable("solution_reports", "public");
                entity.HasIndex(e => e.SolutionId, "ix_solution_reports_solution_id");
                entity.HasIndex(e => e.ReportedByUserId, "ix_solution_reports_reported_by_user_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.SolutionId).HasColumnName("SolutionId");
                entity.Property(e => e.ReportedByUserId).HasColumnName("ReportedByUserId");
                entity.Property(e => e.Reason).HasColumnName("Reason").IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Status).HasColumnName("Status").IsRequired().HasMaxLength(50).HasDefaultValue("Pending");
                entity.Property(e => e.ReportDate).HasColumnName("ReportDate").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Solution).WithMany(p => p.SolutionReports)
                    .HasForeignKey(d => d.SolutionId)
                    .HasConstraintName("fk_solution_reports_solution_id");
                entity.HasOne(d => d.ReportedByUser).WithMany(p => p.SolutionReports)
                    .HasForeignKey(d => d.ReportedByUserId)
                    .HasConstraintName("fk_solution_reports_reported_by_user_id");
            });

            // Chatbots
            modelBuilder.Entity<Chatbot>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_chatbots");
                entity.ToTable("chatbots", "public");
                entity.HasIndex(e => e.UserId, "ix_chatbots_user_id");
                entity.HasIndex(e => e.QuestionId, "ix_chatbots_question_id");
                entity.HasIndex(e => e.SubjectId, "ix_chatbots_subject_id");
                entity.HasIndex(e => e.CreatedAt, "ix_chatbots_created_at");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.UserMessage).HasColumnName("UserMessage").IsRequired().HasMaxLength(2000);
                entity.Property(e => e.BotResponse).HasColumnName("BotResponse").IsRequired().HasMaxLength(5000);
                entity.Property(e => e.MessageType).HasColumnName("MessageType").HasMaxLength(50).HasDefaultValue("TEXT");
                entity.Property(e => e.Context).HasColumnName("Context").HasMaxLength(100);
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.IsHelpful).HasColumnName("IsHelpful").HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.User).WithMany(p => p.Chatbots)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_chatbots_user_id");
                entity.HasOne(d => d.Question).WithMany(p => p.Chatbots)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_chatbots_question_id");
                entity.HasOne(d => d.Subject).WithMany(p => p.Chatbots)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_chatbots_subject_id");
            });

            // Subscription Types
            modelBuilder.Entity<SubscriptionType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_subscription_types");
                entity.ToTable("subscription_types", "public");
                entity.HasIndex(e => e.SubscriptionCode, "uq_subscription_types_subscription_code").IsUnique();
                entity.HasIndex(e => e.UpdatedBy, "ix_subscription_types_updated_by");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.SubscriptionCode).HasColumnName("SubscriptionCode").IsRequired().HasMaxLength(50);
                entity.Property(e => e.SubscriptionName).HasColumnName("SubscriptionName").IsRequired().HasMaxLength(255);
                entity.Property(e => e.SubscriptionPrice).HasColumnName("SubscriptionPrice").HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500);
                entity.Property(e => e.MaxSolutionViews).HasColumnName("MaxSolutionViews").HasDefaultValue(0);
                entity.Property(e => e.MaxAIRequests).HasColumnName("MaxAIRequests").HasDefaultValue(0);
                entity.Property(e => e.IsAIEnabled).HasColumnName("IsAIEnabled").HasDefaultValue(false);
                entity.Property(e => e.Features).HasColumnName("Features").HasMaxLength(2000);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedBy).HasColumnName("UpdatedBy");
                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("fk_subscription_types_updated_by");
            });

            // User Subscriptions
            modelBuilder.Entity<UserSubscription>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_user_subscriptions");
                entity.ToTable("user_subscriptions", "public");
                entity.HasIndex(e => e.UserId, "ix_user_subscriptions_user_id");
                entity.HasIndex(e => e.SubscriptionTypeId, "ix_user_subscriptions_subscription_type_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.SubscriptionTypeId).HasColumnName("SubscriptionTypeId");
                entity.Property(e => e.Amount).HasColumnName("Amount");
                entity.Property(e => e.PaymentStatus).HasColumnName("PaymentStatus").IsRequired().HasMaxLength(50);
                entity.Property(e => e.PaymentGatewayTransactionId).HasColumnName("PaymentGatewayTransactionId").HasMaxLength(255);
                entity.Property(e => e.StartDate).HasColumnName("StartDate").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EndDate).HasColumnName("EndDate");
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.User).WithMany(p => p.UserSubscriptions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user_subscriptions_user_id");
                entity.HasOne(d => d.SubscriptionType).WithMany(p => p.UserSubscriptions)
                    .HasForeignKey(d => d.SubscriptionTypeId)
                    .HasConstraintName("fk_user_subscriptions_subscription_type_id");
            });

            // User Usage Tracking
            modelBuilder.Entity<UserUsageTracking>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_user_usage_tracking");
                entity.ToTable("user_usage_tracking", "public");
                entity.HasIndex(e => e.UserId, "ix_user_usage_tracking_user_id");
                entity.HasIndex(e => e.SubscriptionTypeId, "ix_user_usage_tracking_subscription_type_id");
                entity.HasIndex(e => e.UsageType, "ix_user_usage_tracking_usage_type");
                entity.HasIndex(e => e.ResetDate, "ix_user_usage_tracking_reset_date");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.SubscriptionTypeId).HasColumnName("SubscriptionTypeId");
                entity.Property(e => e.UsageType).HasColumnName("UsageType").IsRequired().HasMaxLength(50);
                entity.Property(e => e.UsageCount).HasColumnName("UsageCount").HasDefaultValue(0);
                entity.Property(e => e.ResetDate).HasColumnName("ResetDate");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.User).WithMany(p => p.UserUsageTrackings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user_usage_tracking_user_id");
                entity.HasOne(d => d.SubscriptionType).WithMany(p => p.UserUsageTrackings)
                    .HasForeignKey(d => d.SubscriptionTypeId)
                    .HasConstraintName("fk_user_usage_tracking_subscription_type_id");
            });

            // User Usage History
            modelBuilder.Entity<UserUsageHistory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_user_usage_history");
                entity.ToTable("user_usage_history", "public");
                entity.HasIndex(e => e.UserId, "ix_user_usage_history_user_id");
                entity.HasIndex(e => e.UsageType, "ix_user_usage_history_usage_type");
                entity.HasIndex(e => e.CreatedAt, "ix_user_usage_history_created_at");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.UsageType).HasColumnName("UsageType").IsRequired().HasMaxLength(50);
                entity.Property(e => e.ResourceId).HasColumnName("ResourceId");
                entity.Property(e => e.ResourceType).HasColumnName("ResourceType").HasMaxLength(50);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.User).WithMany(p => p.UserUsageHistories)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user_usage_history_user_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
