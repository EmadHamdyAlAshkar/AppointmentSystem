﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Service.DoctorServices.Dtos
{
    public class CreateDoctorDto
    {
        public string Name { get; set; }
        public string Specialization { get; set; }
        public double Rating { get; set; }
    }
}
