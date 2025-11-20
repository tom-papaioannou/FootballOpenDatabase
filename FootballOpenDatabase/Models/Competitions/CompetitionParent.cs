namespace FootballOpenDatabase.Models.Competitions
{
    public class CompetitionParent
    {
        public Guid CompetitionParentID { get; set; }

        public string? Name { get; set; }

        public CompetitionParentType CompetitionParentType { get; set; }

        public int NumberOfLeagues { get; set; }

        public int NumberOfCups { get; set; }

        public int? NumberOfNationalLeagues { get; set; } // only for Global and Continental

        public int? NumberOfNationalCups { get; set; } // only for Global and Continental

        public Guid? NationalTeamID { get; set; }
    }
}
