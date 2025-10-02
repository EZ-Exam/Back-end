using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace teamseven.EzExam.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentHistoryEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_exams_grades_GradeId",
                schema: "public",
                table: "exams");

            migrationBuilder.DropForeignKey(
                name: "FK_lessons_grades_GradeId",
                schema: "public",
                table: "lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_questions_grades_GradeId",
                schema: "public",
                table: "questions");

            migrationBuilder.RenameIndex(
                name: "IX_questions_GradeId",
                schema: "public",
                table: "questions",
                newName: "ix_questions_grade_id");

            migrationBuilder.RenameIndex(
                name: "IX_lessons_GradeId",
                schema: "public",
                table: "lessons",
                newName: "ix_lessons_grade_id");

            migrationBuilder.RenameIndex(
                name: "IX_exams_GradeId",
                schema: "public",
                table: "exams",
                newName: "ix_exams_grade_id");

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

            migrationBuilder.AddForeignKey(
                name: "fk_exams_grade_id",
                schema: "public",
                table: "exams",
                column: "GradeId",
                principalSchema: "public",
                principalTable: "grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_grade_id",
                schema: "public",
                table: "lessons",
                column: "GradeId",
                principalSchema: "public",
                principalTable: "grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_questions_grade_id",
                schema: "public",
                table: "questions",
                column: "GradeId",
                principalSchema: "public",
                principalTable: "grades",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_exams_grade_id",
                schema: "public",
                table: "exams");

            migrationBuilder.DropForeignKey(
                name: "fk_lessons_grade_id",
                schema: "public",
                table: "lessons");

            migrationBuilder.DropForeignKey(
                name: "fk_questions_grade_id",
                schema: "public",
                table: "questions");

            migrationBuilder.DropTable(
                name: "lessons_enhanced_questions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "student_performance_summaries",
                schema: "public");

            migrationBuilder.DropTable(
                name: "student_question_attempts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "lessons_enhanced",
                schema: "public");

            migrationBuilder.DropTable(
                name: "student_quiz_histories",
                schema: "public");

            migrationBuilder.RenameIndex(
                name: "ix_questions_grade_id",
                schema: "public",
                table: "questions",
                newName: "IX_questions_GradeId");

            migrationBuilder.RenameIndex(
                name: "ix_lessons_grade_id",
                schema: "public",
                table: "lessons",
                newName: "IX_lessons_GradeId");

            migrationBuilder.RenameIndex(
                name: "ix_exams_grade_id",
                schema: "public",
                table: "exams",
                newName: "IX_exams_GradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_exams_grades_GradeId",
                schema: "public",
                table: "exams",
                column: "GradeId",
                principalSchema: "public",
                principalTable: "grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lessons_grades_GradeId",
                schema: "public",
                table: "lessons",
                column: "GradeId",
                principalSchema: "public",
                principalTable: "grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_questions_grades_GradeId",
                schema: "public",
                table: "questions",
                column: "GradeId",
                principalSchema: "public",
                principalTable: "grades",
                principalColumn: "Id");
        }
    }
}
