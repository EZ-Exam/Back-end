namespace teamseven.EzExam.Services.Services.JwtHelperService
{
    public interface IJwtHelperService
    {
        int? GetCurrentUserIdFromToken(string authHeader);
        string? GetCurrentUserEmailFromToken(string authHeader);
        int? GetCurrentUserRoleIdFromToken(string authHeader);
        string? GetCurrentUserBalanceFromToken(string authHeader);
        string? GetCurrentUserSubscriptionTypeFromToken(string authHeader);
        string? GetCurrentUserSubscriptionCodeFromToken(string authHeader);
        string? GetCurrentUserSubscriptionNameFromToken(string authHeader);
        string? GetCurrentUserSubscriptionEndDateFromToken(string authHeader);
        bool? GetCurrentUserSubscriptionIsActiveFromToken(string authHeader);
        bool IsTokenValid(string token);
    }
}
