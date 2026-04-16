using Serilog;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ZucoHR.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ZucoHR.Infrastructure.Repository;
using ZucoHR.Infrastructure.Interfaces;
using ZucoHR.Application.Interfaces;
using ZucoHR.Application.Services;
using ZucoHR.Infrastructure;
using ZucoHR.Shared;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using ZucoHR.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ZucoHR.Application.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ?? Serilog configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/zucohr-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers().AddFluentValidation();

// DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<ZucoHrDbContext>(options =>
//    options.UseNpgsql(conn));

builder.Services.AddDbContext<ZucoHrDbContext>(options =>
    options.UseSqlServer(conn));


builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ZucoHrDbContext>()
                .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 7;
});

// AutoMapper
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Authentication (JWT)
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuerSigningKey = true
    };
});

// DI - Repositories (Infrastructure implementations)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<IPerformanceRepository, PerformanceRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IRecruitmentRepository, RecruitmentRepository>();
builder.Services.AddScoped<IOnboardingRepository, OnboardingRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();

// DI - Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IRecruitmentService, RecruitmentService>();
builder.Services.AddScoped<IOnboardingService, OnboardingService>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins("*"));

});

//builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LeaveRequestDtoValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ZucoHR", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below. \r\n\r\nBearer"
    });
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


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ZucoHrDbContext>();
    ctx.Database.Migrate();
    await SeedData.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseMiddleware<TenantMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
