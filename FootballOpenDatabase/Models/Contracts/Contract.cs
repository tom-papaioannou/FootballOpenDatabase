namespace FootballOpenDatabase.Models.Contracts
{
    public class Contract
    {
        public Guid ContractID { get; set; }

        public Guid PersonID { get; set; }

        public Guid TeamID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Role Role { get; set; }
    }
}
