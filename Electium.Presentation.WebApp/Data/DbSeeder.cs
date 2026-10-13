using Electium.Core.Application.DTOs;
using Electium.Core.Application.Interfaces;
using Electium.Core.Domain.Enums;

namespace Electium.Presentation.WebApp.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            var userService = services.GetRequiredService<IUserService>();

            // Only seed if there are no users yet
            if (await userService.AnyAsync()) return;

            await userService.CreateAsync(new CreateUserDto
            {
                FirstName = "Admin",
                LastName = "Electium",
                Email = "admin@electium.local",
                UserName = "admin",
                Password = "Admin123!",
                ConfirmPassword = "Admin123!",
                Role = UserRole.Administrator
            });
        }
    }
}
