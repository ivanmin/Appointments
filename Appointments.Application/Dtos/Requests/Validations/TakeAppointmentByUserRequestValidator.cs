using FluentValidation;

namespace Appointments.Application.Dtos.Requests.Validations
{
    public class TakeAppointmentByUserRequestValidator : AbstractValidator<TakeAppointmentByUserRequest>
    {
        public TakeAppointmentByUserRequestValidator()
        {
            RuleFor(x => x).NotNull().WithMessage("The appointment data is not valid.");
        }
    }
}

