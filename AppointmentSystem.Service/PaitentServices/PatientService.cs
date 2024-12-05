using AppointmentSystem.Domain.Entities;
using AppointmentSystem.Repository.Interfaces;
using AppointmentSystem.Service.PaitentServices.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Service.PaitentServices
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public PatientService(IPatientRepository patientRepository,
                              IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }
        public async Task AddAsync(CreatePatientDto patientDto)
        {
            var patient = _mapper.Map<Patient>(patientDto);

            await _patientRepository.AddAsync(patient);
        }

        public async Task DeleteAsync(int id)
        {
            await _patientRepository.DeleteAsync(id);
        }

        public async Task<IReadOnlyList<PatientDetailsDto>> GetAllAsync()
        {
            var patients = await _patientRepository.GetAllAsync();

            var mappedPatients =  _mapper.Map<IReadOnlyList<PatientDetailsDto>>(patients);

            return mappedPatients;
        }

        public async Task<PatientDetailsDto> GetByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);

            var mappedPatient = _mapper.Map<PatientDetailsDto>(patient);

            return mappedPatient;
        }

        public async Task UpdateAsync(UpdatePatientDto patientDto)
        {
            var patient = _mapper.Map<Patient>(patientDto);
            await _patientRepository.UpdateAsync(patient);
        }
    }
}
