// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using FootballOpenServer.Models.Servers;
using FootballOpenServer.Models.Teams;
using FootballOpenServer.Models.World;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballOpenServer.Models.Competitions
{
    public class Competition
    {
        public Guid CompetitionID { get; set; }
        public string? CompetitionName { get; set; }
        public Guid? NationID { get; set; }
        [ForeignKey("NationID")]
        public virtual Nation? Nation { get; set; }
        public Guid? ContinentID { get; set; }
        [ForeignKey("ContinentID")]
        public virtual Continent? Continent { get; set; }
        public CompetitionTeamsType CompetitionTeamsType { get; set; }
        public int Priority { get; set; } // Lower number means higher League/Cup in the hierarchy
        public CompetitionType CompetitionType { get; set; }
        public ICollection<Team>? Teams { get; set; }
        public Guid? ServerID { get; set; }
        [JsonIgnore]
        [ForeignKey("ServerID")]
        public virtual Server? Server { get; set; }
    }
}
