using AppointmentSystem.Service.PaitentServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Service.PaitentServices
{
    public interface IPatientService
    {
        Task<PatientDetailsDto> GetByIdAsync(int id);
        Task<IReadOnlyList<PatientDetailsDto>> GetAllAsync();
        Task AddAsync(CreatePatientDto patient);
        Task UpdateAsync(UpdatePatientDto patient);
        Task DeleteAsync(int id);
    }
}
