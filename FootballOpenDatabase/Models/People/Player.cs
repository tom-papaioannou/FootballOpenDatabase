namespace FootballOpenDatabase.Models.People
{
    public class Player
    {
        public Guid PlayerID { get; set; }
        public Guid PersonID { get; set; }
        public virtual Person? Person { get; set; }
    }
}
