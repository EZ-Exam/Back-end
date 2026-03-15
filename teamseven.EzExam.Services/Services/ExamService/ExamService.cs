using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Services.Extensions;

namespace teamseven.EzExam.Services.Services.ExamService
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<int> CreateExamAsync(ExamRequest examRequest)
        {
            if (examRequest == null)
                throw new ArgumentNullException(nameof(examRequest));

            var exam = _mapper.Map<Exam>(examRequest);
            exam.CreatedAt = DateTime.UtcNow;
            exam.IsDeleted = false;

            await _unitOfWork.ExamRepository.AddAsync(exam);
            await _unitOfWork.SaveChangesWithTransactionAsync();
            return exam.Id;
        }

        public async Task<ExamResponse> UpdateExamAsync(UpdateExamRequest updateExamRequest)
        {
            if (updateExamRequest == null) throw new ArgumentNullException(nameof(updateExamRequest));

            var existingExam = await _unitOfWork.ExamRepository.GetByIdAsync(updateExamRequest.Id);
            if (existingExam == null)
                throw new ArgumentException($"Exam with ID {updateExamRequest.Id} not found");

            _mapper.Map(updateExamRequest, existingExam);
            existingExam.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ExamRepository.UpdateAsync(existingExam);
            await _unitOfWork.SaveChangesWithTransactionAsync();

            return _mapper.Map<ExamResponse>(existingExam);
        }

        public Task CreateExamHistoryAsync(ExamHistoryRequest examHistoryRequest)
        {
            throw new NotImplementedException();
        }
        public async Task SoftDeleteExamAsync(int examId)
        {
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId);
            if (exam == null)
                throw new NotFoundException("Exam not found");

            if (exam.IsDeleted == true)
                return;

            exam.IsDeleted = true;
            exam.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ExamRepository.Update(exam);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }
        public async Task RecoverExamAsync(int examId)
        {
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId);
            if (exam == null)
                throw new NotFoundException("Exam not found");

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
                throw new NotFoundException("Exam not found");

            var questionExists = await _unitOfWork.QuestionRepository.GetByIdAsync(examQuestionRequest.QuestionId);
            if (questionExists == null)
                throw new NotFoundException("Question not found");

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

        public Task DeleteExamHistoryAsync(ExamHistoryRequest historyRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ExamResponse>> GetAllExamAsync()
        {
            // ProjectTo: EF Core generates lean SQL (only ExamResponse columns)
            return await _unitOfWork.Context.Exams
                .AsNoTracking()
                .Where(e => e.IsDeleted == false)
                .OrderByDescending(e => e.CreatedAt)
                .ProjectTo<ExamResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<ExamResponse> GetExamAsync(int id)
        {
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(id);
            if (exam == null)
                throw new NotFoundException("Exam not found");

            return _mapper.Map<ExamResponse>(exam);
        }

        public Task<ExamHistoryResponseDto> GetExamHistoryResponseAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task RenameExamAsync(int examId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("New exam name cannot be empty");

            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId);
            if (exam == null)
                throw new NotFoundException("Exam not found");

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

            return _mapper.Map<IEnumerable<ExamQuestionResponse>>(examQuestions);
        }

        public async Task<IEnumerable<ExamQuestionDetailResponse>> GetExamQuestionsDetailAsync(int examId)
        {
            var examQuestions = await _unitOfWork.ExamQuestionRepository.GetByExamIdAsync(examId);
            if (examQuestions == null || !examQuestions.Any())
                return new List<ExamQuestionDetailResponse>();

            return _mapper.Map<IEnumerable<ExamQuestionDetailResponse>>(examQuestions.OrderBy(eq => eq.Order));
        }
        public async Task<IEnumerable<ExamResponse>> GetExamsByUserIdAsync(int userId)
        {
            return await _unitOfWork.Context.Exams
                .AsNoTracking()
                .Where(e => e.CreatedByUserId == userId && e.IsDeleted == false)
                .OrderByDescending(e => e.CreatedAt)
                .ProjectTo<ExamResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task RemoveExamQuestion(ExamQuestionRequest examQuestionRequest)
        {
            if (examQuestionRequest == null)
                throw new ArgumentNullException(nameof(examQuestionRequest));

            var examQuestion = await _unitOfWork.ExamQuestionRepository.GetByExamAndQuestionIdAsync(
                examQuestionRequest.ExamId,
                examQuestionRequest.QuestionId);

            if (examQuestion == null)
                throw new NotFoundException("Exam question not found");

            await _unitOfWork.ExamQuestionRepository.DeleteAsync(examQuestion);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task<PagedResponse<ExamResponse>> GetExamsAsync(
            int? pageNumber = null,
            int? pageSize = null,
            string? search = null,
            string? sort = null,
            int? subjectId = null,
            int? lessonId = null,
            int? examTypeId = null,
            int? createdByUserId = null,
            int isSort = 0)
        {
            var pn = pageNumber.GetValueOrDefault(1);
            var ps = pageSize.GetValueOrDefault(20);

            // Delegate filtering + paging to repository, then ProjectTo avoids Select *
            var (items, total) = await _unitOfWork.ExamRepository.GetPagedAsync(
                pn, ps, search, sort,
                subjectId, lessonId, examTypeId, createdByUserId, isSort);

            var ids = items.Select(e => e.Id).ToList();
            var list = await _unitOfWork.Context.Exams
                .AsNoTracking()
                .Where(e => ids.Contains(e.Id))
                .ProjectTo<ExamResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResponse<ExamResponse>(list, pn, ps, total);
        }

        public async Task<PagedResponse<ExamFeedResponse>> GetOptimizedExamsFeedAsync(
            int page = 1,
            int pageSize = 20,
            string? search = null,
            int? subjectId = null,
            int? lessonId = null,
            int? examTypeId = null,
            int? createdByUserId = null,
            int isSort = 0)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            var skip = (page - 1) * pageSize;

            var baseQuery = _unitOfWork.ExamRepository.GetBaseOptimizedFeedQuery()
                .Where(e => e.IsDeleted == false)
                .Where(e => e.CreatedByUser != null && e.CreatedByUser.RoleId == 3);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                baseQuery = baseQuery.Where(e => e.Name.ToLower().Contains(s) || (e.Description != null && e.Description.ToLower().Contains(s)));
            }

            if (subjectId.HasValue) baseQuery = baseQuery.Where(e => e.SubjectId == subjectId.Value);
            if (lessonId.HasValue) baseQuery = baseQuery.Where(e => e.LessonId == lessonId.Value);
            if (examTypeId.HasValue) baseQuery = baseQuery.Where(e => e.ExamTypeId == examTypeId.Value);
            if (createdByUserId.HasValue) baseQuery = baseQuery.Where(e => e.CreatedByUserId == createdByUserId.Value);

            var totalItems = await baseQuery.CountAsync();

            var query = baseQuery.OrderByDescending(e => e.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ProjectTo<ExamFeedResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            var items = await query.ToListAsync();
            return new PagedResponse<ExamFeedResponse>(items, page, pageSize, totalItems);
        }

        public async Task<PagedResponse<ExamFeedResponse>> GetOptimizedExamsFeedByUserAsync(
            int userId,
            int page = 1,
            int pageSize = 20)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            var skip = (page - 1) * pageSize;

            var baseQuery = _unitOfWork.ExamRepository.GetBaseOptimizedFeedQuery()
                .Where(e => e.IsDeleted == false)
                .Where(e => e.CreatedByUserId == userId);

            var totalItems = await baseQuery.CountAsync();

            var query = baseQuery.OrderByDescending(e => e.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ProjectTo<ExamFeedResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            var items = await query.ToListAsync();
            return new PagedResponse<ExamFeedResponse>(items, page, pageSize, totalItems);
        }

        public async Task<ExamDetailOptimizedResponse> GetOptimizedExamDetailsAsync(int examId, int currentUserId = 0)
        {
            var ctx = _unitOfWork.Context;

            var exam = await ctx.Exams.AsNoTracking()
                .Where(e => e.Id == examId && e.IsDeleted == false)
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Description,
                    e.SubjectId,
                    e.LessonId,
                    ExamTypeName = e.ExamType != null ? e.ExamType.Name : null,
                    e.CreatedByUserId,
                    CreatedByUserName = e.CreatedByUser != null ? e.CreatedByUser.Email : null,
                    e.CreatedAt,
                    e.UpdatedAt,
                    e.TimeLimit,
                    e.Duration
                }).FirstOrDefaultAsync();

            if (exam == null) throw new ArgumentException("Exam not found");

            var questionIds = await ctx.ExamQuestions.AsNoTracking()
                .Where(eq => eq.ExamId == examId)
                .OrderBy(eq => eq.Order)
                .Select(eq => eq.QuestionId)
                .ToListAsync();

            var attemptCount = await ctx.ExamHistories.CountAsync(h => h.ExamId == examId);
            var averageScore = await ctx.ExamHistories.Where(h => h.ExamId == examId).Select(h => (decimal?)h.Score).AverageAsync() ?? 0m;
            var isAttempted = currentUserId > 0 && await ctx.ExamHistories.AnyAsync(h => h.ExamId == examId && h.UserId == currentUserId);

            return new ExamDetailOptimizedResponse
            {
                Id = exam.Id,
                Name = exam.Name,
                Description = exam.Description,
                SubjectId = exam.SubjectId,
                LessonId = exam.LessonId,
                ExamTypeName = exam.ExamTypeName,
                CreatedByUserId = exam.CreatedByUserId,
                CreatedByUserName = exam.CreatedByUserName,
                CreatedAt = exam.CreatedAt,
                UpdatedAt = exam.UpdatedAt,
                TimeLimit = exam.TimeLimit,
                Duration = exam.Duration,
                TotalQuestions = questionIds.Count,
                QuestionIds = questionIds,
                AttemptCount = attemptCount,
                AverageScore = averageScore,
                IsAttemptedByCurrentUser = isAttempted
            };
        }

    }
}
