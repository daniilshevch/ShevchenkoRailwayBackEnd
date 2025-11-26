using RailwayCore.InternalServices.SystemServices;
using System.Diagnostics.CodeAnalysis;
public enum ProgramUnit
{
    Core,
    ClientAPI,
    AdminAPI,
    SystemAPI
}

public enum ErrorType
{
    General, // Загальна помилка невідомого походження
    BadRequest, //400
    Unauthorized, //401
    Forbidden, //403
    NotFound, //404
    Conflict, //409
    InternalServerError, //500
    NoError, //помилки нема
    ServiceUnrecheable //503
}
public class Error
{
    public ErrorType Type { get; set; }
    public string Message { get; set; }
    public Error(ErrorType type, string message, string? annotation = null, ProgramUnit? unit = null)
    {
        ConsoleLogService.PrintUnit(unit);
        ConsoleLogService.PrintAnnotationForFailQuery(annotation);
        ConsoleLogService.PrintMessage(message);
        Type = type;
        Message = message;
    }
}
public class SuccessMessage
{
    public string Text { get; set; }
    public SuccessMessage(string text, string? annotation = null, ProgramUnit? unit = null)
    {
        ConsoleLogService.PrintUnit(unit);
        ConsoleLogService.PrintAnnotationForSuccessQuery(annotation);
        ConsoleLogService.PrintMessage(text);
        Text = text;
    }
}
public abstract class QueryResult<T>
{
    public T? Value { get; set; }
    [MemberNotNullWhen(false, nameof(Value))] //Цей атрибут показує системі, що якщо Fail = false, то Value != null
    public bool Fail { get; set; }
    public SuccessMessage Success_Message { get; set; } = new SuccessMessage(string.Empty);
    public Error Error { get; set; } = new Error(ErrorType.NoError, string.Empty);
}
public abstract class QueryResult
{
    public bool Fail { get; set; }
    public SuccessMessage Success_Message { get; set; } = new SuccessMessage(string.Empty);
    public Error Error { get; set; } = new Error(ErrorType.NoError, string.Empty);
}
public class SuccessQuery<T> : QueryResult<T>
{
    public SuccessQuery(T value, SuccessMessage? success_message = null) : base()
    {
        Fail = false;
        if (value == null)
        {
            throw new ArgumentNullException("Value in success query can't be null(Internal Error)");
        }
        Value = value;
        if(success_message != null)
        {
            Success_Message = success_message;
        }
        Error = new Error(ErrorType.NoError, string.Empty);
    }
}
public class SuccessQuery : QueryResult
{
    public SuccessQuery(SuccessMessage? success_message = null)
    {
        Fail = false;
        if (success_message != null)
        {
            Success_Message = success_message;
        }
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
public class FailQuery : QueryResult
{
    public FailQuery(Error error)
    {
        Fail = true;
        Error = error;
    }
}