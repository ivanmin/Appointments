using Appointments.Application.Dtos.Requests;
using Appointments.Application.Dtos.Responses;

namespace Appointments.Application.Services.Interfaces
{
	public interface IAppointmentService
	{
        Task<ScheduleResponse> GetWeeklyFreeSlots(DateTime desiredDate);
        Task TakeAppointmentByUser(TakeAppointmentByUserRequest appointment);
    }
}

