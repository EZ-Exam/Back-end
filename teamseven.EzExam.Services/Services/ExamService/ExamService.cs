using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.ExamService
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<int> CreateExamAsync(ExamRequest examRequest)
        {
            if (examRequest == null)
                throw new ArgumentNullException(nameof(examRequest));

            var exam = new Exam
            {
                Name = examRequest.Name,
                LessonId = examRequest.LessonId,
                ExamTypeId = examRequest.ExamTypeId,
                CreatedByUserId = examRequest.CreatedByUserId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.ExamRepository.AddAsync(exam);
            await _unitOfWork.SaveChangesWithTransactionAsync();
            return exam.Id;
        }

        public async Task SoftDeleteExamAsync(int examId)
        {
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId);
            if (exam == null)
                throw new ArgumentException("Exam not found");

            if (exam.IsDeleted == true)
                return; // Already deleted

            exam.IsDeleted = true;
            exam.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ExamRepository.Update(exam);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task RecoverExamAsync(int examId)
        {
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId);
            if (exam == null)
                throw new ArgumentException("Exam not found");

            if (exam.IsDeleted == false)
                return;

            exam.IsDeleted = false;
            exam.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ExamRepository.Update(exam);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task CreateExamQuestionAsync(ExamQuestionRequest examQuestionRequest)
        {
            if (examQuestionRequest == null)
                throw new ArgumentNullException(nameof(examQuestionRequest));

            var examExists = await _unitOfWork.ExamRepository.GetByIdAsync(examQuestionRequest.ExamId);
            if (examExists == null)
                throw new ArgumentException("Exam not found");

            var questionExists = await _unitOfWork.QuestionRepository.GetByIdAsync(examQuestionRequest.QuestionId);
            if (questionExists == null)
                throw new ArgumentException("Question not found");

            // Check if the question is already assigned to the exam
            if (await _unitOfWork.ExamQuestionRepository.GetByExamAndQuestionIdAsync(examQuestionRequest.ExamId, examQuestionRequest.QuestionId) != null)
                throw new InvalidOperationException("Question is already assigned to this exam");

            var examQuestion = new ExamQuestion
            {
                ExamId = examQuestionRequest.ExamId,
                QuestionId = examQuestionRequest.QuestionId,
                Order = examQuestionRequest.Order,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ExamQuestionRepository.AddAsync(examQuestion);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task<IEnumerable<ExamResponse>> GetAllExamAsync()
        {
            var exams = await _unitOfWork.ExamRepository.GetAllAsync();
            return exams.Select(e => new ExamResponse
            {
                Id = e.Id,
                Name = e.Name,
                LessonId = e.LessonId,
                ExamTypeId = e.ExamTypeId,
                CreatedByUserId = e.CreatedByUserId,
                IsDeleted = e.IsDeleted,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                CreatedByUserName = e.CreatedByUser?.Email,
                ExamTypeName = e.ExamType?.Name,
                LessonName = e.Lesson?.Name,
                QuestionCount = e.ExamQuestions?.Count ?? 0,
                HistoryCount = e.ExamHistories?.Count ?? 0
            }).ToList();
        }

        public async Task<ExamResponse> GetExamAsync(int id)
        {
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(id);
            if (exam == null)
                throw new ArgumentException("Exam not found");

            return new ExamResponse
            {
                Id = exam.Id,
                Name = exam.Name,
                LessonId = exam.LessonId,
                ExamTypeId = exam.ExamTypeId,
                CreatedByUserId = exam.CreatedByUserId,
                IsDeleted = exam.IsDeleted,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt,
                CreatedByUserName = exam.CreatedByUser?.Email,
                ExamTypeName = exam.ExamType?.Name,
                LessonName = exam.Lesson?.Name,
                QuestionCount = exam.ExamQuestions?.Count ?? 0,
                HistoryCount = exam.ExamHistories?.Count ?? 0
            };
        }

        public async Task RenameExamAsync(int examId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("New exam name cannot be empty");

            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId);
            if (exam == null)
                throw new ArgumentException("Exam not found");

            exam.Name = newName;
            exam.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ExamRepository.Update(exam);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task<IEnumerable<ExamQuestionResponse>> GetExamQuestionByIdAsync(int id)
        {
            var examQuestions = await _unitOfWork.ExamQuestionRepository.GetByExamIdAsync(id);
            if (!examQuestions.Any())
                return new List<ExamQuestionResponse>();

            return examQuestions.Select(eq => new ExamQuestionResponse
            {
                Id = eq.Id,
                ExamId = eq.ExamId,
                QuestionId = eq.QuestionId,
                Order = eq.Order,
                CreatedAt = eq.CreatedAt,
                ExamName = eq.Exam?.Name,
                QuestionContent = eq.Question?.Content
            }).ToList();
        }

        public async Task<IEnumerable<ExamResponse>> GetExamsByUserIdAsync(int userId)
        {
            var exams = await _unitOfWork.ExamRepository.GetByCreatorAsync(userId);
            return exams.Select(e => new ExamResponse
            {
                Id = e.Id,
                Name = e.Name,
                LessonId = e.LessonId,
                ExamTypeId = e.ExamTypeId,
                CreatedByUserId = e.CreatedByUserId,
                IsDeleted = e.IsDeleted,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                CreatedByUserName = e.CreatedByUser?.Email,
                ExamTypeName = e.ExamType?.Name,
                LessonName = e.Lesson?.Name,
                QuestionCount = e.ExamQuestions?.Count ?? 0,
                HistoryCount = e.ExamHistories?.Count ?? 0
            }).ToList();
        }

        public async Task RemoveExamQuestion(ExamQuestionRequest examQuestionRequest)
        {
            if (examQuestionRequest == null)
                throw new ArgumentNullException(nameof(examQuestionRequest));

            var examQuestion = await _unitOfWork.ExamQuestionRepository.GetByExamAndQuestionIdAsync(
                examQuestionRequest.ExamId,
                examQuestionRequest.QuestionId);

            if (examQuestion == null)
                throw new ArgumentException("Exam question not found");

            await _unitOfWork.ExamQuestionRepository.DeleteAsync(examQuestion);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }
    }
}