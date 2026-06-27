// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

namespace FootballOpenServer.Models.Competitions
{
    public class CupTie
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

        public virtual CupRound CupRound { get; set; } = null!;
    }
}
