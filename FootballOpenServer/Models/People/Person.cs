// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.Servers;
using FootballOpenServer.Models.Teams;
using FootballOpenServer.Models.Users;
using FootballOpenServer.Models.World;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FootballOpenServer.Models.People
{
    public class Person
    {
        public Guid PersonID { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
        public AppUser? AppUser { get; set; }
        public Guid? NationID { get; set; }
        [JsonIgnore]
        public Nation? Nation { get; set; }
        public Guid? ServerID { get; set; }
        [JsonIgnore]
        [ForeignKey("ServerID")]
        public virtual Server? Server { get; set; }

        public ICollection<PlayerTrainedPosition>? PlayerTrainedPositions { get; set; }
        public ICollection<PlayerTrainedRole>? PlayerTrainedRoles { get; set; }
        public virtual PlayerStats? PlayerStats { get; set; }
        [JsonIgnore]
        public ICollection<PlayerTactic>? PlayerTactics { get; set; }
        public StaffRole? StaffRole { get; set; }
    }
}
