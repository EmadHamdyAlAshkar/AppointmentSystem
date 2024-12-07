using AppointmentSystem.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Service.AvailabilityServices.Dtos
{
    public class DoctorAvailabilityProfile : Profile
    {
        public DoctorAvailabilityProfile()
        {
            CreateMap<DoctorAvailability, DoctorAvailabilityDto>();
        }
    }
}
