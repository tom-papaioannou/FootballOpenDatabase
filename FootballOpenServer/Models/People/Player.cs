// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using FootballOpenServer.Models.Teams;
using System.Text.Json.Serialization;

namespace FootballOpenServer.Models.People
{
    public class Player
    {
        public Guid PlayerID { get; set; }
        public Guid PersonID { get; set; }
        public virtual Person? Person { get; set; }
        public ICollection<PlayerTrainedPosition>? PlayerTrainedPositions { get; set; }
        public ICollection<PlayerTrainedRole>? PlayerTrainedRoles { get; set; }
        public virtual PlayerStats? PlayerStats { get; set; }
        [JsonIgnore]
        public ICollection<PlayerTactic>? PlayerTactics { get; set; }
    }
}
