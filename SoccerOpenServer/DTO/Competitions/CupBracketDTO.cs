// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using SoccerOpenServer.Models.Competitions;

namespace SoccerOpenServer.DTO.Competitions
{
    public class CupBracketDTO
    {
        public Guid CompetitionID { get; set; }
        public List<CupBracketRoundDTO> Rounds { get; set; } = new();
    }

    public class CupBracketRoundDTO
    {
        public Guid CupRoundID { get; set; }
        public int RoundNumber { get; set; }
        public int TeamCount { get; set; }
        public CupRoundType RoundType { get; set; }
        public List<CupBracketTieDTO> Ties { get; set; } = new();
    }

    public class CupBracketTieDTO
    {
        public Guid CupTieID { get; set; }
        public Guid CupRoundID { get; set; }
        public int TieNumber { get; set; }
        public Guid? HomeTeamID { get; set; }
        public Guid? AwayTeamID { get; set; }
        public Guid? WinnerTeamID { get; set; }
        public Guid? NextCupTieID { get; set; }
        public bool AdvancesAsHomeTeam { get; set; }
        public bool IsCompleted { get; set; }
        public CupBracketTeamDTO? HomeTeam { get; set; }
        public CupBracketTeamDTO? AwayTeam { get; set; }
        public CupBracketTeamDTO? WinnerTeam { get; set; }
    }

    public class CupBracketTeamDTO
    {
        public Guid TeamID { get; set; }
        public string? Name { get; set; }
        public string? BadgeColor { get; set; }
    }
}
