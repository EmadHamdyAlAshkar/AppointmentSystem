using AppointmentSystem.Domain.Contexts;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Service.AppointmentServices.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentSystemDbContext _context;

        public AppointmentController(AppointmentSystemDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> MakeAppointment(AppointmentDto appointmentDto)
        {
            try
            {
                var doctorAvailabilities = await _context.DoctorAvailabilities
                                        .Where(a => a.DoctorId == appointmentDto.DoctorId
                                                && a.DayOfWeek == appointmentDto.AppointmentDate.DayOfWeek.ToString())
                                                .ToListAsync();

                var isAvailable = doctorAvailabilities.Any(a =>
                                                           a.StartTime <= appointmentDto.ExaminationStartTime
                                                           && appointmentDto.ExaminationStartTime + TimeSpan.FromMinutes(a.ExaminationDuration) <= a.EndTime);


                if (!isAvailable)
                {
                    return BadRequest("No available examination time for the doctor !!!");
                }

                //var existingAppiontment = await _context.Appointments
                //    .FirstOrDefaultAsync(ap => ap.DoctorId == appointmentDto.DoctorId
                //                         && ap.AppointmentDate == appointmentDto.AppointmentDate
                //                         && ap.ExaminationStartTime == appointmentDto.ExaminationStartTime);

                var existingAppiontment = await _context.Appointments
                    .FirstOrDefaultAsync(ap => ap.DoctorId == appointmentDto.DoctorId
                                         && ap.AppointmentDate == appointmentDto.AppointmentDate
                                         && ap.ExaminationStartTime == appointmentDto.ExaminationStartTime);

                if (existingAppiontment != null)
                {
                    return BadRequest("This examination time is already booked !!!");
                }

                var appointment = new Appointment
                {
                    PatientId = appointmentDto.PatientId,
                    DoctorId = appointmentDto.DoctorId,
                    AppointmentDate = appointmentDto.AppointmentDate,
                    ExaminationStartTime = appointmentDto.ExaminationStartTime,
                    Status = "Booked"
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return Ok("Appointment booked successfully");
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            
        }
    }
}
