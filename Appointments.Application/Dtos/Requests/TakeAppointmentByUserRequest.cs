using Appointments.Domain.Dtos;

namespace Appointments.Application.Dtos.Requests
{
    public class TakeAppointmentByUserRequest
    {
        public required Guid FacilityId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Comments { get; set; } = string.Empty;
        public required Patient Patient { get; set; }
    }
}

