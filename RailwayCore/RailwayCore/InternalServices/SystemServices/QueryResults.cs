using System.Diagnostics.CodeAnalysis;

public enum ErrorType
{
    General, // Загальна помилка невідомого походження
    BadRequest, //400
    Unauthorized, //401
    Forbidden, //403
    NotFound, //404
    Conflict, //409
    InternalServerError, //500
    NoError //помилки нема
}
public class Error
{
    public ErrorType Type { get; set; }
    public string Message { get; set; }
    public Error(ErrorType type, string message)
    {
        Type = type;
        Message = message;
    }
}
public abstract class QueryResult<T>
{
    public T? Value { get; set; }
    [MemberNotNullWhen(false, nameof(Value))] //Цей атрибут показує системі, що якщо Fail = false, то Value != null
    public bool Fail { get; set; }
    public Error Error { get; set; } = new Error(ErrorType.NoError, string.Empty);
}
public abstract class QueryResult
{
    public bool Fail { get; set; }
    public Error Error { get; set; } = new Error(ErrorType.NoError, string.Empty);
}
public class SuccessQuery<T> : QueryResult<T>
{
    public SuccessQuery(T value) : base()
    {
        Fail = false;
        if(value == null)
        {
            throw new ArgumentNullException("Value in success query can't be null(Internal Error)");
        }
        Value = value;
        Error = new Error(ErrorType.NoError, string.Empty);
    }
}
public class SuccessQuery: QueryResult
{
    public SuccessQuery()
    {
        Fail = false;
        Error = new Error(ErrorType.NoError, string.Empty);
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
