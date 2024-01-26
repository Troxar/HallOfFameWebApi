namespace HallOfFameWebApi.Services.Exceptions
{
    public abstract class PersonException : ApplicationException
    {
        public PersonException() { }

        public PersonException(string? message) : base(message) { }

        public PersonException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
