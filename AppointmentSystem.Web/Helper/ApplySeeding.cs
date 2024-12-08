using AppointmentSystem.Domain.Contexts;
using AppointmentSystem.Repository;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Web.Helper
{
    public class ApplySeeding
    {
        public static async Task ApplySeedingAsync(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                try
                {
                    var context = services.GetRequiredService<AppointmentSystemDbContext>();

                    await context.Database.MigrateAsync();

                    await AppointmentContextSeed.SeedAsync(context, loggerFactory);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<ApplySeeding>();
                    throw;
                }
            }
        }
    }
}
