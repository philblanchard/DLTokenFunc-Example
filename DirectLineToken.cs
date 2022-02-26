using System.Net;

namespace DirectLineTokenFuncProj;
public class DirectLineToken
{
    public DirectLineToken(Error error)
    {
        Error = error;
    }
    public string ConversationId { get; set; }
    public string Token { get; set; }
    public int ExpiresIn { get; set; }
    public Error Error { get; set; }
}

public class Error
{
    public Error(string message, HttpStatusCode? statusCode)
    {
        Message = message;
        StatusCode = statusCode;
    }
    public string Message { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
}