using System.Security.Claims;
using FootballOpenServer.Models.Teams;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenServer.Services
{
    public interface ITeamAccessService
    {
        Guid? GetCurrentUserID(ClaimsPrincipal user);
        Task<Team?> GetOwnedTeamAsync(ClaimsPrincipal user);
        Task<bool> OwnsTeamAsync(ClaimsPrincipal user, Guid teamId);
    }

    public class TeamAccessService : ITeamAccessService
    {
        private readonly FootballDbContext _db;

        public TeamAccessService(FootballDbContext db)
        {
            _db = db;
        }

        public Guid? GetCurrentUserID(ClaimsPrincipal user)
        {
            string? rawUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(rawUserId))
                return null;

            if (!Guid.TryParse(rawUserId, out Guid userId))
                return null;

            return userId;
        }

        public async Task<Team?> GetOwnedTeamAsync(ClaimsPrincipal user)
        {
            Guid? userId = GetCurrentUserID(user);
            if (userId == null)
                return null;

            return await _db.Teams.FirstOrDefaultAsync(t => t.AppUserID == userId.Value);
        }

        public async Task<bool> OwnsTeamAsync(ClaimsPrincipal user, Guid teamID)
        {
            Guid? appUserID = GetCurrentUserID(user);
            if (appUserID == null)
                return false;

            return await _db.Teams.AnyAsync(t => t.TeamID == teamID && t.AppUserID == appUserID.Value);
        }
    }
}
