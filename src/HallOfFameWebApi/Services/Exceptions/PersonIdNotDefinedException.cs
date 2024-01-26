namespace HallOfFameWebApi.Services.Exceptions
{
    public class PersonIdNotDefinedException : PersonException
    {
        public PersonIdNotDefinedException()
            : base("Person id not defined") { }

        public PersonIdNotDefinedException(string? message) : base(message) { }

        public PersonIdNotDefinedException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
