using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.Json;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Services.MappingProfiles;
using teamseven.EzExam.Services.Services.ExamService;
using teamseven.EzExam.Services.Services.LessonService;
using teamseven.EzExam.Services.Services.QuestionsService;
using teamseven.EzExam.Services.Services.SolutionService;
using teamseven.EzExam.Services.Services.TextBookService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace teamseven.EzExam.Tests.Services
{
    /// <summary>
    /// BASELINE tests — run these BEFORE applying Redis cache and ProjectTo optimizations.
    ///
    /// Each test:
    ///  1. Seeds an InMemory DB via TestDbFactory
    ///  2. Calls the CURRENT (unoptimized) service method under a Stopwatch
    ///  3. Asserts expected data (count, field values) to guarantee correctness
    ///  4. Writes a BenchmarkResult to the shared BenchmarkReport
    ///
    /// After the optimizations are applied, run AfterTests.cs with the same
    /// assertions to verify data integrity AND compare timings.
    /// </summary>
    public class BeforeTests : IDisposable
    {
        private readonly teamsevenezexamdbContext _ctx;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // ──────────────────────────────────────────────────────────────────────────
        // Shared report file written alongside the test assembly
        // ──────────────────────────────────────────────────────────────────────────
        private static readonly string ReportPath =
            Path.Combine(AppContext.BaseDirectory, "benchmark_before.json");

        public BeforeTests()
        {
            _ctx        = TestDbFactory.CreateContext();
            _unitOfWork = new UnitOfWork(_ctx);

            // AutoMapper 15+ requires ILoggerFactory → pass NullLoggerFactory directly
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IMapper>(sp =>
            {
                var config = new MapperConfiguration(cfg =>
                    cfg.AddMaps(typeof(EzExamMappingProfile).Assembly),
                    sp.GetRequiredService<ILoggerFactory>());
                return config.CreateMapper();
            });
            var sp2 = services.BuildServiceProvider();
            _mapper = sp2.GetRequiredService<IMapper>();
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 1.  TEXT BOOK
        // ══════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Before_TextBook_GetAll_ReturnsCorrectCountAndData()
        {
            var service = new TextBookService(_unitOfWork,
                NullLogger<TextBookService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();

            // ── CURRENT path: DB hit every call, no cache ──────────────────────
            var result = (await service.GetAllTextBookAsync()).ToList();

            sw.Stop();

            // ── Expected data assertions ───────────────────────────────────────
            Assert.Equal(3, result.Count);
            Assert.Contains(result, tb => tb.Name == "Math Book G10");
            Assert.Contains(result, tb => tb.Name == "Physics Book G10");
            Assert.Contains(result, tb => tb.Name == "Math Book G11");

            WriteResult(new BenchmarkResult(
                "TextBook_GetAll", "Before",
                sw.ElapsedMilliseconds, result.Count));
        }

        [Fact]
        public async Task Before_TextBook_GetByGradeId_ReturnsFilteredResults()
        {
            var service = new TextBookService(_unitOfWork,
                NullLogger<TextBookService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetAsync(gradeId: 1);
            sw.Stop();

            Assert.Equal(2, result.Count);

            WriteResult(new BenchmarkResult(
                "TextBook_GetByGradeId", "Before",
                sw.ElapsedMilliseconds, result.Count));
        }

        [Fact]
        public async Task Before_TextBook_GetById_ReturnsCorrectItem()
        {
            var service = new TextBookService(_unitOfWork,
                NullLogger<TextBookService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetTextBookByIdAsync(1);
            sw.Stop();

            Assert.NotNull(result);
            Assert.Equal("Math Book G10", result.Name);
            Assert.Equal(1, result.GradeId);

            WriteResult(new BenchmarkResult(
                "TextBook_GetById", "Before",
                sw.ElapsedMilliseconds, 1));
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 2.  LESSONS
        // ══════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Before_Lesson_GetAll_ReturnsAllLessons()
        {
            var service = new LessonService(_unitOfWork,
                NullLogger<LessonService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = (await service.GetAllLessonAsync()).ToList();
            sw.Stop();

            Assert.Equal(20, result.Count);

            WriteResult(new BenchmarkResult(
                "Lesson_GetAll", "Before",
                sw.ElapsedMilliseconds, result.Count));
        }

        [Fact]
        public async Task Before_Lesson_GetByChapterId_ReturnsFilteredLessons()
        {
            var service = new LessonService(_unitOfWork,
                NullLogger<LessonService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = (await service.GetLessonsByChapterIdAsync(1)).ToList();
            sw.Stop();

            // Chapter 1 → odd lessons (1,3,5,...,19) = 10 items
            Assert.Equal(10, result.Count);
            Assert.All(result, l => Assert.NotEmpty(l.Name));

            WriteResult(new BenchmarkResult(
                "Lesson_GetByChapterId", "Before",
                sw.ElapsedMilliseconds, result.Count));
        }

        [Fact]
        public async Task Before_Lesson_GetPaged_ReturnsCorrectPage()
        {
            var service = new LessonService(_unitOfWork,
                NullLogger<LessonService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetLessonsAsync(pageNumber: 1, pageSize: 5);
            sw.Stop();

            Assert.Equal(5,  result.Items.Count);
            Assert.Equal(20, result.TotalItems);

            WriteResult(new BenchmarkResult(
                "Lesson_GetPaged", "Before",
                sw.ElapsedMilliseconds, result.Items.Count));
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 3.  QUESTIONS  — exposes n+1 loop in current code
        // ══════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Before_Questions_GetPaged_ReturnsCorrectDataWithN1()
        {
            var service = new QuestionsService(_unitOfWork,
                NullLogger<QuestionsService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            // CURRENT: loads questions then fires GetByQuestionIdAsync per item
            var result = await service.GetQuestionsAsync(pageNumber: 1, pageSize: 10);
            sw.Stop();

            Assert.Equal(10,  result.Items.Count);
            Assert.Equal(100, result.TotalItems);

            // Spot-check fields
            var first = result.Items.First();
            Assert.NotEmpty(first.Content);
            Assert.NotNull(first.Type);

            WriteResult(new BenchmarkResult(
                "Questions_GetPaged_10", "Before",
                sw.ElapsedMilliseconds, result.Items.Count,
                $"TotalItems={result.TotalItems}"));
        }

        [Fact]
        public async Task Before_Questions_GetBySubjectId_ReturnsCorrectData()
        {
            var service = new QuestionsService(_unitOfWork,
                NullLogger<QuestionsService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetQuestionBySubjectIdAsync(1);
            sw.Stop();

            // SubjectId=1 → 50 questions (odd indices)
            Assert.Equal(50, result.Count);
            Assert.All(result, q => Assert.NotEmpty(q.Content));

            WriteResult(new BenchmarkResult(
                "Questions_GetBySubjectId", "Before",
                sw.ElapsedMilliseconds, result.Count));
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 4.  EXAMS
        // ══════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Before_Exam_GetAll_ReturnsAllExams()
        {
            var service = new ExamService(_unitOfWork, _mapper);

            var sw = Stopwatch.StartNew();
            var result = (await service.GetAllExamAsync()).ToList();
            sw.Stop();

            Assert.Equal(30, result.Count);

            WriteResult(new BenchmarkResult(
                "Exam_GetAll", "Before",
                sw.ElapsedMilliseconds, result.Count));
        }

        [Fact]
        public async Task Before_Exam_GetPaged_ReturnsCorrectPage()
        {
            var service = new ExamService(_unitOfWork, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetExamsAsync(pageNumber: 1, pageSize: 10);
            sw.Stop();

            Assert.Equal(10, result.Items.Count);
            Assert.Equal(30, result.TotalItems);

            WriteResult(new BenchmarkResult(
                "Exam_GetPaged_10", "Before",
                sw.ElapsedMilliseconds, result.Items.Count));
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 5.  SOLUTIONS
        // ══════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task Before_Solution_GetById_ReturnsCorrectData()
        {
            var service = new SolutionService(_unitOfWork,
                NullLogger<SolutionService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetSolutionByIdAsync(1);
            sw.Stop();

            Assert.NotNull(result);
            Assert.Equal(1, result.QuestionId);
            Assert.Contains("Solution 1", result.Content);

            WriteResult(new BenchmarkResult(
                "Solution_GetById", "Before",
                sw.ElapsedMilliseconds, 1));
        }

        [Fact]
        public async Task Before_Solution_GetAll_ReturnsAllSolutions()
        {
            var service = new SolutionService(_unitOfWork,
                NullLogger<SolutionService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = (await service.GetAllSolutionsAsync()).ToList();
            sw.Stop();

            Assert.Equal(20, result.Count);

            WriteResult(new BenchmarkResult(
                "Solution_GetAll", "Before",
                sw.ElapsedMilliseconds, result.Count));
        }

        // ══════════════════════════════════════════════════════════════════════════
        // Helpers
        // ══════════════════════════════════════════════════════════════════════════

        private static void WriteResult(BenchmarkResult result)
        {
            // Thread-safe append to JSON file
            lock (_fileLock)
            {
                var list = ReadExisting();
                // Remove old entry with same key so we only keep latest per test
                list.RemoveAll(r => r.TestName == result.TestName && r.Phase == result.Phase);
                list.Add(result);
                File.WriteAllText(ReportPath,
                    JsonSerializer.Serialize(list,
                        new JsonSerializerOptions { WriteIndented = true }));
            }

            Console.WriteLine(
                $"[{result.Phase}] {result.TestName}: " +
                $"{result.ElapsedMs}ms | count={result.ItemCount} {result.ExtraNote}");
        }

        private static List<BenchmarkResult> ReadExisting()
        {
            if (!File.Exists(ReportPath)) return new();
            try
            {
                var json = File.ReadAllText(ReportPath);
                return JsonSerializer.Deserialize<List<BenchmarkResult>>(json) ?? new();
            }
            catch { return new(); }
        }

        private static readonly object _fileLock = new();

        public void Dispose() => _ctx.Dispose();
    }

    // ════════════════════════════════════════════════════════════════════════════
    // Shared report model
    // ════════════════════════════════════════════════════════════════════════════

    public record BenchmarkResult(
        string TestName,
        string Phase,           // "Before" or "After"
        long   ElapsedMs,
        int    ItemCount,
        string ExtraNote = "");
}
