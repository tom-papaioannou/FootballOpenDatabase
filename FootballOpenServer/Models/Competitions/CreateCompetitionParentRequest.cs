// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

namespace FootballOpenServer.Models.Competitions
{
    public class CreateCompetitionParentRequest
    {
        public string? Name { get; set; }
        public CompetitionParentType CompetitionParentType { get; set; }
        public int NumberOfLeagues { get; set; }
        public int NumberOfCups { get; set; }
        public int? NumberOfNationalLeagues { get; set; }
        public int? NumberOfNationalCups { get; set; }
        public Guid? NationalTeamID { get; set; }
    }
}
