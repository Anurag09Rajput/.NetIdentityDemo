using System.Net;

namespace Application.AngularIdentity.Concerns
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
        public Exception Exception { get; set; }
    }

    public class Response<T> : Response
    {
        public T? Data { get; set; }
    }
}
