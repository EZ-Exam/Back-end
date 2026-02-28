using AutoMapper;
using teamseven.EzExam.Repository.Dtos;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Services.Object.Requests;
using teamseven.EzExam.Services.Object.Responses;

namespace teamseven.EzExam.Services.MappingProfiles
{
    public class EzExamMappingProfile : Profile
    {
        public EzExamMappingProfile()
        {
            // Auto-generated mappings for Entities <-> DTOs
            CreateMap<Answer, AnswerDetailResponse>().ReverseMap();
            CreateMap<Chapter, ChapterDataResponse>().ReverseMap();
            CreateMap<Exam, ExamDataResponse>().ReverseMap();
            CreateMap<Exam, ExamDetailOptimizedResponse>().ReverseMap();
            CreateMap<Exam, ExamFeedResponse>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : null))
                .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Name : null))
                .ForMember(dest => dest.ExamTypeName, opt => opt.MapFrom(src => src.ExamType != null ? src.ExamType.Name : null))
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Email : null))
                .ForMember(dest => dest.AttemptCount, opt => opt.MapFrom(src => src.ExamHistories.Count))
                .ForMember(dest => dest.AverageScore, opt => opt.MapFrom(src => src.ExamHistories.Any() ? src.ExamHistories.Average(h => h.Score) : 0m))
                .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.TotalQuestions != 0 ? src.TotalQuestions : src.ExamQuestions.Count));
            CreateMap<ExamRequest, Exam>();
            CreateMap<UpdateExamRequest, Exam>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Exam, ExamResponse>().ReverseMap();
            CreateMap<ExamHistory, ExamHistoryMinimalResponse>()
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.SubjectId : 0))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Exam != null && src.Exam.Subject != null ? src.Exam.Subject.Name : "Unknown"))
                .ForMember(dest => dest.GradeId, opt => opt.MapFrom(src => src.Exam != null ? (int?)src.Exam.GradeId : null))
                .ForMember(dest => dest.GradeName, opt => opt.MapFrom(src => src.Exam != null && src.Exam.Grade != null ? src.Exam.Grade.Name : "Unknown"))
                .ForMember(dest => dest.ChapterId, opt => opt.MapFrom(src => src.Exam != null && src.Exam.Lesson != null ? (int?)src.Exam.Lesson.ChapterId : null))
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Exam != null && src.Exam.Lesson != null && src.Exam.Lesson.Chapter != null ? src.Exam.Lesson.Chapter.Name : "Unknown"))
                .ForMember(dest => dest.LessonId, opt => opt.MapFrom(src => src.Exam != null ? (int?)src.Exam.LessonId : null))
                .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Exam != null && src.Exam.Lesson != null ? src.Exam.Lesson.Name : "Unknown"));
            CreateMap<ExamHistory, ExamHistoryResponse>().ReverseMap();
            CreateMap<ExamQuestion, ExamQuestionDetailResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Question.Id))
                .ForMember(dest => dest.ContentQuestion, opt => opt.MapFrom(src => src.Question.Content ?? string.Empty))
                .ForMember(dest => dest.CorrectAnswer, opt => opt.MapFrom(src => src.Question.CorrectAnswer))
                .ForMember(dest => dest.Explanation, opt => opt.MapFrom(src => src.Question.Explanation))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Question.Image))
                .ForMember(dest => dest.Formula, opt => opt.MapFrom(src => src.Question.Formula))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => 
                    !string.IsNullOrEmpty(src.Question.Options) 
                        ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(src.Question.Options, new System.Text.Json.JsonSerializerOptions()) ?? new List<string>() 
                        : (src.Question.Answers != null ? src.Question.Answers.Select(a => a.Content).ToList() : new List<string>())));
            CreateMap<ExamQuestion, ExamQuestionRequest>().ReverseMap();
            CreateMap<ExamQuestion, ExamQuestionResponse>().ReverseMap();
            CreateMap<Grade, GradeDataResponse>().ReverseMap();
            CreateMap<Lesson, LessonDataResponse>().ReverseMap();
            CreateMap<Lesson, LessonDetailOptimizedResponse>().ReverseMap();
            CreateMap<Lesson, LessonFeedResponse>().ReverseMap();
            CreateMap<LessonEnhanced, LessonEnhancedResponse>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.subjectId, opt => opt.MapFrom(src => src.SubjectId.ToString()))
                .ForMember(dest => dest.pdfUrl, opt => opt.MapFrom(src => src.PdfUrl))
                .ForMember(dest => dest.createdAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.updatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.questions, opt => opt.Ignore());
            // Question -> QuestionDataResponse
            CreateMap<Question, QuestionDataResponse>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.QuestionType != null ? src.QuestionType.ToLower() : "multiple-choice"))
                .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Name : null))
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter != null ? src.Chapter.Name : null))
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Email : null))
                .ForMember(dest => dest.Options, opt => opt.Ignore());
            // QuestionDataRequest -> Question
            CreateMap<QuestionDataRequest, Question>()
                .ForMember(dest => dest.Options, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionType, opt => opt.Ignore());
            CreateMap<Question, QuestionFeedResponse>().ReverseMap();
            CreateMap<Question, QuestionMinimalResponse>().ReverseMap();
            CreateMap<Question, QuestionSimpleResponse>().ReverseMap();
            // QuestionComment -> QuestionCommentResponse
            CreateMap<QuestionComment, QuestionCommentResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Email : "Unknown"))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : "Unknown"))
                .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.Replies, opt => opt.Ignore());
            CreateMap<QuestionReport, QuestionReportDataResponse>().ReverseMap();
            CreateMap<Semester, SemesterDataResponse>().ReverseMap();
            CreateMap<Solution, SolutionDataResponse>().ReverseMap();
            CreateMap<SolutionReport, SolutionReportRequest>().ReverseMap();
            CreateMap<SolutionReport, SolutionReportResponse>().ReverseMap();
            CreateMap<CreateSolutionRequest, Solution>();
            // User -> BalanceResponse
            CreateMap<User, BalanceResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FullName ?? "Unknown"))
                .ForMember(dest => dest.PreviousBalance, opt => opt.Ignore())
                .ForMember(dest => dest.AddedAmount, opt => opt.Ignore())
                .ForMember(dest => dest.NewBalance, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore());
            CreateMap<StudentPerformanceSummary, StudentPerformanceSummaryResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : null))
                .ForMember(dest => dest.GradeName, opt => opt.MapFrom(src => src.Grade != null ? src.Grade.Name : null))
                .ReverseMap();
            CreateMap<StudentQuestionAttempt, StudentQuestionAttemptRequest>().ReverseMap();
            CreateMap<StudentQuestionAttempt, StudentQuestionAttemptResponse>()
                .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => "Multiple Choice"))
                .ForMember(dest => dest.QuestionContent, opt => opt.MapFrom(src => src.Question != null ? src.Question.Content : null))
                .ForMember(dest => dest.SelectedAnswerContent, opt => opt.MapFrom(src => src.SelectedAnswer != null ? src.SelectedAnswer.Content : null))
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter != null ? src.Chapter.Name : null))
                .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Name : null))
                .ReverseMap();
            CreateMap<StudentQuizHistory, StudentQuizHistoryResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Name : null))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => (src.Exam != null && src.Exam.Subject != null) ? src.Exam.Subject.Name : null))
                .ForMember(dest => dest.ExamTypeName, opt => opt.MapFrom(src => (src.Exam != null && src.Exam.ExamType != null) ? src.Exam.ExamType.Name : null))
                .ReverseMap();
            // SubscriptionTypeRequest -> SubscriptionType
            CreateMap<SubscriptionTypeRequest, SubscriptionType>();
            CreateMap<SubscriptionType, SubscriptionTypeRequest>().ReverseMap();
            CreateMap<SubscriptionType, SubscriptionTypeResponse>().ReverseMap();
            CreateMap<TestSession, TestSessionResponse>().ReverseMap();
            CreateMap<TestSessionAnswer, TestSessionAnswerResponse>().ReverseMap();
            CreateMap<TextBook, TextBookDataResponse>().ReverseMap();
            CreateMap<CreateTextBookRequest, TextBook>();
            // CreateLessonRequest -> Lesson
            CreateMap<CreateLessonRequest, Lesson>();
            CreateMap<User, UserResponse>().ReverseMap();
            CreateMap<UserQuestionCart, UserQuestionCartResponse>().ReverseMap();
            CreateMap<UserSocialProvider, UserSocialProviderDataResponse>().ReverseMap();
            CreateMap<CreateUserSocialProviderRequest, UserSocialProvider>();
            CreateMap<UserSubscription, UserSubscriptionDataResponse>().ReverseMap();
            CreateMap<UserSubscription, UserSubscriptionRequest>().ReverseMap();
            // UsageTrackingRequest -> UserUsageTracking / UserUsageHistory
            CreateMap<UsageTrackingRequest, UserUsageTracking>()
                .ForMember(dest => dest.SubscriptionTypeId, opt => opt.Ignore())
                .ForMember(dest => dest.UsageCount, opt => opt.Ignore())
                .ForMember(dest => dest.ResetDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<UsageTrackingRequest, UserUsageHistory>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            // UserUsageTracking -> UsageTrackingResponse
            CreateMap<UserUsageTracking, UsageTrackingResponse>();
            // UserUsageHistory -> UsageHistoryResponse
            CreateMap<UserUsageHistory, UsageHistoryResponse>();
            // CreateMap<UserSubscription, UserSubscriptionResponse>().ReverseMap();
        }
    }
}
