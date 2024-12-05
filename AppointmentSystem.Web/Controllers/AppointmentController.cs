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

                
                var doctorAvailability = await _context.DoctorAvailabilities
                    .FirstOrDefaultAsync(a => a.DoctorId == appointmentDto.DoctorId
                                             && a.DayOfWeek == appointmentDto.AppointmentDate.DayOfWeek.ToString());

                if (doctorAvailability == null)
                {
                    return BadRequest("No available examination time for the doctor !!!");
                }

                // دى لستة من الاوقات المسموح بيها علشان لما حد يدخل وقت معين هيشوف مناسبة للستة دى ولا لا
                var allowedStartTimes = new List<TimeSpan>();

                //هنا هحدد البداية بتاعتى علشان هبدا ازود عليها عدد مرات على حسب الدكتور محدد كام دقيقة
                var time = doctorAvailability.StartTime;
                while (time < doctorAvailability.EndTime)
                {
                    //هنا مثلا لو البداية الساعة 8:00 هيضيفها ف اللستة بتاعت الاوقات المسموحة 
                    allowedStartTimes.Add(time);
                    //هنا هيزود المدة اللى الدكتور حاططها على البداية 
                    time = time.Add(TimeSpan.FromMinutes(doctorAvailability.ExaminationDuration));
                    //بعدها هيرجع يزود الاوقات المسموحة وهكذا لحد م الوقت يساوى وقت النهاية ساعتها هيقف
                }

                // هنا هيروح يشوف اذا كان الوقت اللى المريض مدخله متناسب مع اى وقت من اللستة اللى انا محددها
                if (!allowedStartTimes.Contains(appointmentDto.ExaminationStartTime))
                {
                    return BadRequest("The selected start time is not valid based on the doctor's availability and examination duration.");
                }


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
