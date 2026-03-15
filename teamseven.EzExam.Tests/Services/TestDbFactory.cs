using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;

namespace teamseven.EzExam.Tests.Services
{
    /// <summary>
    /// Factory that creates a fresh InMemory DbContext with realistic seeded data
    /// so both "Before" and "After" benchmark tests operate on the same dataset.
    /// </summary>
    public static class TestDbFactory
    {
        private static int _dbCounter = 0;

        /// <summary>Creates a new unique InMemory DbContext with seeded data.</summary>
        public static teamsevenezexamdbContext CreateContext()
        {
            var name = $"EzExamTestDb_{System.Threading.Interlocked.Increment(ref _dbCounter)}";
            var opts = new DbContextOptionsBuilder<teamsevenezexamdbContext>()
                .UseInMemoryDatabase(name)
                .Options;

            var ctx = new teamsevenezexamdbContext(opts);
            SeedData(ctx);
            return ctx;
        }

        private static void SeedData(teamsevenezexamdbContext ctx)
        {
            // ── Roles ──────────────────────────────────────────────────────────────
            var role = new Role { Id = 1, RoleName = "Admin" };
            ctx.Roles.Add(role);

            // ── Grades ─────────────────────────────────────────────────────────────
            var grade1 = new Grade { Id = 1, Name = "Grade 10" };
            var grade2 = new Grade { Id = 2, Name = "Grade 11" };
            ctx.Grades.AddRange(grade1, grade2);

            // ── Subjects ───────────────────────────────────────────────────────────
            var subject1 = new Subject { Id = 1, Name = "Math", Code = "MATH" };
            var subject2 = new Subject { Id = 2, Name = "Physics", Code = "PHYS" };
            ctx.Subjects.AddRange(subject1, subject2);

            // ── Semesters ──────────────────────────────────────────────────────────
            var sem1 = new Semester { Id = 1, Name = "HK1", GradeId = 1 };
            ctx.Semesters.Add(sem1);

            // ── Difficulty Levels ──────────────────────────────────────────────────
            var easy = new DifficultyLevel { Id = 1, Name = "Easy", Code = "EASY" };
            var hard = new DifficultyLevel { Id = 2, Name = "Hard", Code = "HARD" };
            ctx.DifficultyLevels.AddRange(easy, hard);

            // ── TextBooks ─────────────────────────────────────────────────────────
            var tb1 = new TextBook { Id = 1, Name = "Math Book G10", GradeId = 1, SubjectId = 1 };
            var tb2 = new TextBook { Id = 2, Name = "Physics Book G10", GradeId = 1, SubjectId = 2 };
            var tb3 = new TextBook { Id = 3, Name = "Math Book G11", GradeId = 2, SubjectId = 1 };
            ctx.TextBooks.AddRange(tb1, tb2, tb3);

            // ── Chapters ──────────────────────────────────────────────────────────
            var ch1 = new Chapter { Id = 1, Name = "Algebra", SubjectId = 1, SemesterId = 1 };
            var ch2 = new Chapter { Id = 2, Name = "Geometry", SubjectId = 1, SemesterId = 1 };
            ctx.Chapters.AddRange(ch1, ch2);

            // ── Lessons ───────────────────────────────────────────────────────────
            var lessons = Enumerable.Range(1, 20).Select(i => new Lesson
            {
                Id = i,
                Name = $"Lesson {i}",
                ChapterId = (i % 2 == 0) ? ch2.Id : ch1.Id,
                GradeId = 1
            }).ToList();
            ctx.Lessons.AddRange(lessons);

            // ── Users ─────────────────────────────────────────────────────────────
            var user1 = new User { Id = 1, Email = "teacher@test.com", RoleId = 1 };
            ctx.Users.Add(user1);

            // ── Questions (100 seeded) with related data ──────────────────────────
            var questions = Enumerable.Range(1, 100).Select(i => new Question
            {
                Id         = i,
                Content    = $"Question content {i}: What is the value of x?",
                SubjectId  = (i % 2 == 0) ? 2 : 1,
                GradeId    = 1,
                ChapterId  = (i % 2 == 0) ? ch2.Id : ch1.Id,
                LessonId   = (i % 20) + 1,
                TextbookId = tb1.Id,
                DifficultyLevelId = (i % 2 == 0) ? easy.Id : hard.Id,
                CreatedByUserId   = user1.Id,
                QuestionType      = "MULTIPLE_CHOICE",
                IsActive          = true,
                CorrectAnswer     = "A",
                Options           = "[\"A. 1\",\"B. 2\",\"C. 3\",\"D. 4\"]",
                Explanation       = $"Explanation for question {i}"
            }).ToList();
            ctx.Questions.AddRange(questions);

            // ── Answers for first 10 questions ────────────────────────────────────
            var answers = Enumerable.Range(1, 10).SelectMany(qId =>
                new[] { "A", "B", "C", "D" }.Select((key, idx) => new Answer
                {
                    Id         = (qId - 1) * 4 + idx + 1,
                    QuestionId = qId,
                    AnswerKey  = key,
                    Content    = $"Option {key} for Q{qId}",
                    IsCorrect  = key == "A"
                })
            ).ToList();
            ctx.Answers.AddRange(answers);

            // ── ExamType ──────────────────────────────────────────────────────────
            var examType = new ExamType { Id = 1, Name = "Quiz", TypeCode = "QUIZ" };
            ctx.ExamTypes.Add(examType);

            // ── Exams (30 seeded) ─────────────────────────────────────────────────
            var exams = Enumerable.Range(1, 30).Select(i => new Exam
            {
                Id             = i,
                Name           = $"Exam {i}",
                Description    = $"Exam description {i}",
                SubjectId      = (i % 2 == 0) ? 2 : 1,
                LessonId       = (i % 20) + 1,
                ExamTypeId     = examType.Id,
                CreatedByUserId = user1.Id,
                IsDeleted      = false,
                TotalQuestions = 10,
                CreatedAt      = DateTime.UtcNow.AddDays(-i)
            }).ToList();
            ctx.Exams.AddRange(exams);

            // ── Solutions (20 seeded) ──────────────────────────────────────────────
            var solutions = Enumerable.Range(1, 20).Select(i => new Solution
            {
                Id              = i,
                QuestionId      = i,
                Content         = $"Solution {i}: The answer is A because...",
                Explanation     = $"Detailed explanation for solution {i}",
                CreatedByUserId = user1.Id,
                IsApproved      = true,
                CreatedAt       = DateTime.UtcNow
            }).ToList();
            ctx.Solutions.AddRange(solutions);

            ctx.SaveChanges();
        }
    }
}
