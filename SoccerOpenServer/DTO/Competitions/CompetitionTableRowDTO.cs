// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

namespace SoccerOpenServer.DTO.Competitions
{
    public class CompetitionTableRowDTO
    {
        public int Position { get; set; }
        public Guid TeamID { get; set; }
        public string? TeamName { get; set; }
        public int Points { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int MatchesPlayed { get; set; }
    }
}
