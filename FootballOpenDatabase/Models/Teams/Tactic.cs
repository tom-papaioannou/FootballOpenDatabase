using System.ComponentModel.DataAnnotations.Schema;

namespace FootballOpenDatabase.Models.Teams
{
    public class Tactic
    {
        public Guid TacticID { get; set; }
        public Guid TeamID {  get; set; }

        [ForeignKey("TeamID")]
        public virtual Team? Team { get; set; }

        public bool isMain {  get; set; }
    }
}
