namespace MediTransact.WebApi.Contracts;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string AccessToken, string TokenType, int ExpiresInSeconds);
