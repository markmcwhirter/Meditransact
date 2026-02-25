namespace MediTransact.Application.Errors;

public enum ErrorKind
{
    NotFound,
    Validation,
    Conflict,
    Unauthorized,
    Unexpected
}

public sealed record AppError(ErrorKind Kind, string Code, string Message);
