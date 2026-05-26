// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System.Security.Claims;
using FootballOpenServer.DTO.Teams;
using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Teams;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenServer.Services
{
    public interface ITeamAccessService
    {
        Guid? GetCurrentUserID(ClaimsPrincipal user);
        Task<TeamInformationDTO?> GetOwnedTeamAsync(ClaimsPrincipal user);
        Task<TeamCompetitionsDTO?> GetOwnedTeamCompetitionsAsync(ClaimsPrincipal user);
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

        public async Task<TeamInformationDTO?> GetOwnedTeamAsync(ClaimsPrincipal user)
        {
            Guid? userId = GetCurrentUserID(user);
            if (userId == null)
                return null;

            return await _db.Teams
                .Where(t => t.AppUserID == userId.Value)
                .AsNoTracking()
                .Include(t => t.Stadium)
                .Select(t => new TeamInformationDTO
                {
                    TeamID = t.TeamID,
                    Name = t.Name,
                    LeagueID = t.Competitions!
                        .Where(c => c.CompetitionType == CompetitionType.League)
                        .OrderBy(c => c.Priority)
                        .Select(c => (Guid?)c.CompetitionID)
                        .FirstOrDefault(),
                    LeagueName = t.Competitions!
                        .Where(c => c.CompetitionType == CompetitionType.League)
                        .OrderBy(c => c.Priority)
                        .Select(c => c.CompetitionName)
                        .FirstOrDefault(),
                    Stadium = new StadiumDTO
                    {
                        StadiumID = t.Stadium.StadiumID,
                        Name = t.Stadium.Name,
                        Capacity = t.Stadium.Capacity,
                        City = t.Stadium.City,
                        Latitude = t.Stadium.Latitude,
                        Longitude = t.Stadium.Longitude
                    },
                    Kit = t.Kit
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TeamCompetitionsDTO?> GetOwnedTeamCompetitionsAsync(ClaimsPrincipal user)
        {
            Guid? userId = GetCurrentUserID(user);
            if (userId == null)
                return null;

            return await _db.Teams
                .Where(t => t.AppUserID == userId.Value)
                .AsNoTracking()
                .Include(t => t.Competitions).ThenInclude(c => c.Nation)
                .Include(t => t.Competitions).ThenInclude(c => c.Continent)
                .Select(t => new TeamCompetitionsDTO
                {
                    TeamID = t.TeamID,
                    Competitions = t.Competitions != null ? t.Competitions.ToList() : new List<Competition>(),
                })
                .FirstOrDefaultAsync();
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
