using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace teamseven.EzExam.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "difficulty_levels",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_difficulty_levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "exam_types",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TypeCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DefaultDuration = table.Column<int>(type: "integer", nullable: true),
                    MaxQuestions = table.Column<int>(type: "integer", nullable: true),
                    MinQuestions = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exam_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "grades",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lessons_enhanced",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    PdfUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lessons_enhanced", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subjects",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "semesters",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    GradeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_semesters", x => x.Id);
                    table.ForeignKey(
                        name: "fk_semesters_grade_id",
                        column: x => x.GradeId,
                        principalSchema: "public",
                        principalTable: "grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "textbooks",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GradeId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_textbooks", x => x.Id);
                    table.ForeignKey(
                        name: "fk_textbooks_grade_id",
                        column: x => x.GradeId,
                        principalSchema: "public",
                        principalTable: "grades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    EmailVerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPremium = table.Column<bool>(type: "boolean", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    Balance = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.Id);
                    table.ForeignKey(
                        name: "fk_users_role_id",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_updated_by",
                        column: x => x.UpdatedBy,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "chapters",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chapters", x => x.Id);
                    table.ForeignKey(
                        name: "fk_chapters_semester_id",
                        column: x => x.SemesterId,
                        principalSchema: "public",
                        principalTable: "semesters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_chapters_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_performance_summaries",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: true),
                    GradeId = table.Column<int>(type: "integer", nullable: true),
                    TotalQuizzesCompleted = table.Column<int>(type: "integer", nullable: false),
                    RecentQuizzesCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
                    RecentQuizIds = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AverageScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    AverageTimePerQuiz = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    AverageTimePerQuestion = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    OverallAccuracy = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ImprovementTrend = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TrendPercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    StrongTopics = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WeakTopics = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DifficultyProfile = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RecommendedDifficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LearningVelocity = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ConsistencyScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    PredictedNextScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    ConfidenceLevel = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    TimeManagementScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    LastQuizDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastAnalysisDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_performance_summaries", x => x.Id);
                    table.ForeignKey(
                        name: "fk_student_performance_summaries_grade_id",
                        column: x => x.GradeId,
                        principalSchema: "public",
                        principalTable: "grades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_student_performance_summaries_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_student_performance_summaries_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_types",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubscriptionCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SubscriptionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SubscriptionPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    MaxSolutionViews = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    MaxAIRequests = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsAIEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Features = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_types", x => x.Id);
                    table.ForeignKey(
                        name: "fk_subscription_types_updated_by",
                        column: x => x.UpdatedBy,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "user_overall_competencies",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    OverallScore = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    OverallAccuracy = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    TotalQuestionsAttempted = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TotalCorrectAnswers = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TotalIncorrectAnswers = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TotalTimeSpent = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AverageTimePerQuestion = table.Column<decimal>(type: "numeric(8,2)", nullable: false, defaultValue: 0m),
                    CompetencyLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "BEGINNER"),
                    ConfidenceScore = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    ConsistencyScore = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    ImprovementRate = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    Strengths = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Weaknesses = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RecommendedFocusAreas = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DifficultyBreakdown = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TopicBreakdown = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    LearningVelocity = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    RetentionRate = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_overall_competencies", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_overall_competencies_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_overall_competencies_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_social_providers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProviderId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProfileUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_social_providers", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_social_providers_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_usage_history",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UsageType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ResourceId = table.Column<int>(type: "integer", nullable: true),
                    ResourceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_usage_history", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_usage_history_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lessons",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ChapterId = table.Column<int>(type: "integer", nullable: false),
                    GradeId = table.Column<int>(type: "integer", nullable: true),
                    Document = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    DocumentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lessons", x => x.Id);
                    table.ForeignKey(
                        name: "fk_lessons_chapter_id",
                        column: x => x.ChapterId,
                        principalSchema: "public",
                        principalTable: "chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_lessons_grade_id",
                        column: x => x.GradeId,
                        principalSchema: "public",
                        principalTable: "grades",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "user_subscriptions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionTypeId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    PaymentStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentGatewayTransactionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_subscriptions_subscription_type_id",
                        column: x => x.SubscriptionTypeId,
                        principalSchema: "public",
                        principalTable: "subscription_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_subscriptions_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_usage_tracking",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionTypeId = table.Column<int>(type: "integer", nullable: false),
                    UsageType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ResetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_usage_tracking", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_usage_tracking_subscription_type_id",
                        column: x => x.SubscriptionTypeId,
                        principalSchema: "public",
                        principalTable: "subscription_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_usage_tracking_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ai_test_recommendations",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TestName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    ChapterId = table.Column<int>(type: "integer", nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    RecommendedDuration = table.Column<int>(type: "integer", nullable: true),
                    RecommendedQuestionCount = table.Column<int>(type: "integer", nullable: true),
                    DifficultyDistribution = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TopicDistribution = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    BasedOnHistory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    BasedOnWeakAreas = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    BasedOnProgress = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ConfidenceScore = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    IsAccepted = table.Column<bool>(type: "boolean", nullable: true),
                    IsGenerated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    GeneratedExamId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_test_recommendations", x => x.Id);
                    table.ForeignKey(
                        name: "fk_ai_test_recommendations_chapter_id",
                        column: x => x.ChapterId,
                        principalSchema: "public",
                        principalTable: "chapters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_ai_test_recommendations_lesson_id",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_ai_test_recommendations_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ai_test_recommendations_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    QuestionSource = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DifficultyLevelId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    GradeId = table.Column<int>(type: "integer", nullable: true),
                    ChapterId = table.Column<int>(type: "integer", nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    TextbookId = table.Column<int>(type: "integer", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    QuestionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "MULTIPLE_CHOICE"),
                    Image = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    TemplateQuestionId = table.Column<int>(type: "integer", nullable: true),
                    IsCloned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AverageRating = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Formula = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CorrectAnswer = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Explanation = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Options = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.Id);
                    table.ForeignKey(
                        name: "fk_questions_chapter_id",
                        column: x => x.ChapterId,
                        principalSchema: "public",
                        principalTable: "chapters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_questions_created_by_user_id",
                        column: x => x.CreatedByUserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_questions_difficulty_level_id",
                        column: x => x.DifficultyLevelId,
                        principalSchema: "public",
                        principalTable: "difficulty_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_questions_grade_id",
                        column: x => x.GradeId,
                        principalSchema: "public",
                        principalTable: "grades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_questions_lesson_id",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_questions_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_questions_template_question_id",
                        column: x => x.TemplateQuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_questions_textbook_id",
                        column: x => x.TextbookId,
                        principalSchema: "public",
                        principalTable: "textbooks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "user_competency_assessments",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    ChapterId = table.Column<int>(type: "integer", nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    DifficultyLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Topic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CorrectAnswers = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IncorrectAnswers = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AccuracyRate = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    CompetencyScore = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    ConfidenceLevel = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    AverageTimePerQuestion = table.Column<decimal>(type: "numeric(8,2)", nullable: false, defaultValue: 0m),
                    TotalTimeSpent = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastAttemptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FirstAttemptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ImprovementTrend = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    WeaknessAreas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StrengthAreas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RecommendedActions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_competency_assessments", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_competency_assessments_chapter_id",
                        column: x => x.ChapterId,
                        principalSchema: "public",
                        principalTable: "chapters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_competency_assessments_lesson_id",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_competency_assessments_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_competency_assessments_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exams",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    GradeId = table.Column<int>(type: "integer", nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    ExamTypeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    TimeLimit = table.Column<int>(type: "integer", nullable: true),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TotalMarks = table.Column<int>(type: "integer", nullable: true),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    TestConfiguration = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DifficultyDistribution = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TopicDistribution = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsAutoGenerated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    GenerationSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AITestRecommendationId = table.Column<int>(type: "integer", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ChapterId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exams_chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalSchema: "public",
                        principalTable: "chapters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_exams_ai_test_recommendation_id",
                        column: x => x.AITestRecommendationId,
                        principalSchema: "public",
                        principalTable: "ai_test_recommendations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_exams_created_by_user_id",
                        column: x => x.CreatedByUserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_exams_exam_type_id",
                        column: x => x.ExamTypeId,
                        principalSchema: "public",
                        principalTable: "exam_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_exams_grade_id",
                        column: x => x.GradeId,
                        principalSchema: "public",
                        principalTable: "grades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_exams_lesson_id",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_exams_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "answers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    AnswerKey = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Explanation = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_answers", x => x.Id);
                    table.ForeignKey(
                        name: "fk_answers_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chatbots",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    UserMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    BotResponse = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    MessageType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "TEXT"),
                    Context = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    QuestionId = table.Column<int>(type: "integer", nullable: true),
                    SubjectId = table.Column<int>(type: "integer", nullable: true),
                    IsHelpful = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chatbots", x => x.Id);
                    table.ForeignKey(
                        name: "fk_chatbots_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_chatbots_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_chatbots_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lessons_enhanced_questions",
                schema: "public",
                columns: table => new
                {
                    LessonId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lessons_enhanced_questions", x => new { x.LessonId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_lessons_enhanced_questions_lessons_enhanced_LessonId",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons_enhanced",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lessons_enhanced_questions_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "question_comments",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ParentCommentId = table.Column<int>(type: "integer", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsHelpful = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_comments", x => x.Id);
                    table.ForeignKey(
                        name: "fk_question_comments_parent_comment_id",
                        column: x => x.ParentCommentId,
                        principalSchema: "public",
                        principalTable: "question_comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_question_comments_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_question_comments_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_reports",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    ReportedByUserId = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ReportDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsHandled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_reports", x => x.Id);
                    table.ForeignKey(
                        name: "fk_question_reports_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_question_reports_reported_by_user_id",
                        column: x => x.ReportedByUserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solutions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Explanation = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    PythonScript = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    Mp4Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: true),
                    IsMp4Generated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsMp4Reused = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    OriginalSolutionId = table.Column<int>(type: "integer", nullable: true),
                    VideoData = table.Column<byte[]>(type: "bytea", nullable: true),
                    VideoContentType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_solutions", x => x.Id);
                    table.ForeignKey(
                        name: "fk_solutions_created_by_user_id",
                        column: x => x.CreatedByUserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_solutions_original_solution_id",
                        column: x => x.OriginalSolutionId,
                        principalSchema: "public",
                        principalTable: "solutions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_solutions_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_question_carts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsSelected = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UserNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DifficultyPreference = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SubjectId = table.Column<int>(type: "integer", nullable: true),
                    ChapterId = table.Column<int>(type: "integer", nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_question_carts", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_question_carts_chapter_id",
                        column: x => x.ChapterId,
                        principalSchema: "public",
                        principalTable: "chapters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_question_carts_lesson_id",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_question_carts_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_question_carts_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_question_carts_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exam_histories",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExamId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    CorrectCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IncorrectCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    UnansweredCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    TimeTaken = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Answers = table.Column<string>(type: "text", maxLength: 10000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exam_histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_exam_histories_exams_ExamId",
                        column: x => x.ExamId,
                        principalSchema: "public",
                        principalTable: "exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exam_histories_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exam_questions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExamId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exam_questions", x => x.Id);
                    table.ForeignKey(
                        name: "fk_exam_questions_exam_id",
                        column: x => x.ExamId,
                        principalSchema: "public",
                        principalTable: "exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_exam_questions_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_sessions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExamId = table.Column<int>(type: "integer", nullable: false),
                    SessionStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "NOT_STARTED"),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TimeSpent = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TotalScore = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    CorrectAnswers = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsPassed = table.Column<bool>(type: "boolean", nullable: true),
                    PassingScore = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    SessionData = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    DeviceInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsCheatingDetected = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CheatingDetails = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_sessions", x => x.Id);
                    table.ForeignKey(
                        name: "fk_test_sessions_exam_id",
                        column: x => x.ExamId,
                        principalSchema: "public",
                        principalTable: "exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_test_sessions_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_learning_histories",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    ChapterId = table.Column<int>(type: "integer", nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    QuestionId = table.Column<int>(type: "integer", nullable: true),
                    ExamId = table.Column<int>(type: "integer", nullable: true),
                    ActivityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TimeSpent = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Score = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: true),
                    DifficultyLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TopicTags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WeakAreas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Strengths = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LearningProgress = table.Column<decimal>(type: "numeric(3,2)", nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_learning_histories", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_learning_histories_chapter_id",
                        column: x => x.ChapterId,
                        principalSchema: "public",
                        principalTable: "chapters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_learning_histories_exam_id",
                        column: x => x.ExamId,
                        principalSchema: "public",
                        principalTable: "exams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_learning_histories_lesson_id",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_learning_histories_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_learning_histories_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_learning_histories_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solution_reports",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SolutionId = table.Column<int>(type: "integer", nullable: false),
                    ReportedByUserId = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ReportDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_solution_reports", x => x.Id);
                    table.ForeignKey(
                        name: "fk_solution_reports_reported_by_user_id",
                        column: x => x.ReportedByUserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_solution_reports_solution_id",
                        column: x => x.SolutionId,
                        principalSchema: "public",
                        principalTable: "solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_quiz_histories",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExamId = table.Column<int>(type: "integer", nullable: false),
                    TestSessionId = table.Column<int>(type: "integer", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TimeSpent = table.Column<int>(type: "integer", nullable: false),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "integer", nullable: false),
                    IncorrectAnswers = table.Column<int>(type: "integer", nullable: false),
                    SkippedQuestions = table.Column<int>(type: "integer", nullable: false),
                    TotalScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    PassingScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    IsPassed = table.Column<bool>(type: "boolean", nullable: true),
                    QuizStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AverageTimePerQuestion = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    DifficultyBreakdown = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TopicPerformance = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    WeakAreas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StrongAreas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ImprovementAreas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PerformanceRating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ComparedToPrevious = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    DeviceInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SessionData = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    IsCheatingDetected = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CheatingDetails = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_quiz_histories", x => x.Id);
                    table.ForeignKey(
                        name: "fk_student_quiz_histories_exam_id",
                        column: x => x.ExamId,
                        principalSchema: "public",
                        principalTable: "exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_student_quiz_histories_test_session_id",
                        column: x => x.TestSessionId,
                        principalSchema: "public",
                        principalTable: "test_sessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_student_quiz_histories_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "test_session_answers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestSessionId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    SelectedAnswerId = table.Column<int>(type: "integer", nullable: true),
                    UserAnswer = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: true),
                    TimeSpent = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsMarkedForReview = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ConfidenceLevel = table.Column<int>(type: "integer", nullable: true),
                    AnswerSequence = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsChanged = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ChangeCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_session_answers", x => x.Id);
                    table.ForeignKey(
                        name: "fk_test_session_answers_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_test_session_answers_selected_answer_id",
                        column: x => x.SelectedAnswerId,
                        principalSchema: "public",
                        principalTable: "answers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_test_session_answers_test_session_id",
                        column: x => x.TestSessionId,
                        principalSchema: "public",
                        principalTable: "test_sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_question_attempts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    SelectedAnswerId = table.Column<int>(type: "integer", nullable: true),
                    UserAnswer = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    TimeSpent = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ConfidenceLevel = table.Column<int>(type: "integer", nullable: true),
                    AttemptType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "PRACTICE"),
                    SessionId = table.Column<int>(type: "integer", nullable: true),
                    ExamId = table.Column<int>(type: "integer", nullable: true),
                    DifficultyLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Topic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SubjectId = table.Column<int>(type: "integer", nullable: true),
                    ChapterId = table.Column<int>(type: "integer", nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    IsHintUsed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HintCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsSkipped = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsMarkedForReview = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AttemptSequence = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    PreviousAttempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LearningOutcome = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MistakeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_question_attempts", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_question_attempts_chapter_id",
                        column: x => x.ChapterId,
                        principalSchema: "public",
                        principalTable: "chapters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_question_attempts_exam_id",
                        column: x => x.ExamId,
                        principalSchema: "public",
                        principalTable: "exams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_question_attempts_lesson_id",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_question_attempts_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_question_attempts_selected_answer_id",
                        column: x => x.SelectedAnswerId,
                        principalSchema: "public",
                        principalTable: "answers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_question_attempts_session_id",
                        column: x => x.SessionId,
                        principalSchema: "public",
                        principalTable: "test_sessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_question_attempts_subject_id",
                        column: x => x.SubjectId,
                        principalSchema: "public",
                        principalTable: "subjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_question_attempts_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_question_attempts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentQuizHistoryId = table.Column<int>(type: "integer", nullable: false),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SelectedAnswerId = table.Column<int>(type: "integer", nullable: true),
                    UserAnswer = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TimeSpent = table.Column<int>(type: "integer", nullable: false),
                    Topic = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ChapterId = table.Column<int>(type: "integer", nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: true),
                    QuestionOrder = table.Column<int>(type: "integer", nullable: false),
                    ConfidenceLevel = table.Column<int>(type: "integer", nullable: true),
                    IsMarkedForReview = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsSkipped = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AnswerChangeCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_student_question_attempts", x => x.Id);
                    table.ForeignKey(
                        name: "fk_student_question_attempts_chapter_id",
                        column: x => x.ChapterId,
                        principalSchema: "public",
                        principalTable: "chapters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_student_question_attempts_lesson_id",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_student_question_attempts_question_id",
                        column: x => x.QuestionId,
                        principalSchema: "public",
                        principalTable: "questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_student_question_attempts_quiz_history_id",
                        column: x => x.StudentQuizHistoryId,
                        principalSchema: "public",
                        principalTable: "student_quiz_histories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_student_question_attempts_selected_answer_id",
                        column: x => x.SelectedAnswerId,
                        principalSchema: "public",
                        principalTable: "answers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_student_question_attempts_user_id",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ai_test_recommendations_ChapterId",
                schema: "public",
                table: "ai_test_recommendations",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_ai_test_recommendations_LessonId",
                schema: "public",
                table: "ai_test_recommendations",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "ix_ai_test_recommendations_subject_id",
                schema: "public",
                table: "ai_test_recommendations",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "ix_ai_test_recommendations_user_id",
                schema: "public",
                table: "ai_test_recommendations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_answers_is_correct",
                schema: "public",
                table: "answers",
                column: "IsCorrect");

            migrationBuilder.CreateIndex(
                name: "ix_answers_question_id",
                schema: "public",
                table: "answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "ix_chapters_semester_id",
                schema: "public",
                table: "chapters",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "ix_chapters_subject_id",
                schema: "public",
                table: "chapters",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "ix_chatbots_created_at",
                schema: "public",
                table: "chatbots",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "ix_chatbots_question_id",
                schema: "public",
                table: "chatbots",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "ix_chatbots_subject_id",
                schema: "public",
                table: "chatbots",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "ix_chatbots_user_id",
                schema: "public",
                table: "chatbots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "uq_difficulty_levels_code",
                schema: "public",
                table: "difficulty_levels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamHistories_ExamId",
                schema: "public",
                table: "exam_histories",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamHistories_Score",
                schema: "public",
                table: "exam_histories",
                column: "Score");

            migrationBuilder.CreateIndex(
                name: "IX_ExamHistories_SubmittedAt",
                schema: "public",
                table: "exam_histories",
                column: "SubmittedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ExamHistories_UserId",
                schema: "public",
                table: "exam_histories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_exam_questions_exam_id",
                schema: "public",
                table: "exam_questions",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "ix_exam_questions_question_id",
                schema: "public",
                table: "exam_questions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "uq_exam_types_name",
                schema: "public",
                table: "exam_types",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_exams_AITestRecommendationId",
                schema: "public",
                table: "exams",
                column: "AITestRecommendationId");

            migrationBuilder.CreateIndex(
                name: "IX_exams_ChapterId",
                schema: "public",
                table: "exams",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "ix_exams_created_by_user_id",
                schema: "public",
                table: "exams",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "ix_exams_exam_type_id",
                schema: "public",
                table: "exams",
                column: "ExamTypeId");

            migrationBuilder.CreateIndex(
                name: "ix_exams_grade_id",
                schema: "public",
                table: "exams",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "ix_exams_is_active",
                schema: "public",
                table: "exams",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "ix_exams_is_deleted",
                schema: "public",
                table: "exams",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "ix_exams_is_public",
                schema: "public",
                table: "exams",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "ix_exams_lesson_id",
                schema: "public",
                table: "exams",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "ix_exams_subject_id",
                schema: "public",
                table: "exams",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "uq_grades_name",
                schema: "public",
                table: "grades",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lessons_chapter_id",
                schema: "public",
                table: "lessons",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "ix_lessons_grade_id",
                schema: "public",
                table: "lessons",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "idx_le_subject",
                schema: "public",
                table: "lessons_enhanced",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "idx_leq_lesson",
                schema: "public",
                table: "lessons_enhanced_questions",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "idx_leq_question",
                schema: "public",
                table: "lessons_enhanced_questions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "ix_question_comments_is_approved",
                schema: "public",
                table: "question_comments",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "ix_question_comments_is_deleted",
                schema: "public",
                table: "question_comments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "ix_question_comments_parent_comment_id",
                schema: "public",
                table: "question_comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "ix_question_comments_question_id",
                schema: "public",
                table: "question_comments",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "ix_question_comments_user_id",
                schema: "public",
                table: "question_comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_question_reports_question_id",
                schema: "public",
                table: "question_reports",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "ix_question_reports_reported_by_user_id",
                schema: "public",
                table: "question_reports",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "ix_questions_chapter_id",
                schema: "public",
                table: "questions",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "ix_questions_created_by_user_id",
                schema: "public",
                table: "questions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "ix_questions_difficulty_level_id",
                schema: "public",
                table: "questions",
                column: "DifficultyLevelId");

            migrationBuilder.CreateIndex(
                name: "ix_questions_grade_id",
                schema: "public",
                table: "questions",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "ix_questions_is_active",
                schema: "public",
                table: "questions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "ix_questions_lesson_id",
                schema: "public",
                table: "questions",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "ix_questions_question_type",
                schema: "public",
                table: "questions",
                column: "QuestionType");

            migrationBuilder.CreateIndex(
                name: "ix_questions_subject_id",
                schema: "public",
                table: "questions",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "ix_questions_template_question_id",
                schema: "public",
                table: "questions",
                column: "TemplateQuestionId");

            migrationBuilder.CreateIndex(
                name: "ix_questions_textbook_id",
                schema: "public",
                table: "questions",
                column: "TextbookId");

            migrationBuilder.CreateIndex(
                name: "uq_roles_role_name",
                schema: "public",
                table: "roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_semesters_grade_id",
                schema: "public",
                table: "semesters",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "ix_solution_reports_reported_by_user_id",
                schema: "public",
                table: "solution_reports",
                column: "ReportedByUserId");

            migrationBuilder.CreateIndex(
                name: "ix_solution_reports_solution_id",
                schema: "public",
                table: "solution_reports",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "ix_solutions_created_by_user_id",
                schema: "public",
                table: "solutions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "ix_solutions_original_solution_id",
                schema: "public",
                table: "solutions",
                column: "OriginalSolutionId");

            migrationBuilder.CreateIndex(
                name: "ix_solutions_question_id",
                schema: "public",
                table: "solutions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_student_performance_summaries_GradeId",
                schema: "public",
                table: "student_performance_summaries",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "ix_student_performance_summaries_subject_id",
                schema: "public",
                table: "student_performance_summaries",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "ix_student_performance_summaries_user_id",
                schema: "public",
                table: "student_performance_summaries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_student_performance_summaries_user_subject",
                schema: "public",
                table: "student_performance_summaries",
                columns: new[] { "UserId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_question_attempts_ChapterId",
                schema: "public",
                table: "student_question_attempts",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_student_question_attempts_LessonId",
                schema: "public",
                table: "student_question_attempts",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "ix_student_question_attempts_question_id",
                schema: "public",
                table: "student_question_attempts",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "ix_student_question_attempts_quiz_history_id",
                schema: "public",
                table: "student_question_attempts",
                column: "StudentQuizHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_student_question_attempts_SelectedAnswerId",
                schema: "public",
                table: "student_question_attempts",
                column: "SelectedAnswerId");

            migrationBuilder.CreateIndex(
                name: "ix_student_question_attempts_user_id",
                schema: "public",
                table: "student_question_attempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_student_question_attempts_user_question",
                schema: "public",
                table: "student_question_attempts",
                columns: new[] { "UserId", "QuestionId" });

            migrationBuilder.CreateIndex(
                name: "ix_student_quiz_histories_exam_id",
                schema: "public",
                table: "student_quiz_histories",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "ix_student_quiz_histories_test_session_id",
                schema: "public",
                table: "student_quiz_histories",
                column: "TestSessionId");

            migrationBuilder.CreateIndex(
                name: "ix_student_quiz_histories_user_completed",
                schema: "public",
                table: "student_quiz_histories",
                columns: new[] { "UserId", "CompletedAt" });

            migrationBuilder.CreateIndex(
                name: "ix_student_quiz_histories_user_id",
                schema: "public",
                table: "student_quiz_histories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "uq_subjects_code",
                schema: "public",
                table: "subjects",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscription_types_updated_by",
                schema: "public",
                table: "subscription_types",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "uq_subscription_types_subscription_code",
                schema: "public",
                table: "subscription_types",
                column: "SubscriptionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_test_session_answers_QuestionId",
                schema: "public",
                table: "test_session_answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_test_session_answers_SelectedAnswerId",
                schema: "public",
                table: "test_session_answers",
                column: "SelectedAnswerId");

            migrationBuilder.CreateIndex(
                name: "ix_test_session_answers_test_session_id",
                schema: "public",
                table: "test_session_answers",
                column: "TestSessionId");

            migrationBuilder.CreateIndex(
                name: "ix_test_sessions_exam_id",
                schema: "public",
                table: "test_sessions",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "ix_test_sessions_user_id",
                schema: "public",
                table: "test_sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_textbooks_grade_id",
                schema: "public",
                table: "textbooks",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "ix_textbooks_subject_id",
                schema: "public",
                table: "textbooks",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "uq_textbooks_name_grade",
                schema: "public",
                table: "textbooks",
                columns: new[] { "Name", "GradeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_competency_assessments_ChapterId",
                schema: "public",
                table: "user_competency_assessments",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_user_competency_assessments_LessonId",
                schema: "public",
                table: "user_competency_assessments",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "ix_user_competency_assessments_subject_id",
                schema: "public",
                table: "user_competency_assessments",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "ix_user_competency_assessments_topic",
                schema: "public",
                table: "user_competency_assessments",
                column: "Topic");

            migrationBuilder.CreateIndex(
                name: "ix_user_competency_assessments_user_id",
                schema: "public",
                table: "user_competency_assessments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_learning_histories_ChapterId",
                schema: "public",
                table: "user_learning_histories",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_user_learning_histories_ExamId",
                schema: "public",
                table: "user_learning_histories",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_user_learning_histories_LessonId",
                schema: "public",
                table: "user_learning_histories",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_user_learning_histories_QuestionId",
                schema: "public",
                table: "user_learning_histories",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "ix_user_learning_histories_subject_id",
                schema: "public",
                table: "user_learning_histories",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "ix_user_learning_histories_user_id",
                schema: "public",
                table: "user_learning_histories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_user_overall_competencies_subject_id",
                schema: "public",
                table: "user_overall_competencies",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "ix_user_overall_competencies_user_id",
                schema: "public",
                table: "user_overall_competencies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_question_attempts_ChapterId",
                schema: "public",
                table: "user_question_attempts",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_user_question_attempts_ExamId",
                schema: "public",
                table: "user_question_attempts",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_user_question_attempts_LessonId",
                schema: "public",
                table: "user_question_attempts",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "ix_user_question_attempts_question_id",
                schema: "public",
                table: "user_question_attempts",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_user_question_attempts_SelectedAnswerId",
                schema: "public",
                table: "user_question_attempts",
                column: "SelectedAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_user_question_attempts_SessionId",
                schema: "public",
                table: "user_question_attempts",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "ix_user_question_attempts_subject_id",
                schema: "public",
                table: "user_question_attempts",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "ix_user_question_attempts_user_id",
                schema: "public",
                table: "user_question_attempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_question_carts_ChapterId",
                schema: "public",
                table: "user_question_carts",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_user_question_carts_LessonId",
                schema: "public",
                table: "user_question_carts",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "ix_user_question_carts_question_id",
                schema: "public",
                table: "user_question_carts",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_user_question_carts_SubjectId",
                schema: "public",
                table: "user_question_carts",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "ix_user_question_carts_user_id",
                schema: "public",
                table: "user_question_carts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_user_social_providers_user_id",
                schema: "public",
                table: "user_social_providers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "uq_user_social_providers_provider",
                schema: "public",
                table: "user_social_providers",
                columns: new[] { "ProviderName", "ProviderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_subscriptions_subscription_type_id",
                schema: "public",
                table: "user_subscriptions",
                column: "SubscriptionTypeId");

            migrationBuilder.CreateIndex(
                name: "ix_user_subscriptions_user_id",
                schema: "public",
                table: "user_subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_user_usage_history_created_at",
                schema: "public",
                table: "user_usage_history",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "ix_user_usage_history_usage_type",
                schema: "public",
                table: "user_usage_history",
                column: "UsageType");

            migrationBuilder.CreateIndex(
                name: "ix_user_usage_history_user_id",
                schema: "public",
                table: "user_usage_history",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_user_usage_tracking_reset_date",
                schema: "public",
                table: "user_usage_tracking",
                column: "ResetDate");

            migrationBuilder.CreateIndex(
                name: "ix_user_usage_tracking_subscription_type_id",
                schema: "public",
                table: "user_usage_tracking",
                column: "SubscriptionTypeId");

            migrationBuilder.CreateIndex(
                name: "ix_user_usage_tracking_usage_type",
                schema: "public",
                table: "user_usage_tracking",
                column: "UsageType");

            migrationBuilder.CreateIndex(
                name: "ix_user_usage_tracking_user_id",
                schema: "public",
                table: "user_usage_tracking",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_users_role_id",
                schema: "public",
                table: "users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "ix_users_updated_by",
                schema: "public",
                table: "users",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "uq_users_email",
                schema: "public",
                table: "users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chatbots",
                schema: "public");

            migrationBuilder.DropTable(
                name: "exam_histories",
                schema: "public");

            migrationBuilder.DropTable(
                name: "exam_questions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "lessons_enhanced_questions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "question_comments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "question_reports",
                schema: "public");

            migrationBuilder.DropTable(
                name: "solution_reports",
                schema: "public");

            migrationBuilder.DropTable(
                name: "student_performance_summaries",
                schema: "public");

            migrationBuilder.DropTable(
                name: "student_question_attempts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "test_session_answers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_competency_assessments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_learning_histories",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_overall_competencies",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_question_attempts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_question_carts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_social_providers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_subscriptions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_usage_history",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_usage_tracking",
                schema: "public");

            migrationBuilder.DropTable(
                name: "lessons_enhanced",
                schema: "public");

            migrationBuilder.DropTable(
                name: "solutions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "student_quiz_histories",
                schema: "public");

            migrationBuilder.DropTable(
                name: "answers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "subscription_types",
                schema: "public");

            migrationBuilder.DropTable(
                name: "test_sessions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "questions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "exams",
                schema: "public");

            migrationBuilder.DropTable(
                name: "difficulty_levels",
                schema: "public");

            migrationBuilder.DropTable(
                name: "textbooks",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ai_test_recommendations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "exam_types",
                schema: "public");

            migrationBuilder.DropTable(
                name: "lessons",
                schema: "public");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public");

            migrationBuilder.DropTable(
                name: "chapters",
                schema: "public");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "semesters",
                schema: "public");

            migrationBuilder.DropTable(
                name: "subjects",
                schema: "public");

            migrationBuilder.DropTable(
                name: "grades",
                schema: "public");
        }
    }
}
