namespace HallOfFameWebApi.Services.Exceptions
{
    public class PersonNotFoundException : PersonException
    {
        public PersonNotFoundException() { }

        public PersonNotFoundException(string? message) : base(message) { }

        public PersonNotFoundException(string? message, Exception? innerException)
            : base(message, innerException) { }

        public PersonNotFoundException(long id)
            : base($"Person not found, id: {id}") { }
    }
}
