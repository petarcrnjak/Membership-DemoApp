namespace Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}