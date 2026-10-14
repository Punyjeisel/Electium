using Electium.Core.Application.Interfaces;
using Electium.Core.Application.Mappings;
using Electium.Core.Application.Services;
using Electium.Core.Domain.Interfaces;
using Electium.Infrastructure.Persistence;
using Electium.Infrastructure.Shared;
using Electium.Infrastructure.Shared.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- MVC ---
builder.Services.AddControllersWithViews();

// --- Session (used by the voting flow) ---
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// --- Authentication (cookie based) ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddAuthorization();

// --- Layers ---
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddShared(builder.Configuration);

// --- AutoMapper ---
builder.Services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly);

// --- Generic services ---
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));

// --- Specific services ---
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICitizenService, CitizenService>();
builder.Services.AddScoped<IPoliticalPartyService, PoliticalPartyService>();
builder.Services.AddScoped<IElectivePositionService, ElectivePositionService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IPartyLeaderService, PartyLeaderService>();
builder.Services.AddScoped<ICandidatePositionService, CandidatePositionService>();
builder.Services.AddScoped<IAllianceService, AllianceService>();
builder.Services.AddScoped<IElectionService, ElectionService>();
builder.Services.AddScoped<IVotingService, VotingService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ILeaderDashboardService, LeaderDashboardService>();

// --- File storage and OCR (they need WebRootPath) ---
builder.Services.AddScoped<IFileStorageService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new FileStorageService(env.WebRootPath);
});

builder.Services.AddScoped<IOcrService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var tessdataPath = Path.Combine(env.WebRootPath, "tessdata");
    return new OcrService(tessdataPath);
});

var app = builder.Build();

// --- Seed admin ---
using (var scope = app.Services.CreateScope())
{
    await Electium.Presentation.WebApp.Data.DbSeeder.SeedAdminAsync(scope.ServiceProvider);
}

// --- Middleware pipeline ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();           // before auth
app.UseAuthentication();    // before authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();