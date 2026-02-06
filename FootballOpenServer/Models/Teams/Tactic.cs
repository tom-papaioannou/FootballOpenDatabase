using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballOpenServer.Models.Teams
{
    public class Tactic
    {
        public Guid TacticID { get; set; }
        public Guid TeamID {  get; set; }

        [ForeignKey("TeamID")]
        public virtual Team? Team { get; set; }

        [StringLength(100, MinimumLength = 1)]
        public string Name {  get; set; } = string.Empty;

        public bool isMain {  get; set; }

        public Formation Formation {  get; set; }
    }
}
