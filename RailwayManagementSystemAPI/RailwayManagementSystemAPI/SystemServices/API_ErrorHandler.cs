namespace RailwayManagementSystemAPI.SystemServices
{
    public class API_ErrorHandler
    {
        public static Stack<Error?> Api_Errors { get; set; } = new Stack<Error?>();
        public static Error? GetLastError()
        {
            Api_Errors.TryPop(out Error? error);
            return error;
        }
        public static void AddError(Error? internal_error)
        {
            Api_Errors.Push(internal_error);
        }
    }
}
