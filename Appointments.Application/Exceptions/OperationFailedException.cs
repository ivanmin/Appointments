namespace Appointments.Application.Exceptions
{
    public class OperationFailedException : HttpException
    {
        public OperationFailedException(string message)
            : base(message, 500) { }
    }
}

