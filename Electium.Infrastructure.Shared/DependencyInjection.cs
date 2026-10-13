using Electium.Core.Application.Interfaces;
using Electium.Infrastructure.Shared.Services;
using Electium.Infrastructure.Shared.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Electium.Infrastructure.Shared
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddShared(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Bind email settings from appsettings.json
            services.Configure<EmailSettings>(
                configuration.GetSection("EmailSettings"));

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
