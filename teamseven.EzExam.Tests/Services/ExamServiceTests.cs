using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.MappingProfiles;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Services.ExamService;
using teamseven.EzExam.Services.Extensions;
using Xunit;

namespace teamseven.EzExam.Tests.Services
{
    public class ExamServiceTests : IDisposable
    {
        private readonly teamsevenezexamdbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ExamService _examService;

        public ExamServiceTests()
        {
            // Set up In-Memory DB
            var options = new DbContextOptionsBuilder<teamsevenezexamdbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new teamsevenezexamdbContext(options);
            _unitOfWork = new UnitOfWork(_context);

            // Set up AutoMapper
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IUnitOfWork>(_unitOfWork);
            services.AddAutoMapper(cfg => cfg.AddProfile<EzExamMappingProfile>());
            var serviceProvider = services.BuildServiceProvider();
            _mapper = serviceProvider.GetRequiredService<IMapper>();

            _examService = new ExamService(_unitOfWork, _mapper);
            
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Exams.AddRange(new List<Exam>
            {
                new Exam
                {
                    Id = 1,
                    Name = "Test Exam on Supabase Mock Data",
                    Description = "This is a mock description",
                    SubjectId = 1,
                    LessonId = 1,
                    ExamTypeId = 1,
                    CreatedByUserId = 1,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Duration = 60,
                    TotalQuestions = 10,
                    CreatedByUser = new User { Id = 1, Email = "testuser@example.com" },
                    ExamType = new ExamType { Id = 1, Name = "Midterm" },
                    Lesson = new Lesson { Id = 1, Name = "Lesson 1" }
                },
                new Exam { Id = 2, Name = "Exam 2", Duration = 30, IsDeleted = false },
                new Exam { Id = 3, Name = "Exam 3", Duration = 45, IsDeleted = false }
            });
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _unitOfWork.Dispose();
        }

        [Fact]
        public async Task GetExamAsync_WithValidId_ReturnsExamResponse()
        {
            // Act
            var result = await _examService.GetExamAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Exam on Supabase Mock Data", result.Name);
            Assert.Equal(60, result.Duration);
        }

        [Fact]
        public async Task GetExamAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Act & Assert
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _examService.GetExamAsync(999));
            Assert.Equal("Exam not found", ex.Message);
        }

        [Fact]
        public async Task CreateExamAsync_GivenValidRequest_ReturnsNewExamId()
        {
            // Arrange
            var request = new ExamRequest
            {
                Name = "New Supabase Mock Exam",
                Description = "New Description",
                SubjectId = 2,
                LessonId = 2,
                ExamTypeId = 2,
                Duration = 90,
                CreatedByUserId = 1
            };

            // Act
            var newId = await _examService.CreateExamAsync(request);
            
            // Assert
            var createdExam = await _context.Exams.FindAsync(newId);
            Assert.NotNull(createdExam);
            Assert.Equal(request.Name, createdExam.Name);
            Assert.Equal(request.Duration, createdExam.Duration);
            Assert.False(createdExam.IsDeleted);
        }

        [Fact]
        public async Task GetAllExamAsync_ReturnsMappedExams()
        {
            // Act
            var result = await _examService.GetAllExamAsync();

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count); // Seeding added 3 exams
            Assert.Contains(resultList, e => e.Name == "Exam 2");
            Assert.Contains(resultList, e => e.Name == "Test Exam on Supabase Mock Data");
        }
    }
}
