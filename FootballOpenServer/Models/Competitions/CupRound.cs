// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

namespace FootballOpenServer.Models.Competitions
{
    public class CupRound
    {
        public Guid CupRoundID { get; set; }

        public Guid CompetitionID { get; set; }

        // 1 = first round, 2 = next round, etc.
        // With 32 teams:
        // 1 = Round of 32
        // 2 = Round of 16
        // 3 = Quarter-finals
        // 4 = Semi-finals
        // 5 = Final
        public int RoundNumber { get; set; }

        // Number of teams entering this round:
        // 32, 16, 8, 4, 2
        public int TeamCount { get; set; }

        public CupRoundType RoundType { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public virtual Competition Competition { get; set; }
        public virtual ICollection<CupTie> Ties { get; set; } = new List<CupTie>();
    }
}