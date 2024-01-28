namespace HallOfFameWebApi.Services.Exceptions
{
    public class PersonNotFoundException : PersonException
    {
        private readonly long _personId;

        public PersonNotFoundException() { }

        public PersonNotFoundException(string? message) : base(message) { }

        public PersonNotFoundException(string? message, Exception? innerException)
            : base(message, innerException) { }

        public PersonNotFoundException(long personId)
            : base($"Person not found, id: {personId}")
        {
            _personId = personId;
        }

        public override void WriteToLog(ILogger logger, LogLevel logLevel)
        {
            logger.Log(logLevel, "Person not found, id: {PersonId}", _personId);
        }
    }
}
