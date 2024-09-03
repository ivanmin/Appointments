using Appointments.Domain.Dtos;

namespace Appointments.Application.Dtos.Responses
{
    public class ScheduleResponse
    {
        public Guid FacilityId { get; set; }
        public List<Slot>? FreeSlots { get; set; }
    }
}

