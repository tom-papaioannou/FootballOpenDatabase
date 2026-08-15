// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoccerOpenServer.Models.People
{
    public class MedicStats
    {
        public Guid MedicStatsID { get; set; }
        public Guid PersonID { get; set; }

        [JsonIgnore]
        public virtual Person Person { get; set; } = null!;

        [Range(1, 100)]
        public byte Diagnosis { get; set; }

        [Range(1, 100)]
        public byte Treatment { get; set; }

        [Range(1, 100)]
        public byte Rehabilitation { get; set; }

        [Range(1, 100)]
        public byte Prevention { get; set; }
    }
}
