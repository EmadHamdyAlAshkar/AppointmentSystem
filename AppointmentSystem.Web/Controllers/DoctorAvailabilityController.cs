using AppointmentSystem.Domain.Contexts;
using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Service.AvailabilityServices.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DoctorAvailabilityController : ControllerBase
    {
        private readonly AppointmentSystemDbContext _context;

        public DoctorAvailabilityController(AppointmentSystemDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddAvailability(DoctorAvailabilityDto availabilityDto)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == availabilityDto.DoctorId);

            if (doctor == null)
            {
                return BadRequest("Doctor Not Found !!!");
            }

            var existingAvailability = await _context.DoctorAvailabilities
                                        .FirstOrDefaultAsync(a => a.DoctorId == availabilityDto.DoctorId
                                                                  && a.DayOfWeek == availabilityDto.DayOfWeek
                                                                  && (
                                                                        (availabilityDto.StartTime >= a.StartTime && availabilityDto.StartTime < a.EndTime) || // البداية داخل نطاق موجود
                                                                        (availabilityDto.EndTime > a.StartTime && availabilityDto.EndTime <= a.EndTime) ||   // النهاية داخل نطاق موجود
                                                                        (availabilityDto.StartTime <= a.StartTime && availabilityDto.EndTime >= a.EndTime)   // يغطي نطاق موجود بالكامل
                                                                     ));
            if (existingAvailability != null)
            {
                return  BadRequest("The doctor already has availability during this time !!!");
            }

            var availability = new DoctorAvailability
            {
                DoctorId = availabilityDto.DoctorId,
                DayOfWeek = availabilityDto.DayOfWeek,
                StartTime = availabilityDto.StartTime,
                EndTime = availabilityDto.EndTime,
                ExaminationDuration = availabilityDto.ExaminationDuration,
            };

            _context.DoctorAvailabilities.AddAsync(availability);
            await _context.SaveChangesAsync();

            return Ok("Availibility Added Successfully");
        }
    }
}
