using Appointments.Application.Configurations;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Appointments.Application.Dtos.Requests.Validations
{
    public class GetWeeklyFreeSlotsRequestValidator : AbstractValidator<GetWeeklyFreeSlotsRequest>
    {
        private readonly AppointmentSettings _appointmentSettings;
        public GetWeeklyFreeSlotsRequestValidator(IOptions<AppointmentSettings> appointmentSettings)
        {
            _appointmentSettings = appointmentSettings.Value ?? throw new ArgumentNullException(nameof(appointmentSettings));

            RuleFor(x => x.DesiredDate)
               .GreaterThan(DateTime.Now)
               .WithMessage("The appointment desired date cannot be earlier than the current date.");
            RuleFor(x => x.DesiredDate)
               .LessThan(DateTime.Now.AddMonths(_appointmentSettings.MaxMonthsForAnAppointment))
               .WithMessage($"The appointment desired date cannot be later than {_appointmentSettings.MaxMonthsForAnAppointment} months after the current date.");
        }
    }
}

