using Azure.Core.Pipeline;

namespace FootballOpenDatabase.Models.Teams
{
    public class Contract
    {
        public Guid ContractID { get; set; }

        public Guid PersonID { get; set; }

        public Guid TeamID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
