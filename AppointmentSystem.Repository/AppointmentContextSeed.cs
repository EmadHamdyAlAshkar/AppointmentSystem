using AppointmentSystem.Domain.Contexts;
using AppointmentSystem.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppointmentSystem.Repository
{
    public class AppointmentContextSeed
    {
        public static async Task SeedAsync(AppointmentSystemDbContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                if (context.Doctors != null && !context.Doctors.Any())
                {
                    var doctorsData = File.ReadAllText("../AppointmentSystem.Repository/SeedData/Doctors.json");

                    var doctors = JsonSerializer.Deserialize<List<Doctor>>(doctorsData);

                    if (doctors is not null)
                        await context.Doctors.AddRangeAsync(doctors);

                }

                if (context.Patients !=  null && !context.Patients.Any())
                {
                    var patientsData = File.ReadAllText("../AppointmentSystem.Repository/SeedData/Patients.json");

                    var patients = JsonSerializer.Deserialize<List<Patient>>(patientsData);

                    if(patients is not null)
                        await context.Patients.AddRangeAsync(patients);
                }

                



                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                var logger = loggerFactory.CreateLogger<AppointmentContextSeed>();
                logger.LogError(ex.Message);

            }
        }
    }
}
