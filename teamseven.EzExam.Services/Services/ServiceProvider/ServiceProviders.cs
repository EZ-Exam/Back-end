using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Services.ChapterService;
using teamseven.EzExam.Services.Services.ExamService;
using teamseven.EzExam.Services.Services.GradeService;
using teamseven.EzExam.Services.Services.LessonService;
using teamseven.EzExam.Services.Services.QuestionReportService;
using teamseven.EzExam.Services.Services.QuestionsService;
using teamseven.EzExam.Services.Services.SemesterService;
using teamseven.EzExam.Services.Services.SolutionReportService;
using teamseven.EzExam.Services.Services.SolutionService;
using teamseven.EzExam.Services.Services.SubscriptionTypeService;
using teamseven.EzExam.Services.Services.TextBookService;
using teamseven.EzExam.Services.Services.UserService;
using teamseven.EzExam.Services.Services.UserSocialProviderService;
using teamseven.EzExam.Services.Services.UserSubscriptionService;

namespace teamseven.EzExam.Services.Services.ServiceProvider
{
    public class ServiceProviders : IServiceProviders
    {
        private readonly IAuthService _authService;
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;
        private readonly IQuestionsService _questionsService;
        private readonly ISemesterService _semesterService;
        private readonly IUserSocialProviderService _userSocialProviderService;
        private readonly IChapterService _chapterService;
        private readonly IGradeService _gradeService;
        private readonly ISolutionReportService _solutionReportService;
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly IRegisterService _registerService;
        private readonly ILessonService _lessonService;
        private readonly ISolutionService _solutionService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IExamService _examService;
        private readonly IQuestionReportService _questionReportService;
        private readonly ITextBookService _textBookService;
        private readonly IPayOSService _payOSService;
        public ServiceProviders(
            IAuthService authService,
            ILoginService loginService,
            IUserService userService,
            IQuestionsService questionsService,
            ISemesterService semesterService,
            IUserSocialProviderService userSocialProviderService,
            IChapterService chapterService,
            IGradeService gradeService,
            ISolutionReportService solutionReportService,
            ISubscriptionTypeService subscriptionTypeService,
            IRegisterService registerService,
            ILessonService lessonService,
            ISolutionService solutionService,
            IUserSubscriptionService userSubscriptionService,
            IExamService examService,
            IQuestionReportService questionReportService,
            ITextBookService textBookService,
            IPayOSService payOSService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _questionsService = questionsService ?? throw new ArgumentNullException(nameof(questionsService));
            _semesterService = semesterService ?? throw new ArgumentNullException(nameof(semesterService));
            _userSocialProviderService = userSocialProviderService ?? throw new ArgumentNullException(nameof(userSocialProviderService));
            _chapterService = chapterService ?? throw new ArgumentNullException(nameof(chapterService));
            _gradeService = gradeService ?? throw new ArgumentNullException(nameof(gradeService));
            _solutionReportService = solutionReportService ?? throw new ArgumentNullException(nameof(solutionReportService));
            _subscriptionTypeService = subscriptionTypeService ?? throw new ArgumentNullException(nameof(subscriptionTypeService));
            _registerService = registerService ?? throw new ArgumentNullException(nameof(registerService));
            _lessonService = lessonService ?? throw new ArgumentNullException(nameof(_lessonService));
            _solutionService = solutionService ?? throw new ArgumentNullException(nameof(solutionService));
            _userSubscriptionService = userSubscriptionService ?? throw new ArgumentException(nameof(userSubscriptionService));
            _examService = examService ?? throw new ArgumentNullException(nameof(examService));
            _questionReportService = questionReportService ?? throw new ArgumentNullException(nameof(questionReportService));
            _textBookService = textBookService ?? throw new ArgumentNullException(nameof(textBookService));
            _payOSService = payOSService ?? throw new ArgumentNullException(nameof(payOSService));
        }
      

        public IAuthService AuthService => _authService;
        public ILoginService LoginService => _loginService;
        public IUserService UserService => _userService;
        public IQuestionsService QuestionsService => _questionsService;
        public ISemesterService SemesterService => _semesterService;
        public IUserSocialProviderService UserSocialProviderService => _userSocialProviderService;
        public IChapterService ChapterService => _chapterService;
        public IGradeService GradeService => _gradeService;
        public ISolutionReportService SolutionReportService => _solutionReportService;
        public ISubscriptionTypeService SubscriptionTypeService => _subscriptionTypeService;
        public IRegisterService RegisterService => _registerService;
        public ILessonService LessonService => _lessonService;
        public ISolutionService SolutionService => _solutionService;
        public IUserSubscriptionService UserSubscriptionService => _userSubscriptionService;
        public IExamService ExamService => _examService;

        public IQuestionReportService QuestionReportService => _questionReportService;

        public ITextBookService TextBookService => _textBookService;

        public IPayOSService PayOSService => _payOSService;
    }
}
