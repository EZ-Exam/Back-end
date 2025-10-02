using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Repository;

namespace teamseven.EzExam.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        teamsevenezexamdbContext Context { get; }
        AnswerRepository AnswerRepository { get; }
        ChapterRepository ChapterRepository { get; }
        ExamQuestionRepository ExamQuestionRepository { get; }
        ExamRepository ExamRepository { get; }
        ExamTypeRepository ExamTypeRepository { get; }
        GradeRepository GradeRepository { get; }
        LessonRepository LessonRepository { get; }
        QuestionRepository QuestionRepository { get; }
        RoleRepository RoleRepository { get; }
        SemesterRepository SemesterRepository { get; }
        SolutionReportRepository SolutionReportRepository { get; }
        SolutionRepository SolutionRepository { get; }
        SubscriptionTypeRepository SubscriptionTypeRepository { get; }
        UserRepository UserRepository { get; }
        UserSubscriptionRepository UserSubscriptionRepository { get; }
        UserSocialProviderRepository UserSocialProviderRepository { get; }
        QuestionReportRepository QuestionReportRepository { get; }
        QuestionCommentRepository QuestionCommentRepository { get; }
        TextBookRepository TextBookRepository { get; }
        UserUsageTrackingRepository UserUsageTrackingRepository { get; }
        UserUsageHistoryRepository UserUsageHistoryRepository { get; }
        
        // Test System Repositories
        UserQuestionCartRepository UserQuestionCartRepository { get; }
        AITestRecommendationRepository AITestRecommendationRepository { get; }
        TestSessionRepository TestSessionRepository { get; }
        TestSessionAnswerRepository TestSessionAnswerRepository { get; }
        UserCompetencyAssessmentRepository UserCompetencyAssessmentRepository { get; }
        UserQuestionAttemptRepository UserQuestionAttemptRepository { get; }

        int SaveChangesWithTransaction();
        Task<int> SaveChangesWithTransactionAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly teamsevenezexamdbContext _context;

        private AnswerRepository _answerRepository;
        private ChapterRepository _chapterRepository;
        private ExamQuestionRepository _examQuestionRepository;
        private ExamRepository _examRepository;
        private ExamTypeRepository _examTypeRepository;
        private GradeRepository _gradeRepository;
        private LessonRepository _lessonRepository;
        private QuestionRepository _questionRepository;
        private RoleRepository _roleRepository;
        private SemesterRepository _semesterRepository;
        private SolutionReportRepository _solutionReportRepository;
        private SolutionRepository _solutionRepository;
        private SubscriptionTypeRepository _subscriptionTypeRepository;
        private UserRepository _userRepository;
        private UserSubscriptionRepository _userSubscriptionRepository;
        private UserSocialProviderRepository _userSocialProvider;
        private QuestionReportRepository _questionReportRepository;
        private QuestionCommentRepository _questionCommentRepository;
        private TextBookRepository _textBookRepository;
        private UserUsageTrackingRepository _userUsageTrackingRepository;
        private UserUsageHistoryRepository _userUsageHistoryRepository;
        
        // Test System Repositories
        private UserQuestionCartRepository _userQuestionCartRepository;
        private AITestRecommendationRepository _aiTestRecommendationRepository;
        private TestSessionRepository _testSessionRepository;
        private TestSessionAnswerRepository _testSessionAnswerRepository;
        private UserCompetencyAssessmentRepository _userCompetencyAssessmentRepository;
        private UserQuestionAttemptRepository _userQuestionAttemptRepository;

        private bool _disposed = false;

        public UnitOfWork(teamsevenezexamdbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public teamsevenezexamdbContext Context => _context;

        public AnswerRepository AnswerRepository => _answerRepository ??= new AnswerRepository(_context);
        public ChapterRepository ChapterRepository => _chapterRepository ??= new ChapterRepository(_context);
        public ExamQuestionRepository ExamQuestionRepository => _examQuestionRepository ??= new ExamQuestionRepository(_context);
        public ExamRepository ExamRepository => _examRepository ??= new ExamRepository(_context);
        public ExamTypeRepository ExamTypeRepository => _examTypeRepository ??= new ExamTypeRepository(_context);
        public GradeRepository GradeRepository => _gradeRepository ??= new GradeRepository(_context);
        public LessonRepository LessonRepository => _lessonRepository ??= new LessonRepository(_context);
        public QuestionRepository QuestionRepository => _questionRepository ??= new QuestionRepository(_context);
        public RoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context);
        public SemesterRepository SemesterRepository => _semesterRepository ??= new SemesterRepository(_context);
        public SolutionReportRepository SolutionReportRepository => _solutionReportRepository ??= new SolutionReportRepository(_context);
        public SolutionRepository SolutionRepository => _solutionRepository ??= new SolutionRepository(_context);
        public SubscriptionTypeRepository SubscriptionTypeRepository => _subscriptionTypeRepository ??= new SubscriptionTypeRepository(_context);
        public UserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public UserSubscriptionRepository UserSubscriptionRepository => _userSubscriptionRepository ??= new UserSubscriptionRepository(_context);
        public UserSocialProviderRepository UserSocialProviderRepository => _userSocialProvider ??= new UserSocialProviderRepository(_context);
        public QuestionReportRepository QuestionReportRepository => _questionReportRepository ??= new QuestionReportRepository(_context);
        public QuestionCommentRepository QuestionCommentRepository => _questionCommentRepository ??= new QuestionCommentRepository(_context);
        public TextBookRepository TextBookRepository => _textBookRepository ??= new TextBookRepository(_context);
        public UserUsageTrackingRepository UserUsageTrackingRepository => _userUsageTrackingRepository ??= new UserUsageTrackingRepository(_context);
        public UserUsageHistoryRepository UserUsageHistoryRepository => _userUsageHistoryRepository ??= new UserUsageHistoryRepository(_context);
        
        // Test System Repositories
        public UserQuestionCartRepository UserQuestionCartRepository => _userQuestionCartRepository ??= new UserQuestionCartRepository(_context);
        public AITestRecommendationRepository AITestRecommendationRepository => _aiTestRecommendationRepository ??= new AITestRecommendationRepository(_context);
        public TestSessionRepository TestSessionRepository => _testSessionRepository ??= new TestSessionRepository(_context);
        public TestSessionAnswerRepository TestSessionAnswerRepository => _testSessionAnswerRepository ??= new TestSessionAnswerRepository(_context);
        public UserCompetencyAssessmentRepository UserCompetencyAssessmentRepository => _userCompetencyAssessmentRepository ??= new UserCompetencyAssessmentRepository(_context);
        public UserQuestionAttemptRepository UserQuestionAttemptRepository => _userQuestionAttemptRepository ??= new UserQuestionAttemptRepository(_context);

        public int SaveChangesWithTransaction()
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            int result = 0;
            strategy.Execute(() =>
            {
                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    result = _context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            });
            return result;
        }

        public async Task<int> SaveChangesWithTransactionAsync()
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            int result = 0;
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    result = await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        // Log rollback error but don't throw to avoid masking original error
                        Console.WriteLine($"Rollback failed: {rollbackEx.Message}");
                    }
                    throw;
                }
            });
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _context?.Dispose();
            }

            _disposed = true;
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}