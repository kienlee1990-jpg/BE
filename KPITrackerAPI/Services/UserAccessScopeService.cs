using System.Security.Claims;
using KPITrackerAPI.Data;
using KPITrackerAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KPITrackerAPI.Services
{
    public class UserAccessScopeService : IUserAccessScopeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private bool _loaded;
        private long? _ownerScopeDonViId;

        public UserAccessScopeService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<long?> GetCurrentOwnerScopeDonViIdAsync()
        {
            if (_loaded)
            {
                return _ownerScopeDonViId;
            }

            _loaded = true;

            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            var roles = principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList();
            var email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            if (roles.Contains("Admin") || string.Equals(email, "admin@example.com", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var user = await _context.Users
                .AsNoTracking()
                .Include(x => x.DonVi)
                .FirstOrDefaultAsync(x => x.Id == userId);

            var loaiDonVi = (user?.DonVi?.LoaiDonVi ?? string.Empty).Trim().ToUpperInvariant();
            if (user?.DonViId is > 0 && (loaiDonVi == "THANH_PHO" || loaiDonVi == "CAP_QUAN_LY"))
            {
                _ownerScopeDonViId = user.DonViId;
            }

            return _ownerScopeDonViId;
        }
    }
}
