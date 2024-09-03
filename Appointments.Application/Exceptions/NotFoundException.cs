namespace Appointments.Application.Exceptions
{
    public class NotFoundException : HttpException
    {
        public NotFoundException(string message)
            : base(message, 404) { }

        public NotFoundException(string entityName, object? key)
            : base($"Entity \"{entityName}\" ({key}) was not found.", 404) { }
    }
}

