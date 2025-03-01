using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Prod.Exceptions;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.ExceptionHandled = true;

        switch (context.Exception)
        {
            // Unauthorized
            case UnauthorizedException:
                context.Result = new UnauthorizedResult();
                break;

            // Conflicts
            case DbUpdateException
            {
                InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation }
            }:
                context.Result = new ConflictResult();
                break;

            // Not found (on SingleAsync)
            case InvalidOperationException when context.Exception.Message.Contains("Sequence contains no elements"):
                context.Result = new NotFoundResult();
                break;

            case ForbiddenException:
                context.Result = new ObjectResult(new { Reason = context.Exception.Message }) { StatusCode = 403 };
                break;

            case NotFoundException:
                context.Result = new ObjectResult(new { Reason = context.Exception.Message }) { StatusCode = 404 };
                break;
            
            default:
                context.ExceptionHandled = false;
                break;
        }
    }
}
