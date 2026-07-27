// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using SoccerOpenServer.Models.Teams;

namespace SoccerOpenServer.DTO.Teams
{
    public class TeamInformationDTO
    {
        public Guid TeamID { get; set; }
        public string? Name { get; set; }
        public Guid? LeagueID { get; set; }
        public string? LeagueName { get; set; }
        public Guid? ManagerID { get; set; }
        public string? ManagerName { get; set; }
        public bool IsOwned { get; set; }
        public StadiumDTO? Stadium { get; set; }
        public Kit? Kit { get; set; }
    }
}
