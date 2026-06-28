// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using SoccerOpenServer.Models.Competitions;
using SoccerOpenServer.Models.Teams;

namespace SoccerOpenServer.DTO.Teams
{
    public class TeamCompetitionsDTO
    {
        public Guid TeamID { get; set; }
        public List<Competition>? Competitions { get; set; }
    }
}
