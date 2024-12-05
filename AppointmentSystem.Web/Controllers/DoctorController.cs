using AppointmentSystem.Service.DoctorServices;
using AppointmentSystem.Service.DoctorServices.Dtos;
using AppointmentSystem.Service.PaitentServices.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
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
    }
}
