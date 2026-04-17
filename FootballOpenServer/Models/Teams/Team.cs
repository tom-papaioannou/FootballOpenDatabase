// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.Users;
using System.Text.Json.Serialization;

namespace FootballOpenServer.Models.Teams
{
    public class Team
    {
        public Guid TeamID { get; set; }

        public string? Name { get; set; }

        public Guid? AppUserID { get; set; }

        [JsonIgnore]
        public virtual AppUser? AppUser { get; set; }

        [JsonIgnore]
        public ICollection<Competition>? Competitions { get; set; }
        [JsonIgnore]
        public ICollection<Contract>? Contracts { get; set; }

        public string Code { get; set; }
    }
}
