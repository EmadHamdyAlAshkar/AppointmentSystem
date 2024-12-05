using AppointmentSystem.Service.DoctorServices.Dtos;
using AppointmentSystem.Service.PaitentServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Service.DoctorServices
{
    public interface IDoctorService
    {
        Task<DoctorDetailsDto> GetByIdAsync(int id);
        Task<IReadOnlyList<DoctorDetailsDto>> GetAllAsync();
        Task AddAsync(CreateDoctorDto doctor);
        Task UpdateAsync(UpdateDoctorDto doctor);
        Task DeleteAsync(int id);
    }
}
