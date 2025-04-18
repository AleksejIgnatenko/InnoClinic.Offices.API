namespace InnoClinic.Offices.Core.Exceptions
{
    public class ExceptionWithStatusCode : Exception
    {
        public int HttpStatusCode { get; }
        public ExceptionWithStatusCode(string message, int httpStatusCode) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
