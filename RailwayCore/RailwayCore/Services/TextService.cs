public enum ErrorType
{
    NotFound,
    BadRequest
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
namespace RailwayCore.Services
{
    public class ImportanceAttribute : Attribute
    {
        public string Description { get; set; }
        public ImportanceAttribute(string description = "")
        {
            Description = description;
        }
    }
    public class CrucialAttribute : ImportanceAttribute
    {
        public CrucialAttribute(string description = "") : base(description) { }
    }
    public class ExecutiveAttribute : ImportanceAttribute
    {
        public ExecutiveAttribute(string description = "") : base(description) { }
    }
    public class PeripheralAttribute : ImportanceAttribute
    {
        public PeripheralAttribute(string description = "") : base(description) { }
    }
    public class ArchievedAttribute : ImportanceAttribute
    {
        public ArchievedAttribute(string description = "") : base(description) { }
    }
    public class DestinationAttribute : Attribute
    {

    }
    public class InternalAttribute : DestinationAttribute { }
    public class ExternalAttribute : DestinationAttribute { }
    public class AlgorithmAttribute : Attribute
    {
        public string Algorithm { get; set; }
        public AlgorithmAttribute(string algorithm)
        {
            Algorithm = algorithm;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class StatusAttribute: Attribute
    {
        public string Version { get; set; }
        public string Date { get; set; }
        public StatusAttribute(string version, string date)
        {
            Version = version;
            Date = date;
        }
    }
    public class RefactoredAttribute: StatusAttribute
    {
        public RefactoredAttribute(string version, string date): base(version, date) { }
    }
    public class ReengineeredAttribute: StatusAttribute
    {
        public ReengineeredAttribute(string version, string date): base(version, date) { }
    }
    public class OptimizedAttribute : StatusAttribute
    {
        public OptimizedAttribute(string version, string date) : base(version, date) { }
    }
    public class QueryResult<T>
    {
        public T? Value { get; set; }
        public Error? Error { get; set; }
    }
    public class SuccessQuery<T>: QueryResult<T>
    {
        public SuccessQuery(T value)
        {
            Value = value;
            Error = null;
        }
    }
    public class FailQuery<T>: QueryResult<T>
    {
        public FailQuery(Error error)
        {
            Value = default;
            Error = error;
        }
    }
    public class TextService
    {
        private bool SuccessPostEnabled;
        private bool SuccessGetEnabled;
        private bool FailPostEnabled;
        private bool FailGetEnabled;
        private bool DuplicateEnabled;
        private string ServiceTitle;
        public TextService(string service_title, bool success_post_enabled = false, bool success_get_enabled = false,
            bool fail_post_enabled = true, bool fail_get_enabled = true, bool duplicate_enabled = true)
        {
            ServiceTitle = service_title;
            SuccessPostEnabled = success_post_enabled;
            SuccessGetEnabled = success_get_enabled;
            FailPostEnabled = fail_post_enabled;
            FailGetEnabled = fail_get_enabled;
            DuplicateEnabled = duplicate_enabled;

        }
        public void SuccessPostInform(string message)
        {
            if (SuccessPostEnabled)
            {
                Console.WriteLine($"{TextService.Green}[{ServiceTitle}]{TextService.Reset}{TextService.Orange}[POST]{TextService.Reset}" +
                    $" {message}");
            }
        }
        public void SuccessGetInform(string message)
        {
            if (SuccessGetEnabled)
            {
                Console.WriteLine($"{TextService.Green}[{ServiceTitle}]{TextService.Reset}{TextService.Orange}[GET]{TextService.Reset}" +
                    $" {message}");
            }
        }
        public void FailPostInform(string message)
        {
            if (FailPostEnabled)
            {
                Console.WriteLine($"{TextService.Red}[{ServiceTitle}]{TextService.Reset}{TextService.Orange}[POST]{TextService.Reset}" +
                    $" {message}");
            }
        }
        public void FailGetInform(string message)
        {
            if (FailGetEnabled)
            {
                Console.WriteLine($"{TextService.BrightRed}[{ServiceTitle}]{TextService.Reset}{TextService.Orange}[GET]{TextService.Reset}" +
                    $" {message}");
            }
        }
        public void DuplicateGetInform(string message)
        {
            if (DuplicateEnabled)
            {
                Console.WriteLine($"{TextService.Yellow}[{ServiceTitle}]{TextService.Reset}{TextService.Orange}[POST]{TextService.Reset}" +
                    $" {message}");
            }
        }
        // Основні кольори тексту
        public const string Black = "\u001b[30m";
        public const string Red = "\u001b[31m";
        public const string Green = "\u001b[32m";
        public const string Yellow = "\u001b[33m";
        public const string Blue = "\u001b[34m";
        public const string Magenta = "\u001b[35m";
        public const string Cyan = "\u001b[36m";
        public const string White = "\u001b[37m";
        public const string Orange = "\u001b[38;2;255;165;0m";

        // Яскраві кольори тексту
        public const string BrightBlack = "\u001b[90m";
        public const string BrightRed = "\u001b[91m";
        public const string BrightGreen = "\u001b[92m";
        public const string BrightYellow = "\u001b[93m";
        public const string BrightBlue = "\u001b[94m";
        public const string BrightMagenta = "\u001b[95m";
        public const string BrightCyan = "\u001b[96m";
        public const string BrightWhite = "\u001b[97m";

        // Кольори фону
        public const string BgBlack = "\u001b[40m";
        public const string BgRed = "\u001b[41m";
        public const string BgGreen = "\u001b[42m";
        public const string BgYellow = "\u001b[43m";
        public const string BgBlue = "\u001b[44m";
        public const string BgMagenta = "\u001b[45m";
        public const string BgCyan = "\u001b[46m";
        public const string BgWhite = "\u001b[47m";

        // Яскраві кольори фону
        public const string BgBrightBlack = "\u001b[100m";
        public const string BgBrightRed = "\u001b[101m";
        public const string BgBrightGreen = "\u001b[102m";
        public const string BgBrightYellow = "\u001b[103m";
        public const string BgBrightBlue = "\u001b[104m";
        public const string BgBrightMagenta = "\u001b[105m";
        public const string BgBrightCyan = "\u001b[106m";
        public const string BgBrightWhite = "\u001b[107m";

        // Скидання стилів
        public const string Reset = "\u001b[0m";  // Скидає всі стилі (колір, фон, ефекти)

    }

}
