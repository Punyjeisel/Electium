using Electium.Core.Application.Interfaces;
using Electium.Core.Application.Mappings;
using Electium.Core.Application.Services;
using Electium.Core.Domain.Interfaces;
using Electium.Infrastructure.Persistence;
using Electium.Infrastructure.Shared;
using Electium.Infrastructure.Shared.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Session (used for the voting flow)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Layers
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddShared(builder.Configuration);

// AutoMapper - load all profiles from Application
builder.Services.AddAutoMapper(cfg => { }, typeof(UserProfile).Assembly);

// Generic service
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));

// Specific services
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

builder.Services.AddScoped<IFileStorageService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    return new FileStorageService(env.WebRootPath);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await Electium.Presentation.WebApp.Data.DbSeeder.SeedAdminAsync(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();