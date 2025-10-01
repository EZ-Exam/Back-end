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

        public virtual DbSet<LessonEnhanced> LessonsEnhanced { get; set; }
        public virtual DbSet<LessonEnhancedQuestion> LessonsEnhancedQuestions { get; set; }


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

        // Test system tables
        public virtual DbSet<UserQuestionCart> UserQuestionCarts { get; set; }
        public virtual DbSet<AITestRecommendation> AITestRecommendations { get; set; }
        public virtual DbSet<UserLearningHistory> UserLearningHistories { get; set; }
        public virtual DbSet<TestSession> TestSessions { get; set; }
        public virtual DbSet<TestSessionAnswer> TestSessionAnswers { get; set; }
        public virtual DbSet<UserCompetencyAssessment> UserCompetencyAssessments { get; set; }
        public virtual DbSet<UserQuestionAttempt> UserQuestionAttempts { get; set; }
        public virtual DbSet<UserOverallCompetency> UserOverallCompetencies { get; set; }

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
                options => options.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), null)
                                 .CommandTimeout(30))
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
                entity.HasIndex(e => e.SubjectId, "ix_textbooks_subject_id");
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
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
                entity.Property(e => e.Document).HasColumnName("Document").HasMaxLength(5000);
                entity.Property(e => e.DocumentType).HasColumnName("DocumentType").HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.Chapter).WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.ChapterId)
                    .HasConstraintName("fk_lessons_chapter_id");
            });
            // ===== Lessons Enhanced (bảng chính) =====
            modelBuilder.Entity<LessonEnhanced>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_lessons_enhanced");
                entity.ToTable("lessons_enhanced", "public");

                entity.HasIndex(e => e.SubjectId, "idx_le_subject");

                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Title).HasColumnName("Title").IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(1000);
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.PdfUrl).HasColumnName("PdfUrl").HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");

                // 1 - n tới bảng nối (EF-side; DB không cần FK vẫn map được)
                entity.HasMany(e => e.LessonQuestions)
                      .WithOne(lq => lq.Lesson)
                      .HasForeignKey(lq => lq.LessonId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== Lessons Enhanced Questions (bảng nối) =====
            modelBuilder.Entity<LessonEnhancedQuestion>(entity =>
            {
                entity.HasKey(e => new { e.LessonId, e.QuestionId }).HasName("pk_lessons_enhanced_questions");
                entity.ToTable("lessons_enhanced_questions", "public");

                entity.HasIndex(e => e.LessonId, "idx_leq_lesson");
                entity.HasIndex(e => e.QuestionId, "idx_leq_question");

                entity.Property(e => e.LessonId).HasColumnName("LessonId");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.Position).HasColumnName("Position");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");

                // n - 1 tới Question (không cần thêm nav collection ở Question)
                entity.HasOne(e => e.Question)
                      .WithMany()
                      .HasForeignKey(e => e.QuestionId)
                      .OnDelete(DeleteBehavior.Restrict);
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
                entity.Property(e => e.TypeCode).HasColumnName("TypeCode").IsRequired().HasMaxLength(20);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500);
                entity.Property(e => e.DefaultDuration).HasColumnName("DefaultDuration");
                entity.Property(e => e.MaxQuestions).HasColumnName("MaxQuestions");
                entity.Property(e => e.MinQuestions).HasColumnName("MinQuestions");
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
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
                entity.Property(e => e.TimeLimit).HasColumnName("TimeLimit");
                entity.Property(e => e.TotalQuestions).HasColumnName("TotalQuestions").HasDefaultValue(0);
                entity.Property(e => e.TotalMarks).HasColumnName("TotalMarks");
                entity.Property(e => e.Duration).HasColumnName("Duration");
                entity.Property(e => e.TestConfiguration).HasColumnName("TestConfiguration").HasMaxLength(2000);
                entity.Property(e => e.DifficultyDistribution).HasColumnName("DifficultyDistribution").HasMaxLength(500);
                entity.Property(e => e.TopicDistribution).HasColumnName("TopicDistribution").HasMaxLength(1000);
                entity.Property(e => e.IsAutoGenerated).HasColumnName("IsAutoGenerated").HasDefaultValue(false);
                entity.Property(e => e.GenerationSource).HasColumnName("GenerationSource").HasMaxLength(50);
                entity.Property(e => e.AITestRecommendationId).HasColumnName("AITestRecommendationId");
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
                entity.HasOne(d => d.AITestRecommendation).WithMany()
                    .HasForeignKey(d => d.AITestRecommendationId)
                    .HasConstraintName("fk_exams_ai_test_recommendation_id");
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

            // Test System Models Configuration
            ConfigureTestSystemModels(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        private void ConfigureTestSystemModels(ModelBuilder modelBuilder)
        {
            // User Question Cart
            modelBuilder.Entity<UserQuestionCart>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_user_question_carts");
                entity.ToTable("user_question_carts", "public");
                entity.HasIndex(e => e.UserId, "ix_user_question_carts_user_id");
                entity.HasIndex(e => e.QuestionId, "ix_user_question_carts_question_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.AddedAt).HasColumnName("AddedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsSelected).HasColumnName("IsSelected").HasDefaultValue(true);
                entity.Property(e => e.UserNotes).HasColumnName("UserNotes").HasMaxLength(1000);
                entity.Property(e => e.DifficultyPreference).HasColumnName("DifficultyPreference").HasMaxLength(20);
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.ChapterId).HasColumnName("ChapterId");
                entity.Property(e => e.LessonId).HasColumnName("LessonId");
                entity.HasOne(d => d.User).WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user_question_carts_user_id");
                entity.HasOne(d => d.Question).WithMany()
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_user_question_carts_question_id");
                entity.HasOne(d => d.Subject).WithMany()
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_user_question_carts_subject_id");
                entity.HasOne(d => d.Chapter).WithMany()
                    .HasForeignKey(d => d.ChapterId)
                    .HasConstraintName("fk_user_question_carts_chapter_id");
                entity.HasOne(d => d.Lesson).WithMany()
                    .HasForeignKey(d => d.LessonId)
                    .HasConstraintName("fk_user_question_carts_lesson_id");
            });

            // AI Test Recommendation
            modelBuilder.Entity<AITestRecommendation>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_ai_test_recommendations");
                entity.ToTable("ai_test_recommendations", "public");
                entity.HasIndex(e => e.UserId, "ix_ai_test_recommendations_user_id");
                entity.HasIndex(e => e.SubjectId, "ix_ai_test_recommendations_subject_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.TestName).HasColumnName("TestName").IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(1000);
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.ChapterId).HasColumnName("ChapterId");
                entity.Property(e => e.LessonId).HasColumnName("LessonId");
                entity.Property(e => e.RecommendedDuration).HasColumnName("RecommendedDuration");
                entity.Property(e => e.RecommendedQuestionCount).HasColumnName("RecommendedQuestionCount");
                entity.Property(e => e.DifficultyDistribution).HasColumnName("DifficultyDistribution").HasMaxLength(500);
                entity.Property(e => e.TopicDistribution).HasColumnName("TopicDistribution").HasMaxLength(1000);
                entity.Property(e => e.BasedOnHistory).HasColumnName("BasedOnHistory").HasDefaultValue(true);
                entity.Property(e => e.BasedOnWeakAreas).HasColumnName("BasedOnWeakAreas").HasDefaultValue(true);
                entity.Property(e => e.BasedOnProgress).HasColumnName("BasedOnProgress").HasDefaultValue(true);
                entity.Property(e => e.ConfidenceScore).HasColumnName("ConfidenceScore").HasColumnType("decimal(3,2)").HasDefaultValue(0);
                entity.Property(e => e.IsAccepted).HasColumnName("IsAccepted");
                entity.Property(e => e.IsGenerated).HasColumnName("IsGenerated").HasDefaultValue(false);
                entity.Property(e => e.GeneratedExamId).HasColumnName("GeneratedExamId");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.ExpiresAt).HasColumnName("ExpiresAt");
                entity.HasOne(d => d.User).WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_ai_test_recommendations_user_id");
                entity.HasOne(d => d.Subject).WithMany()
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_ai_test_recommendations_subject_id");
                entity.HasOne(d => d.Chapter).WithMany()
                    .HasForeignKey(d => d.ChapterId)
                    .HasConstraintName("fk_ai_test_recommendations_chapter_id");
                entity.HasOne(d => d.Lesson).WithMany()
                    .HasForeignKey(d => d.LessonId)
                    .HasConstraintName("fk_ai_test_recommendations_lesson_id");
                // Foreign key to Exam removed to avoid circular reference
                // Use Exam.AITestRecommendation navigation instead
            });

            // User Learning History
            modelBuilder.Entity<UserLearningHistory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_user_learning_histories");
                entity.ToTable("user_learning_histories", "public");
                entity.HasIndex(e => e.UserId, "ix_user_learning_histories_user_id");
                entity.HasIndex(e => e.SubjectId, "ix_user_learning_histories_subject_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.ChapterId).HasColumnName("ChapterId");
                entity.Property(e => e.LessonId).HasColumnName("LessonId");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.ExamId).HasColumnName("ExamId");
                entity.Property(e => e.ActivityType).HasColumnName("ActivityType").IsRequired().HasMaxLength(50);
                entity.Property(e => e.TimeSpent).HasColumnName("TimeSpent").HasDefaultValue(0);
                entity.Property(e => e.Score).HasColumnName("Score").HasColumnType("decimal(5,2)");
                entity.Property(e => e.IsCorrect).HasColumnName("IsCorrect");
                entity.Property(e => e.DifficultyLevel).HasColumnName("DifficultyLevel").HasMaxLength(20);
                entity.Property(e => e.TopicTags).HasColumnName("TopicTags").HasMaxLength(500);
                entity.Property(e => e.WeakAreas).HasColumnName("WeakAreas").HasMaxLength(1000);
                entity.Property(e => e.Strengths).HasColumnName("Strengths").HasMaxLength(1000);
                entity.Property(e => e.LearningProgress).HasColumnName("LearningProgress").HasColumnType("decimal(3,2)").HasDefaultValue(0);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.User).WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user_learning_histories_user_id");
                entity.HasOne(d => d.Subject).WithMany()
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_user_learning_histories_subject_id");
                entity.HasOne(d => d.Chapter).WithMany()
                    .HasForeignKey(d => d.ChapterId)
                    .HasConstraintName("fk_user_learning_histories_chapter_id");
                entity.HasOne(d => d.Lesson).WithMany()
                    .HasForeignKey(d => d.LessonId)
                    .HasConstraintName("fk_user_learning_histories_lesson_id");
                entity.HasOne(d => d.Question).WithMany()
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_user_learning_histories_question_id");
                entity.HasOne(d => d.Exam).WithMany()
                    .HasForeignKey(d => d.ExamId)
                    .HasConstraintName("fk_user_learning_histories_exam_id");
            });

            // Test Session
            modelBuilder.Entity<TestSession>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_test_sessions");
                entity.ToTable("test_sessions", "public");
                entity.HasIndex(e => e.UserId, "ix_test_sessions_user_id");
                entity.HasIndex(e => e.ExamId, "ix_test_sessions_exam_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.ExamId).HasColumnName("ExamId");
                entity.Property(e => e.SessionStatus).HasColumnName("SessionStatus").IsRequired().HasMaxLength(50).HasDefaultValue("NOT_STARTED");
                entity.Property(e => e.StartedAt).HasColumnName("StartedAt");
                entity.Property(e => e.CompletedAt).HasColumnName("CompletedAt");
                entity.Property(e => e.TimeSpent).HasColumnName("TimeSpent").HasDefaultValue(0);
                entity.Property(e => e.TotalScore).HasColumnName("TotalScore").HasColumnType("decimal(5,2)");
                entity.Property(e => e.CorrectAnswers).HasColumnName("CorrectAnswers").HasDefaultValue(0);
                entity.Property(e => e.TotalQuestions).HasColumnName("TotalQuestions").HasDefaultValue(0);
                entity.Property(e => e.IsPassed).HasColumnName("IsPassed");
                entity.Property(e => e.PassingScore).HasColumnName("PassingScore").HasColumnType("decimal(5,2)");
                entity.Property(e => e.SessionData).HasColumnName("SessionData").HasMaxLength(10000);
                entity.Property(e => e.DeviceInfo).HasColumnName("DeviceInfo").HasMaxLength(500);
                entity.Property(e => e.IsCheatingDetected).HasColumnName("IsCheatingDetected").HasDefaultValue(false);
                entity.Property(e => e.CheatingDetails).HasColumnName("CheatingDetails").HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.User).WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_test_sessions_user_id");
                entity.HasOne(d => d.Exam).WithMany()
                    .HasForeignKey(d => d.ExamId)
                    .HasConstraintName("fk_test_sessions_exam_id");
            });

            // Test Session Answer
            modelBuilder.Entity<TestSessionAnswer>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_test_session_answers");
                entity.ToTable("test_session_answers", "public");
                entity.HasIndex(e => e.TestSessionId, "ix_test_session_answers_test_session_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.TestSessionId).HasColumnName("TestSessionId");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.SelectedAnswerId).HasColumnName("SelectedAnswerId");
                entity.Property(e => e.UserAnswer).HasColumnName("UserAnswer").HasMaxLength(5000);
                entity.Property(e => e.IsCorrect).HasColumnName("IsCorrect");
                entity.Property(e => e.TimeSpent).HasColumnName("TimeSpent").HasDefaultValue(0);
                entity.Property(e => e.AnsweredAt).HasColumnName("AnsweredAt");
                entity.Property(e => e.IsMarkedForReview).HasColumnName("IsMarkedForReview").HasDefaultValue(false);
                entity.Property(e => e.ConfidenceLevel).HasColumnName("ConfidenceLevel");
                entity.Property(e => e.AnswerSequence).HasColumnName("AnswerSequence").HasDefaultValue(0);
                entity.Property(e => e.IsChanged).HasColumnName("IsChanged").HasDefaultValue(false);
                entity.Property(e => e.ChangeCount).HasColumnName("ChangeCount").HasDefaultValue(0);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.TestSession).WithMany(p => p.TestSessionAnswers)
                    .HasForeignKey(d => d.TestSessionId)
                    .HasConstraintName("fk_test_session_answers_test_session_id");
                entity.HasOne(d => d.Question).WithMany()
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_test_session_answers_question_id");
                entity.HasOne(d => d.SelectedAnswer).WithMany()
                    .HasForeignKey(d => d.SelectedAnswerId)
                    .HasConstraintName("fk_test_session_answers_selected_answer_id");
            });

            // User Competency Assessment
            modelBuilder.Entity<UserCompetencyAssessment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_user_competency_assessments");
                entity.ToTable("user_competency_assessments", "public");
                entity.HasIndex(e => e.UserId, "ix_user_competency_assessments_user_id");
                entity.HasIndex(e => e.SubjectId, "ix_user_competency_assessments_subject_id");
                entity.HasIndex(e => e.Topic, "ix_user_competency_assessments_topic");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.ChapterId).HasColumnName("ChapterId");
                entity.Property(e => e.LessonId).HasColumnName("LessonId");
                entity.Property(e => e.DifficultyLevel).HasColumnName("DifficultyLevel").IsRequired().HasMaxLength(20);
                entity.Property(e => e.Topic).HasColumnName("Topic").IsRequired().HasMaxLength(100);
                entity.Property(e => e.TotalQuestions).HasColumnName("TotalQuestions").HasDefaultValue(0);
                entity.Property(e => e.CorrectAnswers).HasColumnName("CorrectAnswers").HasDefaultValue(0);
                entity.Property(e => e.IncorrectAnswers).HasColumnName("IncorrectAnswers").HasDefaultValue(0);
                entity.Property(e => e.AccuracyRate).HasColumnName("AccuracyRate").HasColumnType("decimal(5,2)").HasDefaultValue(0);
                entity.Property(e => e.CompetencyScore).HasColumnName("CompetencyScore").HasColumnType("decimal(3,2)").HasDefaultValue(0);
                entity.Property(e => e.ConfidenceLevel).HasColumnName("ConfidenceLevel").HasColumnType("decimal(3,2)").HasDefaultValue(0);
                entity.Property(e => e.AverageTimePerQuestion).HasColumnName("AverageTimePerQuestion").HasColumnType("decimal(8,2)").HasDefaultValue(0);
                entity.Property(e => e.TotalTimeSpent).HasColumnName("TotalTimeSpent").HasDefaultValue(0);
                entity.Property(e => e.LastAttemptedAt).HasColumnName("LastAttemptedAt");
                entity.Property(e => e.FirstAttemptedAt).HasColumnName("FirstAttemptedAt");
                entity.Property(e => e.ImprovementTrend).HasColumnName("ImprovementTrend").HasColumnType("decimal(5,2)").HasDefaultValue(0);
                entity.Property(e => e.WeaknessAreas).HasColumnName("WeaknessAreas").HasMaxLength(1000);
                entity.Property(e => e.StrengthAreas).HasColumnName("StrengthAreas").HasMaxLength(1000);
                entity.Property(e => e.RecommendedActions).HasColumnName("RecommendedActions").HasMaxLength(1000);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.User).WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user_competency_assessments_user_id");
                entity.HasOne(d => d.Subject).WithMany()
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_user_competency_assessments_subject_id");
                entity.HasOne(d => d.Chapter).WithMany()
                    .HasForeignKey(d => d.ChapterId)
                    .HasConstraintName("fk_user_competency_assessments_chapter_id");
                entity.HasOne(d => d.Lesson).WithMany()
                    .HasForeignKey(d => d.LessonId)
                    .HasConstraintName("fk_user_competency_assessments_lesson_id");
            });

            // User Question Attempt
            modelBuilder.Entity<UserQuestionAttempt>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_user_question_attempts");
                entity.ToTable("user_question_attempts", "public");
                entity.HasIndex(e => e.UserId, "ix_user_question_attempts_user_id");
                entity.HasIndex(e => e.QuestionId, "ix_user_question_attempts_question_id");
                entity.HasIndex(e => e.SubjectId, "ix_user_question_attempts_subject_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.QuestionId).HasColumnName("QuestionId");
                entity.Property(e => e.SelectedAnswerId).HasColumnName("SelectedAnswerId");
                entity.Property(e => e.UserAnswer).HasColumnName("UserAnswer").HasMaxLength(5000);
                entity.Property(e => e.IsCorrect).HasColumnName("IsCorrect");
                entity.Property(e => e.TimeSpent).HasColumnName("TimeSpent").HasDefaultValue(0);
                entity.Property(e => e.ConfidenceLevel).HasColumnName("ConfidenceLevel");
                entity.Property(e => e.AttemptType).HasColumnName("AttemptType").IsRequired().HasMaxLength(50).HasDefaultValue("PRACTICE");
                entity.Property(e => e.SessionId).HasColumnName("SessionId");
                entity.Property(e => e.ExamId).HasColumnName("ExamId");
                entity.Property(e => e.DifficultyLevel).HasColumnName("DifficultyLevel").HasMaxLength(20);
                entity.Property(e => e.Topic).HasColumnName("Topic").HasMaxLength(100);
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.ChapterId).HasColumnName("ChapterId");
                entity.Property(e => e.LessonId).HasColumnName("LessonId");
                entity.Property(e => e.IsHintUsed).HasColumnName("IsHintUsed").HasDefaultValue(false);
                entity.Property(e => e.HintCount).HasColumnName("HintCount").HasDefaultValue(0);
                entity.Property(e => e.IsSkipped).HasColumnName("IsSkipped").HasDefaultValue(false);
                entity.Property(e => e.IsMarkedForReview).HasColumnName("IsMarkedForReview").HasDefaultValue(false);
                entity.Property(e => e.AttemptSequence).HasColumnName("AttemptSequence").HasDefaultValue(1);
                entity.Property(e => e.PreviousAttempts).HasColumnName("PreviousAttempts").HasDefaultValue(0);
                entity.Property(e => e.LearningOutcome).HasColumnName("LearningOutcome").HasMaxLength(500);
                entity.Property(e => e.MistakeType).HasColumnName("MistakeType").HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.User).WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user_question_attempts_user_id");
                entity.HasOne(d => d.Question).WithMany()
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_user_question_attempts_question_id");
                entity.HasOne(d => d.SelectedAnswer).WithMany()
                    .HasForeignKey(d => d.SelectedAnswerId)
                    .HasConstraintName("fk_user_question_attempts_selected_answer_id");
                entity.HasOne(d => d.TestSession).WithMany()
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("fk_user_question_attempts_session_id");
                entity.HasOne(d => d.Exam).WithMany()
                    .HasForeignKey(d => d.ExamId)
                    .HasConstraintName("fk_user_question_attempts_exam_id");
                entity.HasOne(d => d.Subject).WithMany()
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_user_question_attempts_subject_id");
                entity.HasOne(d => d.Chapter).WithMany()
                    .HasForeignKey(d => d.ChapterId)
                    .HasConstraintName("fk_user_question_attempts_chapter_id");
                entity.HasOne(d => d.Lesson).WithMany()
                    .HasForeignKey(d => d.LessonId)
                    .HasConstraintName("fk_user_question_attempts_lesson_id");
            });

            // User Overall Competency
            modelBuilder.Entity<UserOverallCompetency>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_user_overall_competencies");
                entity.ToTable("user_overall_competencies", "public");
                entity.HasIndex(e => e.UserId, "ix_user_overall_competencies_user_id");
                entity.HasIndex(e => e.SubjectId, "ix_user_overall_competencies_subject_id");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.SubjectId).HasColumnName("SubjectId");
                entity.Property(e => e.OverallScore).HasColumnName("OverallScore").HasColumnType("decimal(3,2)").HasDefaultValue(0);
                entity.Property(e => e.OverallAccuracy).HasColumnName("OverallAccuracy").HasColumnType("decimal(5,2)").HasDefaultValue(0);
                entity.Property(e => e.TotalQuestionsAttempted).HasColumnName("TotalQuestionsAttempted").HasDefaultValue(0);
                entity.Property(e => e.TotalCorrectAnswers).HasColumnName("TotalCorrectAnswers").HasDefaultValue(0);
                entity.Property(e => e.TotalIncorrectAnswers).HasColumnName("TotalIncorrectAnswers").HasDefaultValue(0);
                entity.Property(e => e.TotalTimeSpent).HasColumnName("TotalTimeSpent").HasDefaultValue(0);
                entity.Property(e => e.AverageTimePerQuestion).HasColumnName("AverageTimePerQuestion").HasColumnType("decimal(8,2)").HasDefaultValue(0);
                entity.Property(e => e.CompetencyLevel).HasColumnName("CompetencyLevel").IsRequired().HasMaxLength(20).HasDefaultValue("BEGINNER");
                entity.Property(e => e.ConfidenceScore).HasColumnName("ConfidenceScore").HasColumnType("decimal(3,2)").HasDefaultValue(0);
                entity.Property(e => e.ConsistencyScore).HasColumnName("ConsistencyScore").HasColumnType("decimal(3,2)").HasDefaultValue(0);
                entity.Property(e => e.ImprovementRate).HasColumnName("ImprovementRate").HasColumnType("decimal(5,2)").HasDefaultValue(0);
                entity.Property(e => e.Strengths).HasColumnName("Strengths").HasMaxLength(2000);
                entity.Property(e => e.Weaknesses).HasColumnName("Weaknesses").HasMaxLength(2000);
                entity.Property(e => e.RecommendedFocusAreas).HasColumnName("RecommendedFocusAreas").HasMaxLength(2000);
                entity.Property(e => e.DifficultyBreakdown).HasColumnName("DifficultyBreakdown").HasMaxLength(1000);
                entity.Property(e => e.TopicBreakdown).HasColumnName("TopicBreakdown").HasMaxLength(2000);
                entity.Property(e => e.LearningVelocity).HasColumnName("LearningVelocity").HasColumnType("decimal(5,2)").HasDefaultValue(0);
                entity.Property(e => e.RetentionRate).HasColumnName("RetentionRate").HasColumnType("decimal(3,2)").HasDefaultValue(0);
                entity.Property(e => e.LastUpdated).HasColumnName("LastUpdated").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(d => d.User).WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user_overall_competencies_user_id");
                entity.HasOne(d => d.Subject).WithMany()
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_user_overall_competencies_subject_id");
            });
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
