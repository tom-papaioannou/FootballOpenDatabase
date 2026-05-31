// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

namespace FootballOpenServer.Models.Competitions
{
    public class CompetitionTable
    {
        public Guid CompetitionTableID { get; set; }
        public Guid CompetitionID { get; set; }
        public Guid TeamID { get; set; }
        public int Points { get; set; }
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
    }
}
