using Appointments.Domain.Dtos;

namespace Appointments.Application.ExternalServices.Interfaces
{
    public interface ISlotExternalService
    {
        Task<Schedule?> GetWeeklyAvailability(string date);
        Task<bool> TakeSlot(Appointment appointment);
    }
}

