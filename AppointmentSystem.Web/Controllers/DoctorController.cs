using AppointmentSystem.Domain.Contexts;
using AppointmentSystem.Service.DoctorServices;
using AppointmentSystem.Service.DoctorServices.Dtos;
using AppointmentSystem.Service.PaitentServices.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using AppointmentSystem.Service.AvailabilityServices.Dtos;
using AutoMapper;

namespace AppointmentSystem.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly AppointmentSystemDbContext _context;
        private readonly IMapper _mapper;

        public DoctorController(IDoctorService doctorService,
                                AppointmentSystemDbContext context,
                                IMapper mapper)
        {
            _doctorService = doctorService;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorById(int docotrId)
        {
            var doctor = await _doctorService.GetByIdAsync(docotrId);
            return Ok(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(CreateDoctorDto doctorDto)
        {
            await _doctorService.AddAsync(doctorDto);

            return Ok(doctorDto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDoctor(UpdateDoctorDto doctorDto)
        {
            await _doctorService.UpdateAsync(doctorDto);
            return Ok(doctorDto);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            await _doctorService.DeleteAsync(id);
            return Ok(new { Message = $"Doctor With Id ({id}) Deleted Successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorSchedule(int id, string? specialization, double? minRating, string? dayOfWeek)
        {
            #region With Manual Mapping
            //var doctor = await _context.Doctors.FindAsync(id);

            //var doctorAvailabilities = await _context.DoctorAvailabilities.Where(x => x.DoctorId == id)
            //                                                              .Select(x => new DoctorAvailabilityDto
            //                                                              {
            //                                                                  DoctorId = x.DoctorId,
            //                                                                  DayOfWeek = x.DayOfWeek,
            //                                                                  StartTime = x.StartTime,
            //                                                                  EndTime = x.EndTime,
            //                                                                  ExaminationDuration = x.ExaminationDuration,
            //                                                              })
            //                                                              .ToListAsync();

            //var doctorSchedule = new DoctorDetailsDto
            //{
            //    Id = doctor.Id,
            //    Name = doctor.Name,
            //    Specialization = doctor.Specialization,
            //    Rating = doctor.Rating,
            //    Availabilities = doctorAvailabilities
            //};

            //return Ok(doctorSchedule);
            #endregion

            #region With AutoMapper

            var doctor = await _context.Doctors.Include(d => d.Availabilities).FirstOrDefaultAsync(d => d.Id == id);

            var mappedDoctor = _mapper.Map<DoctorDetailsDto>(doctor);

            return Ok(mappedDoctor);

            #endregion
        }
    }
}
