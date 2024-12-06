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
        #region Make Appointment With Patient Choise
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

                if (existingAppiontment.PatientId == appointmentDto.PatientId)
                {
                    return BadRequest("You have already booked this examination time !!!");
                }

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
        #endregion

        #region Make Appointment With In Ordering

        [HttpPost]
        public async Task<IActionResult> MakeAppointmentInOrder(AppointmentInOrderDto appointmentDto)
        {
            try
            {
                var doctorAvailability = await _context.DoctorAvailabilities
                    .FirstOrDefaultAsync(a => a.DoctorId == appointmentDto.DoctorId
                                              && a.DayOfWeek == appointmentDto.AppointmentDate.DayOfWeek.ToString());

                if (doctorAvailability == null)
                {
                    return BadRequest("No availability found for the doctor on this day.");
                }


                var lastAppointment = await _context.Appointments
                    .Where(a => a.DoctorId == appointmentDto.DoctorId
                                && a.AppointmentDate == appointmentDto.AppointmentDate)
                    .OrderByDescending(a => a.ExaminationStartTime)
                    .FirstOrDefaultAsync();


                var nextStartTime = lastAppointment != null
                    ? lastAppointment.ExaminationStartTime.Add(TimeSpan.FromMinutes(doctorAvailability.ExaminationDuration))
                    : doctorAvailability.StartTime;


                if (nextStartTime + TimeSpan.FromMinutes(doctorAvailability.ExaminationDuration) > doctorAvailability.EndTime)
                {
                    return BadRequest("No available time slots for this day.");
                }


                var appointment = new Appointment
                {
                    PatientId = appointmentDto.PatientId,
                    DoctorId = appointmentDto.DoctorId,
                    AppointmentDate = appointmentDto.AppointmentDate,
                    ExaminationStartTime = nextStartTime,
                    Status = "Booked"
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Appointment booked successfully",
                    AppointmentDetails = new
                    {
                        DoctorId = appointment.DoctorId,
                        PatientId = appointment.PatientId,
                        AppointmentDate = appointment.AppointmentDate,
                        ExaminationStartTime = appointment.ExaminationStartTime
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        #endregion

        #region Cancel Appointment
        [HttpPost]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(appointmentId);

                if (appointment == null)
                {
                    return BadRequest("This appointment is not exist");
                }

                var appointments = await _context.Appointments
                                    .Where(ap => ap.AppointmentDate == appointment.AppointmentDate)
                                    .ToListAsync();

                var doctorAvailability = await _context.DoctorAvailabilities
                                                .Where(a => a.DoctorId == appointment.DoctorId
                                                        && appointment.AppointmentDate.DayOfWeek.ToString() == a.DayOfWeek)
                                                .FirstOrDefaultAsync();

                if (doctorAvailability == null)
                {
                    return BadRequest("This doctor availablity not found !!!");
                }

                foreach (var app in appointments)
                {
                    if (app.ExaminationStartTime > appointment.ExaminationStartTime)
                    {
                        app.ExaminationStartTime -= TimeSpan.FromMinutes(doctorAvailability.ExaminationDuration);
                    }
                }

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                return Ok(new { Message = $"Appointment Removed" });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        #endregion


    }
}
