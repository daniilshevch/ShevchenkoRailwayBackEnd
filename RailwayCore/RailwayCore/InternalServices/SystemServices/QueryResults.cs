using System.Diagnostics.CodeAnalysis;

public enum ErrorType
{
    General, // Загальна помилка невідомого походження
    BadRequest, //400
    Unauthorized, //401
    Forbidden, //403
    NotFound, //404
    MethodNotAllowed, //405
    Conflict, //409
    InternalServerError, //500
    NoError //помилки нема
}
public class SuccessMessage
{
    public string Message { get; set; }
    public Type? Service_Type { get; set; }
    public SuccessMessage(string message, Type? service_type = null)
    {
        Message = message;
        Service_Type = service_type;
    }
}
public class Error
{
    public ErrorType Type { get; set; }
    public string Message { get; set; }
    public Type? Service_Type { get; set; }
    public Error(ErrorType type, string message, Type? service_type = null)
    {
        Type = type;
        Message = message;
        Service_Type = service_type;
    }
}
public abstract class QueryResult<T> 
{
    public T? Value { get; set; }
    [MemberNotNullWhen(false, nameof(Value))] //Цей атрибут показує системі, що якщо Fail = false, то Value != null
    public bool Fail { get; set; }
    public SuccessMessage Success_Message { get; set; } = new SuccessMessage(string.Empty, null);
    public Error Error { get; set; } = new Error(ErrorType.NoError, string.Empty, null);
}
public abstract class QueryResult
{
    public bool Fail { get; set; }
    public SuccessMessage Success_Message { get; set; } = new SuccessMessage(string.Empty, null);
    public Error Error { get; set; } = new Error(ErrorType.NoError, string.Empty, null);
}
public class SuccessQuery<T> : QueryResult<T>
{
    public SuccessQuery(T value, SuccessMessage? success_message = null) : base()
    {
        Fail = false;
        if(value == null)
        {
            throw new ArgumentNullException("Value in SuccessQuery can't be null(Internal Error)");
        }
        Value = value;
        if(success_message is not null)
        {
            Success_Message = success_message;
        }
        Error = new Error(ErrorType.NoError, string.Empty, null);
    }
}
public class SuccessQuery: QueryResult
{
    public SuccessQuery(SuccessMessage? success_message = null)
    {
        Fail = false;
        if(success_message is not null)
        {
            Success_Message = success_message;
        }
        Error = new Error(ErrorType.NoError, string.Empty, null);
    }
}
public class FailQuery<T> : QueryResult<T>
{
    public FailQuery(Error error) : base()
    {
        Fail = true;
        Value = default;
        Error = error;
    }
}
public class FailQuery: QueryResult
{
    public FailQuery(Error error)
    {
        Fail = true;
        Error = error;
    }
}
