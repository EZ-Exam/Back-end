namespace teamseven.EzExam.Services.Services.JwtHelperService
{
    public interface IJwtHelperService
    {
        int? GetCurrentUserIdFromToken(string authHeader);
        string? GetCurrentUserEmailFromToken(string authHeader);
        int? GetCurrentUserRoleIdFromToken(string authHeader);
        bool IsTokenValid(string token);
    }
}
