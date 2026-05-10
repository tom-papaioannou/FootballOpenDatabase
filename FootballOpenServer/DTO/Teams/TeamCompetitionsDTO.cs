// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Teams;

namespace FootballOpenServer.DTO.Teams
{
    public class TeamCompetitionsDTO
    {
        public Guid TeamID { get; set; }
        public List<Competition>? Competitions { get; set; }
    }
}
