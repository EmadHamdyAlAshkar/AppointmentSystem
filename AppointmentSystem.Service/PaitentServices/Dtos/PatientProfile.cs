using AppointmentSystem.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Service.PaitentServices.Dtos
{
    public class PatientProfile : Profile
    {
        public PatientProfile() 
        {
            CreateMap<Patient, PatientDetailsDto>();
            CreateMap<Patient, CreatePatientDto>().ReverseMap();
            CreateMap<Patient, UpdatePatientDto>().ReverseMap();
        }
    }
}
