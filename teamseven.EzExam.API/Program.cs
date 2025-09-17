using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using teamseven.EzExam.Repository;
using teamseven.EzExam.Repository.Basic;
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
var builder = WebApplication.CreateBuilder(args);

// ================= C?U HÌNH DB =================
//builder.Services.AddDbContext<teamsevenEzExamdbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<teamsevenezexamdbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(60);
    }));

// ================= C?U HÌNH AUTHENTICATION =================
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
        Console.WriteLine("FirebaseApp kh?i t?o thành công.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"L?i khi kh?i t?o Firebase: {ex.Message}");
    throw;
}


// ================= ÐANG KÝ REPOSITORY & SERVICE =================
// Ðang ký d?ch v? v?i DI container

// GHI CHÚ: Ðang ký Scoped và các lifetime trong ASP.NET Core
// 1. Scoped: M?t instance m?i HTTP request, dùng cho DbContext, repository, service liên quan d?n request
//    - Ví d?: DbContext, GenericRepository<Image>, ImageService
//    - Lý do: Ð?m b?o nh?t quán trong request, an toàn v?i nhi?u request d?ng th?i


// ================= ÐANG KÝ REPOSITORY & SERVICE =================
// ?? Repository Layer (Scoped)
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// ?? Service Layer (Scoped)
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
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<ITextBookService, TextBookService>();
builder.Services.AddScoped<IPayOSService, PayOSService>(); // Ðang ký IPayOSService tru?c
// builder.Services.AddScoped<IServiceProviders, ServiceProviders>();

// ?? Utility & Helper Services
builder.Services.AddTransient<IEmailService, EmailService>(); // Email service (Transient)
builder.Services.AddSingleton<IPasswordEncryptionService, PasswordEncryptionService>(); // Encryption (Singleton)
builder.Services.AddSingleton<IIdObfuscator, IdObfuscator>();
builder.Services.AddSingleton<NotificationService>();

// 2. Singleton: M?t instance duy nh?t cho c? ?ng d?ng, dùng cho d?ch v? không tr?ng thái
//    - Ví d?: C?u hình, logger toàn c?c
//    - C?n th?n: Không dùng cho DbContext/repository vì gây l?i concurrency
// Ví d?: builder.Services.AddSingleton<SomeConfigService>();

// 3. Transient: Instance m?i m?i l?n g?i, dùng cho d?ch v? nh?, không luu tr?ng thái
//    - Ví d?: Email sender, d?ch v? t?m th?i
// Ví d?: builder.Services.AddTransient<SomeLightweightService>();

//TÓM L?I: NÊN H?I CON AI COI NÊN XÀI SCOPE GÌ???






// ================= C?U HÌNH BLOB STORAGE =================
////var blobServiceClient = new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]);
//builder.Services.AddSingleton(blobServiceClient);

// ================= C?U HÌNH CORS =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// ================= THAY Ð?I Ð? L?NG NGHE HTTP =================
builder.WebHost.UseUrls("http://0.0.0.0:5000"); // Ðã thay d?i t? https sang http

// ================= C?U HÌNH SWAGGER =================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EzExam", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Vui lòng nh?p Bearer Token (VD: Bearer eyJhbGciOi...)",
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

// ================= C?U HÌNH AUTOMAPPER =================
// ================= C?U HÌNH CONTROLLERS =================
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
