using AppointmentSystem.Service.PaitentServices;
using AppointmentSystem.Service.PaitentServices.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientById(int patientId)
        {
            var patient = await _patientService.GetByIdAsync(patientId);
            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> AddPatient(CreatePatientDto patientDto)
        {
            await _patientService.AddAsync(patientDto);

            return Ok(patientDto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePatient(UpdatePatientDto patientDto)
        {
            await _patientService.UpdateAsync(patientDto);
            return Ok(patientDto);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePatient(int id)
        {
            await _patientService.DeleteAsync(id);
            return Ok(new { Message = $"Patient With Id ({id}) Deleted Successfully" });
        }


    }
}
