using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices
{
    public static class ErrorMessageService
    {
        public static ActionResult<T> GetError<T>(ErrorType error_type, string? message = null)
        {
            switch (error_type)
            {
                case ErrorType.BadRequest:
                    return new BadRequestObjectResult(message);
                case ErrorType.Unauthorized:
                    return new UnauthorizedObjectResult(message);
                case ErrorType.NotFound:
                    return new NotFoundObjectResult(message);
                case ErrorType.Conflict:
                    return new ConflictObjectResult(message);
                case ErrorType.Forbidden:
                    return new ForbidResult(message); //?
                default:
                    return new ObjectResult(message)
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                    };
            }
        }
        public static ActionResult GetErrorFromNonGeneric(ErrorType error_type, string? message = null)
        {
            switch (error_type)
            {
                case ErrorType.BadRequest:
                    return new BadRequestObjectResult(message);
                case ErrorType.Unauthorized:
                    return new UnauthorizedObjectResult(message);
                case ErrorType.NotFound:
                    return new NotFoundObjectResult(message);
                case ErrorType.Conflict:
                    return new ConflictObjectResult(message);
                case ErrorType.Forbidden:
                    return new ForbidResult(message); //?
                default:
                    return new ObjectResult(message)
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                    };
            }
        }
        public static ActionResult<OutputModel> GetErrorFromQueryResult<InternalModel, OutputModel>(this QueryResult<InternalModel> query_result)
        {
            return GetError<OutputModel>(query_result.Error.Type, query_result.Error.Message);
        }
        public static ActionResult GetErrorFromNonGenericQueryResult(this QueryResult query_result)
        {
            return GetErrorFromNonGeneric(query_result.Error.Type, query_result.Error.Message);
        }
        public static ActionResult<OutputModel> GetGeneralError<OutputModel>()
        {
            return new ObjectResult("Unknown origin error")
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }
    }
}
