using Azure.Storage.Blobs;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Basic;
using teamseven.EzExam.Repository.Models;
using teamseven.EzExam.Repository.Context;
using teamseven.EzExam.Repository.Repository;
using teamseven.EzExam.Services.Extensions;
using teamseven.EzExam.Services.Interfaces;
using teamseven.EzExam.Services.Services;
using teamseven.EzExam.Services.Services.Authentication;
using teamseven.EzExam.Services.Services.ChapterService;
using teamseven.EzExam.Services.Services.ExamService;
using teamseven.EzExam.Services.Services.GradeService;
using teamseven.EzExam.Services.Services.LessonService;
using teamseven.EzExam.Services.Services.OtherServices;
using teamseven.EzExam.Services.Services.QuestionReportService;
using teamseven.EzExam.Services.Services.QuestionsService;
using teamseven.EzExam.Services.Services.SemesterService;
using teamseven.EzExam.Services.Services.ServiceProvider;
using teamseven.EzExam.Services.Services.SolutionReportService;
using teamseven.EzExam.Services.Services.SolutionService;
using teamseven.EzExam.Services.Services.SubscriptionTypeService;
using teamseven.EzExam.Services.Services.TextBookService;
using teamseven.EzExam.Services.Services.UserService;
using teamseven.EzExam.Services.Services.UserSocialProviderService;
using teamseven.EzExam.Services.Services.UserSubscriptionService;
using teamseven.EzExam.Services.Services.UsageTrackingService;
using teamseven.EzExam.Services.Services.BalanceService;
using teamseven.EzExam.Services.Services.JwtHelperService;
using teamseven.EzExam.Services.Services.SubscriptionService;
using teamseven.EzExam.Services.Services.TestSystemServices;
using teamseven.EzExam.API.Middleware;
using teamseven.EzExam.API.Services;
using teamseven.EzExam.Services.Services.LessonEnhancedService;
var builder = WebApplication.CreateBuilder(args);

// ================= CẤU HÌNH DB =================
//builder.Services.AddDbContext<teamsevenEzExamdbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<teamsevenezexamdbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(3);
    }));

// ================= CẤU HÌNH AUTHENTICATION =================
ConfigureAuthentication(builder.Services, builder.Configuration);

try
{
    if (FirebaseApp.DefaultInstance == null)
    {
        string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, builder.Configuration.GetSection("Firebase:CredentialsPath").Value);
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile(jsonPath)
        });
        Console.WriteLine("FirebaseApp khởi tạo thành công.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Lỗi khi khởi tạo Firebase: {ex.Message}");
    throw;
}


// ================= ĐĂNG KÝ REPOSITORY & SERVICE =================
// Đăng ký dịch vụ với DI container

// GHI CHÚ: Đăng ký Scoped và các lifetime trong ASP.NET Core
// 1. Scoped: Một instance mỗi HTTP request, dùng cho DbContext, repository, service liên quan đến request
//    - Ví dụ: DbContext, GenericRepository<Image>, ImageService
//    - Lý do: Đảm bảo nhất quán trong request, an toàn với nhiều request đồng thời


// ================= ĐĂNG KÝ REPOSITORY & SERVICE =================
// 📌 Repository Layer (Scoped)
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// 📌 Service Layer (Scoped)
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
builder.Services.AddScoped<ISolutionService, SolutionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQuestionsService, QuestionsService>();
builder.Services.AddScoped<IQuestionReportService, QuestionReportService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IUserSocialProviderService, UserSocialProviderService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<ISolutionReportService, SolutionReportService>();
builder.Services.AddScoped<ISubscriptionTypeService, SubscriptionTypeService>();
builder.Services.AddScoped<IUsageTrackingService, UsageTrackingService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IJwtHelperService, JwtHelperService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<ITextBookService, TextBookService>();
builder.Services.AddScoped<IPayOSService, PayOSService>();
builder.Services.AddScoped<LessonEnhancedRepository>();            // repo thẳng
builder.Services.AddScoped<ILessonEnhancedService, LessonEnhancedService>(); // service


// Test System Services
builder.Services.AddScoped<IUserQuestionCartService, UserQuestionCartService>();
builder.Services.AddScoped<ITestSessionService, TestSessionService>();

