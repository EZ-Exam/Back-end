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
                name: "exam_types",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grades", x => x.Id);
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
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.Id);
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
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    IsActive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    EmailVerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.Id);
                    table.ForeignKey(
                        name: "fk_users_role_id",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "roles",
                        principalColumn: "Id");
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
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SemesterId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    SubscriptionPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DurationInDays = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_types", x => x.Id);
                    table.ForeignKey(
                        name: "fk_subscription_types_updated_by",
                        column: x => x.UpdatedBy,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "lessons",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ChapterId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    PaymentStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PaymentGatewayTransactionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "fk_user_subscriptions_subscription_type_id",
                        column: x => x.SubscriptionTypeId,
                        principalSchema: "public",
                        principalTable: "subscription_types",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_user_subscriptions_user_id",
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
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LessonId = table.Column<int>(type: "integer", nullable: false),
                    ExamTypeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exams", x => x.Id);
                    table.ForeignKey(
                        name: "fk_exams_created_by_user_id",
                        column: x => x.CreatedByUserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_exams_exam_type_id",
                        column: x => x.ExamTypeId,
                        principalSchema: "public",
                        principalTable: "exam_types",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_exams_lesson_id",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons",
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
                    Content = table.Column<string>(type: "text", nullable: false),
                    QuestionSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DifficultyLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LessonId = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.Id);
                    table.ForeignKey(
                        name: "fk_questions_created_by_user_id",
                        column: x => x.CreatedByUserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_questions_lesson_id",
                        column: x => x.LessonId,
                        principalSchema: "public",
                        principalTable: "lessons",
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
                    ActionByUserId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exam_histories", x => x.Id);
                    table.ForeignKey(
                        name: "fk_exam_histories_action_by_user_id",
                        column: x => x.ActionByUserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_exam_histories_exam_id",
                        column: x => x.ExamId,
                        principalSchema: "public",
                        principalTable: "exams",
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "solutions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestionId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "integer", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_solutions", x => x.Id);
                    table.ForeignKey(
                        name: "fk_solutions_created_by_user_id",
                        column: x => x.CreatedByUserId,
                        principalSchema: "public",
                        principalTable: "users",
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
                name: "solution_reports",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SolutionId = table.Column<int>(type: "integer", nullable: false),
                    ReportedByUserId = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
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
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "fk_solution_reports_solution_id",
                        column: x => x.SolutionId,
                        principalSchema: "public",
                        principalTable: "solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "solutions_links",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SolutionId = table.Column<int>(type: "integer", nullable: false),
                    Link = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    GeneratedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_solutions_links", x => x.Id);
                    table.ForeignKey(
                        name: "fk_solutions_links_solution_id",
                        column: x => x.SolutionId,
                        principalSchema: "public",
                        principalTable: "solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chapters_semester_id",
                schema: "public",
                table: "chapters",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "ix_exam_histories_action_by_user_id",
                schema: "public",
                table: "exam_histories",
                column: "ActionByUserId");

            migrationBuilder.CreateIndex(
                name: "ix_exam_histories_exam_id",
                schema: "public",
                table: "exam_histories",
                column: "ExamId");

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
                name: "ix_exams_is_deleted",
                schema: "public",
                table: "exams",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "ix_exams_lesson_id",
                schema: "public",
                table: "exams",
                column: "LessonId");

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
                name: "ix_questions_created_by_user_id",
                schema: "public",
                table: "questions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "ix_questions_lesson_id",
                schema: "public",
                table: "questions",
                column: "LessonId");

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
                name: "ix_solutions_is_approved",
                schema: "public",
                table: "solutions",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "ix_solutions_is_deleted",
                schema: "public",
                table: "solutions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "ix_solutions_question_id",
                schema: "public",
                table: "solutions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "ix_solutions_links_solution_id",
                schema: "public",
                table: "solutions_links",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_types_subscription_code",
                schema: "public",
                table: "subscription_types",
                column: "SubscriptionCode");

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
                name: "ix_user_subscriptions_is_active",
                schema: "public",
                table: "user_subscriptions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "ix_user_subscriptions_payment_status",
                schema: "public",
                table: "user_subscriptions",
                column: "PaymentStatus");

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
                name: "ix_users_email",
                schema: "public",
                table: "users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "ix_users_is_active",
                schema: "public",
                table: "users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "ix_users_last_login_at",
                schema: "public",
                table: "users",
                column: "LastLoginAt");

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
                name: "exam_histories",
                schema: "public");

            migrationBuilder.DropTable(
                name: "exam_questions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "solution_reports",
                schema: "public");

            migrationBuilder.DropTable(
                name: "solutions_links",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_social_providers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_subscriptions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "exams",
                schema: "public");

            migrationBuilder.DropTable(
                name: "solutions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "subscription_types",
                schema: "public");

            migrationBuilder.DropTable(
                name: "exam_types",
                schema: "public");

            migrationBuilder.DropTable(
                name: "questions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public");

            migrationBuilder.DropTable(
                name: "lessons",
                schema: "public");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "chapters",
                schema: "public");

            migrationBuilder.DropTable(
                name: "semesters",
                schema: "public");

            migrationBuilder.DropTable(
                name: "grades",
                schema: "public");
        }
    }
}
