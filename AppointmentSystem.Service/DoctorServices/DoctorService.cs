using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Repository.Interfaces;
using AppointmentSystem.Repository.Repositories;
using AppointmentSystem.Service.DoctorServices.Dtos;
using AppointmentSystem.Service.PaitentServices.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Service.DoctorServices
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public DoctorService(IDoctorRepository doctorRepository,
                             IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(CreateDoctorDto doctorDto)
        {
            var doctor = _mapper.Map<Doctor>(doctorDto);

            await _doctorRepository.AddAsync(doctor);
        }

        public async Task DeleteAsync(int id)
        {
            await _doctorRepository.DeleteAsync(id);
        }

        public async Task<IReadOnlyList<DoctorDetailsDto>> GetAllAsync()
        {
            var doctors = await _doctorRepository.GetAllAsync();

            var mappedDoctors = _mapper.Map<IReadOnlyList<DoctorDetailsDto>>(doctors);
            return mappedDoctors;
        }

        public async Task<DoctorDetailsDto> GetByIdAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);

            var mappedDoctor = _mapper.Map<DoctorDetailsDto>(doctor);

            return mappedDoctor;
        }

        public async Task UpdateAsync(UpdateDoctorDto doctorDto)
        {
            var doctor = _mapper.Map<Doctor>(doctorDto);
            await _doctorRepository.UpdateAsync(doctor);
        }
    }
}
