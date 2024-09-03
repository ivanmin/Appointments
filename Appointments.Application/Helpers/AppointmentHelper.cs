using Appointments.Application.Dtos.Requests;
using Appointments.Domain.Dtos;

namespace Appointments.Application.Helpers
{
    internal static class AppointmentHelper
    {
        internal static Appointment MapTakeAppointmentByUserRequestToAppointment(TakeAppointmentByUserRequest appointmentRequest)
        {
            return new Appointment
            {
                FacilityId = appointmentRequest.FacilityId,
                Start = appointmentRequest.Start.ToString("yyyy-MM-dd HH:mm:ss"),
                End = appointmentRequest.End.ToString("yyyy-MM-dd HH:mm:ss"),
                Patient = appointmentRequest.Patient,
                Comments = appointmentRequest.Comments
            };
        }
    }
}

