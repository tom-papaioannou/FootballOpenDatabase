namespace SoccerOpenServer.Models.People
{
    public class ManagerGameStats
    {
        public Guid PersonID { get; set; }
        public Person Person { get; set; } = null!;

        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GamesPlayed { get; set; }
        public int LeaguesWon { get; set; }
        public int CupsWon { get; set; }
    }
}
