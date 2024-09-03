using Appointments.Application.Configurations;
using Appointments.Application.Dtos.Requests;
using Appointments.Application.Dtos.Responses;
using Appointments.Application.Exceptions;
using Appointments.Application.ExternalServices.Interfaces;
using Appointments.Application.Helpers;
using Appointments.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Appointments.Application.Services.Implementations
{
	public class AppointmentService : IAppointmentService
    {
        private readonly ILogger<IAppointmentService> _logger;
        private readonly ISlotExternalService _slotServiceRemote;
        private readonly AppointmentSettings _appointmentSettings;

        public AppointmentService(ILogger<IAppointmentService> logger, ISlotExternalService slotServiceRemote, IOptions<AppointmentSettings> appointmentSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _slotServiceRemote = slotServiceRemote ?? throw new ArgumentNullException(nameof(slotServiceRemote));
            _appointmentSettings = appointmentSettings.Value ?? throw new ArgumentNullException(nameof(appointmentSettings));
        }

        public async Task<ScheduleResponse> GetWeeklyFreeSlots(DateTime desiredDate)
        {
            try
            {
                if (desiredDate < DateTime.Now ||
                    desiredDate > DateTime.Now.AddMonths(_appointmentSettings.MaxMonthsForAnAppointment))
                {
                    throw new ArgumentOutOfRangeException(nameof(desiredDate));
                }

                DateTime mondayOfWeek = desiredDate.AddDays(-(int)desiredDate.DayOfWeek + 1);
                var schedule = await _slotServiceRemote.GetWeeklyAvailability(mondayOfWeek.ToString("yyyyMMdd"));

                if (schedule == null)
                {
                    throw new NotFoundException("Appointments not found", schedule);
                }

                return ScheduleHelper.GetWeeklyFreeSlotsFromSchedule(schedule, mondayOfWeek);
            }
            catch (ExternalServiceException externalServiceException)
            {
                _logger.LogError(externalServiceException, "Error while processing request from GetWeeklyFreeSlots");
                throw new ExternalServiceException(externalServiceException.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while processing request from GetWeeklyFreeSlots.");
                throw;
            }
        }

        public async Task TakeAppointmentByUser(TakeAppointmentByUserRequest takeAppointmentByUserRequest)
        {
            try
            {
                if (takeAppointmentByUserRequest == null)
                {
                    throw new ArgumentNullException(nameof(takeAppointmentByUserRequest));
                }

                var appointment = AppointmentHelper.MapTakeAppointmentByUserRequestToAppointment(takeAppointmentByUserRequest);

                if (appointment == null)
                {
                    throw new InvalidOperationException("Mapping from TakeAppointmentByUserRequest to Appointment failed due to an invalid state.");
                }

                var response = await _slotServiceRemote.TakeSlot(appointment);

                if (response == false)
                {
                    throw new OperationFailedException("The external service failed to update the record as expected.");
                }
            }
            catch (ExternalServiceException externalServiceException)
            {
                _logger.LogError(externalServiceException, "Error while processing request from TakeSlotByUser");
                throw new ExternalServiceException(externalServiceException.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while processing request from TakeSlotByUser");
                throw;
            }
        }
    }
}