// 📌 Background Services - DISABLED for performance
// builder.Services.AddHostedService<SubscriptionExpirationService>();

// 📌 Utility & Helper Services
builder.Services.AddTransient<IEmailService, EmailService>(); // Email service (Transient)
builder.Services.AddSingleton<IPasswordEncryptionService, PasswordEncryptionService>(); // Encryption (Singleton)
builder.Services.AddSingleton<IIdObfuscator, IdObfuscator>();
builder.Services.AddSingleton<NotificationService>();

// 📌 Service Provider (must be registered after all other services)
builder.Services.AddScoped<IServiceProviders, ServiceProviders>();

// 2. Singleton: Một instance duy nhất cho cả ứng dụng, dùng cho dịch vụ không trạng thái
//    - Ví dụ: Cấu hình, logger toàn cục
//    - Cẩn thận: Không dùng cho DbContext/repository vì gây lỗi concurrency
// Ví dụ: builder.Services.AddSingleton<SomeConfigService>();

// 3. Transient: Instance mới mỗi lần gọi, dùng cho dịch vụ nhẹ, không lưu trạng thái
//    - Ví dụ: Email sender, dịch vụ tạm thời
// Ví dụ: builder.Services.AddTransient<SomeLightweightService>();

//TÓM LẠI: NÊN HỎI CON AI COI NÊN XÀI SCOPE GÌ???






// ================= CẤU HÌNH BLOB STORAGE =================
////var blobServiceClient = new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]);
//builder.Services.AddSingleton(blobServiceClient);

// ================= CẤU HÌNH SUPABASE =================
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:ServiceRoleKey"];

if (!string.IsNullOrEmpty(supabaseUrl) && !string.IsNullOrEmpty(supabaseKey))
{
    builder.Services.AddSingleton<Supabase.Client>(provider => 
    {
        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = false,
            AutoRefreshToken = false
        };
        return new Supabase.Client(supabaseUrl, supabaseKey, options);
    });
    Console.WriteLine("Supabase client registered successfully.");
}
else
{
    Console.WriteLine("Supabase configuration is missing. PDF blob functionality will not be available.");
}

// ================= CẤU HÌNH CORS =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// ================= THAY ĐỔI ĐỂ LẮNG NGHE HTTP =================
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// ================= CẤU HÌNH SWAGGER =================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EzExam", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Vui lòng nhập Bearer Token (VD: Bearer eyJhbGciOi...)",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.EnableAnnotations();
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// ================= CẤU HÌNH AUTOMAPPER =================
// ================= CẤU HÌNH MEMORY CACHE =================
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1000; // Maximum number of cache entries
    options.CompactionPercentage = 0.25; // Remove 25% of entries when limit is reached
});

// ================= CẤU HÌNH CONTROLLERS =================
builder.Services.AddControllers();
builder.Services.AddHttpClient();

var app = builder.Build();

// ================= MIDDLEWARE =================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EzExam API V1");
    c.RoutePrefix = "swagger";
});

app.UseDatabaseKeepAlive();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Add subscription middleware to check AI access
app.UseSubscriptionMiddleware();

app.MapControllers();
app.Run();

// ================= AUTHENTICATION CONFIGURATION FUNCTION =================
void ConfigureAuthentication(IServiceCollection services, IConfiguration config)
{
    // Retrieve JWT key from configuration
    var jwtKey = config["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("JWT Key is missing in the configuration.");
    }

    // JWT Authentication Configuration
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Verify the issuer
            ValidateAudience = true, // Verify the audience
            ValidateLifetime = true, // Check expiration time
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("JWT Token validated successfully.");
                return Task.CompletedTask;
            }
        };
    });


    // Google Authentication
    services.AddAuthentication()
        .AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = config["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = config["Authentication:Google:ClientSecret"];
            googleOptions.SaveTokens = true;
            googleOptions.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
            googleOptions.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");
        });


    // Authorization policies
    services.AddAuthorization(options =>
    {
        options.AddPolicy("DeliveringStaffPolicy", policy => policy.RequireClaim("roleId", "2"));
        options.AddPolicy("SaleStaffPolicy", policy => policy.RequireClaim("roleId", "3"));
    });
}