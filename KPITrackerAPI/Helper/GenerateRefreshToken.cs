using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

namespace KPITrackerAPI.Helper;

public static class TokenHelper
{
    public static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return WebEncoders.Base64UrlEncode(randomBytes);
    }
}
