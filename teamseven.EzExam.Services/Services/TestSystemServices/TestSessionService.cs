using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.Services.TestSystemServices
{
    public interface ITestSessionService
    {
        Task<TestSessionResponse?> StartSessionAsync(StartTestSessionRequest request);
        Task<bool> EndSessionAsync(int sessionId, EndTestSessionRequest request);
        Task<bool> PauseSessionAsync(int sessionId);
        Task<bool> ResumeSessionAsync(int sessionId);
        Task<TestSessionResponse?> GetSessionAsync(int sessionId);
        Task<List<TestSessionResponse>> GetUserSessionsAsync(int userId);
        Task<List<TestSessionResponse>> GetActiveSessionsAsync(int userId);
        Task<List<TestSessionResponse>> GetCompletedSessionsAsync(int userId);
        Task<TestSessionResponse?> GetActiveSessionByExamAsync(int userId, int examId);
        Task<bool> SubmitAnswerAsync(SubmitAnswerRequest request);
        Task<List<TestSessionAnswerResponse>> GetSessionAnswersAsync(int sessionId);
        Task<decimal> GetAverageScoreAsync(int userId);
        Task<int> GetTotalSessionsCountAsync(int userId);
    }

    public class TestSessionService : ITestSessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TestSessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<TestSessionResponse?> StartSessionAsync(StartTestSessionRequest request)
        {
            try
            {
                // Check if user already has an active session for this exam
                var existingSession = await _unitOfWork.TestSessionRepository.GetActiveSessionByExamAsync(request.UserId, request.ExamId);
                if (existingSession != null)
                {
                    return null; // User already has an active session
                }

                var session = new TestSession
                {
                    UserId = request.UserId,
                    ExamId = request.ExamId,
                    SessionStatus = "IN_PROGRESS",
                    StartedAt = DateTime.UtcNow,
                    PassingScore = request.PassingScore,
                    DeviceInfo = request.DeviceInfo,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.TestSessionRepository.CreateAsync(session);
                await _unitOfWork.SaveChangesWithTransactionAsync();

                return MapToResponse(session);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> EndSessionAsync(int sessionId, EndTestSessionRequest request)
        {
            try
            {
                var session = await _unitOfWork.TestSessionRepository.GetByIdAsync(sessionId);
                if (session == null || session.SessionStatus != "IN_PROGRESS")
                {
                    return false;
                }

                session.SessionStatus = "COMPLETED";
                session.CompletedAt = DateTime.UtcNow;
                session.TimeSpent = request.TimeSpent;
                session.TotalScore = request.TotalScore;
                session.CorrectAnswers = request.CorrectAnswers;
                session.TotalQuestions = request.TotalQuestions;
                session.IsPassed = request.TotalScore >= (session.PassingScore ?? 60);
                session.SessionData = request.SessionData;
                session.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.TestSessionRepository.UpdateAsync(session);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PauseSessionAsync(int sessionId)
        {
            try
            {
                var session = await _unitOfWork.TestSessionRepository.GetByIdAsync(sessionId);
                if (session == null || session.SessionStatus != "IN_PROGRESS")
                {
                    return false;
                }

                session.SessionStatus = "PAUSED";
                session.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.TestSessionRepository.UpdateAsync(session);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResumeSessionAsync(int sessionId)
        {
            try
            {
                var session = await _unitOfWork.TestSessionRepository.GetByIdAsync(sessionId);
                if (session == null || session.SessionStatus != "PAUSED")
                {
                    return false;
                }

                session.SessionStatus = "IN_PROGRESS";
                session.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.TestSessionRepository.UpdateAsync(session);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TestSessionResponse?> GetSessionAsync(int sessionId)
        {
            var session = await _unitOfWork.TestSessionRepository.GetByIdAsync(sessionId);
            return session != null ? MapToResponse(session) : null;
        }

        public async Task<List<TestSessionResponse>> GetUserSessionsAsync(int userId)
        {
            var sessions = await _unitOfWork.TestSessionRepository.GetByUserIdAsync(userId);
            return sessions.Select(MapToResponse).ToList();
        }

        public async Task<List<TestSessionResponse>> GetActiveSessionsAsync(int userId)
        {
            var sessions = await _unitOfWork.TestSessionRepository.GetActiveSessionsAsync(userId);
            return sessions.Select(MapToResponse).ToList();
        }

        public async Task<List<TestSessionResponse>> GetCompletedSessionsAsync(int userId)
        {
            var sessions = await _unitOfWork.TestSessionRepository.GetCompletedSessionsAsync(userId);
            return sessions.Select(MapToResponse).ToList();
        }

        public async Task<TestSessionResponse?> GetActiveSessionByExamAsync(int userId, int examId)
        {
            var session = await _unitOfWork.TestSessionRepository.GetActiveSessionByExamAsync(userId, examId);
            return session != null ? MapToResponse(session) : null;
        }

        public async Task<bool> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            try
            {
                var answer = new TestSessionAnswer
                {
                    TestSessionId = request.TestSessionId,
                    QuestionId = request.QuestionId,
                    SelectedAnswerId = request.SelectedAnswerId,
                    UserAnswer = request.UserAnswer,
                    IsCorrect = request.IsCorrect,
                    TimeSpent = request.TimeSpent,
                    AnsweredAt = DateTime.UtcNow,
                    IsMarkedForReview = request.IsMarkedForReview,
                    ConfidenceLevel = request.ConfidenceLevel,
                    AnswerSequence = request.AnswerSequence,
                    IsChanged = request.IsChanged,
                    ChangeCount = request.ChangeCount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.TestSessionAnswerRepository.CreateAsync(answer);
                await _unitOfWork.SaveChangesWithTransactionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<TestSessionAnswerResponse>> GetSessionAnswersAsync(int sessionId)
        {
            var answers = await _unitOfWork.TestSessionAnswerRepository.GetBySessionIdAsync(sessionId);
            return answers.Select(MapAnswerToResponse).ToList();
        }

        public async Task<decimal> GetAverageScoreAsync(int userId)
        {
            return await _unitOfWork.TestSessionRepository.GetAverageScoreByUserIdAsync(userId);
        }

        public async Task<int> GetTotalSessionsCountAsync(int userId)
        {
            return await _unitOfWork.TestSessionRepository.GetTotalSessionsCountAsync(userId);
        }

        private static TestSessionResponse MapToResponse(TestSession session)
        {
            return new TestSessionResponse
            {
                Id = session.Id,
                UserId = session.UserId,
                ExamId = session.ExamId,
                SessionStatus = session.SessionStatus,
                StartedAt = (DateTime)session.StartedAt,
                CompletedAt = session.CompletedAt,
                TimeSpent = session.TimeSpent,
                TotalScore = session.TotalScore,
                CorrectAnswers = session.CorrectAnswers,
                TotalQuestions = session.TotalQuestions,
                IsPassed = session.IsPassed,
                PassingScore = session.PassingScore,
                SessionData = session.SessionData,
                DeviceInfo = session.DeviceInfo,
                IsCheatingDetected = session.IsCheatingDetected,
                CheatingDetails = session.CheatingDetails,
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt
            };
        }

        private static TestSessionAnswerResponse MapAnswerToResponse(TestSessionAnswer answer)
        {
            return new TestSessionAnswerResponse
            {
                Id = answer.Id,
                TestSessionId = answer.TestSessionId,
                QuestionId = answer.QuestionId,
                SelectedAnswerId = answer.SelectedAnswerId,
                UserAnswer = answer.UserAnswer,
                IsCorrect = (bool)answer.IsCorrect,
                TimeSpent = answer.TimeSpent,
                AnsweredAt = (DateTime)answer.AnsweredAt,
                IsMarkedForReview = answer.IsMarkedForReview,
                ConfidenceLevel = answer.ConfidenceLevel,
                AnswerSequence = answer.AnswerSequence,
                IsChanged = answer.IsChanged,
                ChangeCount = answer.ChangeCount,
                CreatedAt = answer.CreatedAt,
                UpdatedAt = answer.UpdatedAt
            };
        }
    }
}
