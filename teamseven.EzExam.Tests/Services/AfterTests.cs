using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using System.Text.Json;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Services.MappingProfiles;
using teamseven.EzExam.Services.Services;
using teamseven.EzExam.Services.Services.ExamService;
using teamseven.EzExam.Services.Services.LessonService;
using teamseven.EzExam.Services.Services.QuestionsService;
using teamseven.EzExam.Services.Services.SolutionService;
using teamseven.EzExam.Services.Services.TextBookService;

namespace teamseven.EzExam.Tests.Services
{
    /// <summary>
    /// AFTER tests — run after applying Redis cache + ProjectTo optimizations.
    ///
    /// Each test makes the SAME assertions as BeforeTests to verify data integrity,
    /// then writes its timing to benchmark_after.json for comparison.
    ///
    /// NOTE: The InMemory EF provider used here effectively measures the overhead
    /// of the data pipeline (mapping, DTO assembly) rather than DB I/O, which
    /// is exactly the difference we optimized (ProjectTo vs in-memory Map loops).
    /// </summary>
    public class AfterTests : IDisposable
    {
        private readonly teamsevenezexamdbContext _ctx;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private static readonly string ReportPath =
            Path.Combine(AppContext.BaseDirectory, "benchmark_after.json");

        public AfterTests()
        {
            _ctx        = TestDbFactory.CreateContext();
            _unitOfWork = new UnitOfWork(_ctx);

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
        // 1.  TEXT BOOK — ProjectTo + Redis cache (NullCache in tests)
        // ══════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task After_TextBook_GetAll_ReturnsCorrectCountAndData()
        {
            var service = new TextBookService(_unitOfWork,
                NullLogger<TextBookService>.Instance, _mapper);  // NullCache by default

            var sw = Stopwatch.StartNew();
            var result = (await service.GetAllTextBookAsync()).ToList();
            sw.Stop();

            // ── Same assertions as BeforeTests ─────────────────────────────────
            Assert.Equal(3, result.Count);
            Assert.Contains(result, tb => tb.Name == "Math Book G10");
            Assert.Contains(result, tb => tb.Name == "Physics Book G10");
            Assert.Contains(result, tb => tb.Name == "Math Book G11");

            WriteResult(new BenchmarkResult(
                "TextBook_GetAll", "After",
                sw.ElapsedMilliseconds, result.Count));
        }

        [Fact]
        public async Task After_TextBook_GetByGradeId_ReturnsFilteredResults()
        {
            var service = new TextBookService(_unitOfWork,
                NullLogger<TextBookService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetAsync(gradeId: 1);
            sw.Stop();

            Assert.Equal(2, result.Count);

            WriteResult(new BenchmarkResult(
                "TextBook_GetByGradeId", "After",
                sw.ElapsedMilliseconds, result.Count));
        }

        [Fact]
        public async Task After_TextBook_GetById_ReturnsCorrectItem()
        {
            var service = new TextBookService(_unitOfWork,
                NullLogger<TextBookService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetTextBookByIdAsync(1);
            sw.Stop();

            Assert.NotNull(result);
            Assert.Equal("Math Book G10", result.Name);

            WriteResult(new BenchmarkResult(
                "TextBook_GetById", "After",
                sw.ElapsedMilliseconds, 1));
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 2.  LESSONS — ProjectTo + Redis cache
        // ══════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task After_Lesson_GetAll_ReturnsAllLessons()
        {
            var service = new LessonService(_unitOfWork,
                NullLogger<LessonService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = (await service.GetAllLessonAsync()).ToList();
            sw.Stop();

            Assert.Equal(20, result.Count);

            WriteResult(new BenchmarkResult(
                "Lesson_GetAll", "After",
                sw.ElapsedMilliseconds, result.Count));
        }

        [Fact]
        public async Task After_Lesson_GetByChapterId_ReturnsFilteredLessons()
        {
            var service = new LessonService(_unitOfWork,
                NullLogger<LessonService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = (await service.GetLessonsByChapterIdAsync(1)).ToList();
            sw.Stop();

            Assert.Equal(10, result.Count);
            Assert.All(result, l => Assert.NotEmpty(l.Name));

            WriteResult(new BenchmarkResult(
                "Lesson_GetByChapterId", "After",
                sw.ElapsedMilliseconds, result.Count));
        }

        [Fact]
        public async Task After_Lesson_GetPaged_ReturnsCorrectPage()
        {
            var service = new LessonService(_unitOfWork,
                NullLogger<LessonService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetLessonsAsync(pageNumber: 1, pageSize: 5);
            sw.Stop();

            Assert.Equal(5,  result.Items.Count);
            Assert.Equal(20, result.TotalItems);

            WriteResult(new BenchmarkResult(
                "Lesson_GetPaged", "After",
                sw.ElapsedMilliseconds, result.Items.Count));
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 3.  QUESTIONS
        // ══════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task After_Questions_GetPaged_ReturnsCorrectData()
        {
            var service = new QuestionsService(_unitOfWork,
                NullLogger<QuestionsService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetQuestionsAsync(pageNumber: 1, pageSize: 10);
            sw.Stop();

            Assert.Equal(10,  result.Items.Count);
            Assert.Equal(100, result.TotalItems);

            var first = result.Items.First();
            Assert.NotEmpty(first.Content);
            Assert.NotNull(first.Type);

            WriteResult(new BenchmarkResult(
                "Questions_GetPaged_10", "After",
                sw.ElapsedMilliseconds, result.Items.Count,
                $"TotalItems={result.TotalItems}"));
        }

        [Fact]
        public async Task After_Questions_GetBySubjectId_ReturnsCorrectData()
        {
            var service = new QuestionsService(_unitOfWork,
                NullLogger<QuestionsService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetQuestionBySubjectIdAsync(1);
            sw.Stop();

            Assert.Equal(50, result.Count);
            Assert.All(result, q => Assert.NotEmpty(q.Content));

            WriteResult(new BenchmarkResult(
                "Questions_GetBySubjectId", "After",
                sw.ElapsedMilliseconds, result.Count));
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 4.  EXAMS
        // ══════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task After_Exam_GetAll_ReturnsAllExams()
        {
            var service = new ExamService(_unitOfWork, _mapper);

            var sw = Stopwatch.StartNew();
            var result = (await service.GetAllExamAsync()).ToList();
            sw.Stop();

            Assert.Equal(30, result.Count);

            WriteResult(new BenchmarkResult(
                "Exam_GetAll", "After",
                sw.ElapsedMilliseconds, result.Count));
        }

        [Fact]
        public async Task After_Exam_GetPaged_ReturnsCorrectPage()
        {
            var service = new ExamService(_unitOfWork, _mapper);

            var sw = Stopwatch.StartNew();
            var result = await service.GetExamsAsync(pageNumber: 1, pageSize: 10);
            sw.Stop();

            Assert.Equal(10, result.Items.Count);
            Assert.Equal(30, result.TotalItems);

            WriteResult(new BenchmarkResult(
                "Exam_GetPaged_10", "After",
                sw.ElapsedMilliseconds, result.Items.Count));
        }

        // ══════════════════════════════════════════════════════════════════════════
        // 5.  SOLUTIONS — Lazy Redis cache
        // ══════════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task After_Solution_GetById_ReturnsCorrectData()
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
                "Solution_GetById", "After",
                sw.ElapsedMilliseconds, 1));
        }

        [Fact]
        public async Task After_Solution_GetAll_ReturnsAllSolutions()
        {
            var service = new SolutionService(_unitOfWork,
                NullLogger<SolutionService>.Instance, _mapper);

            var sw = Stopwatch.StartNew();
            var result = (await service.GetAllSolutionsAsync()).ToList();
            sw.Stop();

            Assert.Equal(20, result.Count);

            WriteResult(new BenchmarkResult(
                "Solution_GetAll", "After",
                sw.ElapsedMilliseconds, result.Count));
        }

        // ══════════════════════════════════════════════════════════════════════════
        // Helpers
        // ══════════════════════════════════════════════════════════════════════════

        private static void WriteResult(BenchmarkResult result)
        {
            lock (_fileLock)
            {
                var list = ReadExisting();
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
                return JsonSerializer.Deserialize<List<BenchmarkResult>>(
                    File.ReadAllText(ReportPath)) ?? new();
            }
            catch { return new(); }
        }

        private static readonly object _fileLock = new();
        public void Dispose() => _ctx.Dispose();
    }
}
