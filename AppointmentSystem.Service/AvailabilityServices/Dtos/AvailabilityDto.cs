using AppointmentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Service.AvailabilityServices.Dtos
{
    public class DoctorAvailabilityDto
    {
        public int DoctorId { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int ExaminationDuration { get; set; }
    }
}
