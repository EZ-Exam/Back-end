using System;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Services.ChapterService;
using teamseven.EzExam.Services.Services.GradeService;
using teamseven.EzExam.Services.Services.LessonService;
using teamseven.EzExam.Services.Services.QuestionsService;
using teamseven.EzExam.Services.Services.SemesterService;
using teamseven.EzExam.Services.Services.SolutionReportService;
using teamseven.EzExam.Services.Services.SubscriptionTypeService;
using teamseven.EzExam.Services.Services.UserService;
using teamseven.EzExam.Services.Services.UserSocialProviderService;
using teamseven.EzExam.Services.Services.SolutionService;
using teamseven.EzExam.Services.Services.UserSubscriptionService;
using teamseven.EzExam.Services.Services.ExamService;
using teamseven.EzExam.Services.Services.QuestionReportService;
using teamseven.EzExam.Services.Services.TextBookService;
using teamseven.EzExam.Services.Services.UsageTrackingService;
using teamseven.EzExam.Services.Services.BalanceService;
using teamseven.EzExam.Services.Services.JwtHelperService;
using teamseven.EzExam.Services.Services.SubscriptionService;
using teamseven.EzExam.Services.Services.TestSystemServices;
using teamseven.EzExam.Services.Services.LessonEnhancedService;

namespace teamseven.EzExam.Services.Services.ServiceProvider
{
    public interface IServiceProviders
    {
        IAuthService AuthService { get; }
        ILoginService LoginService { get; }
        IUserService UserService { get; }
        IQuestionsService QuestionsService { get; }
        ISemesterService SemesterService { get; }
        IUserSocialProviderService UserSocialProviderService { get; }
        IChapterService ChapterService { get; }
        IGradeService GradeService { get; }
        ISolutionReportService SolutionReportService { get; }
        ISubscriptionTypeService SubscriptionTypeService { get; }
        IRegisterService RegisterService { get; }
        ILessonService LessonService { get; }
        ISolutionService SolutionService { get; }
        IUserSubscriptionService UserSubscriptionService { get; }
        IExamService ExamService { get; }
        IQuestionReportService QuestionReportService { get; }
        ITextBookService TextBookService { get; }
        IPayOSService PayOSService { get; }
        IUsageTrackingService UsageTrackingService { get; }
        IBalanceService BalanceService { get; }
        IJwtHelperService JwtHelperService { get; }
        ISubscriptionService SubscriptionService { get; }
        ILessonEnhancedService LessonEnhancedService { get; }


        // Test System Services
        IUserQuestionCartService UserQuestionCartService { get; }
        ITestSessionService TestSessionService { get; }
    }
}