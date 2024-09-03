namespace Appointments.Application.Exceptions
{
    public class ExternalServiceException : HttpException
    {
        public ExternalServiceException(string message)
            : base(message, 502) { }
    }
}

