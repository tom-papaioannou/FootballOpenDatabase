// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoccerOpenServer.Models.People
{
    public class CoachStats
    {
        public Guid CoachStatsID { get; set; }
        public Guid PersonID { get; set; }
        [JsonIgnore]
        public virtual Person Person { get; set; } = null!;

        [Range(1, 100)]
        public byte Attack { get; set; }
        [Range(1, 100)]
        public byte Defend { get; set; }
        [Range(1, 100)]
        public byte Control { get; set; }
        [Range(1, 100)]
        public byte Goalkeeper { get; set; }
        [Range(1, 100)]
        public byte Tactic { get; set; }
        [Range(1, 100)]
        public byte Fitness { get; set; }
    }
}
