// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using FootballOpenServer.Models.Teams;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballOpenServer.Models.Competitions
{
    public class Competition
    {
        public Guid CompetitionID { get; set; }

        public string? CompetitionName { get; set; }

        public Guid ParentID { get; set; }

        [ForeignKey("ParentID")]
        public virtual CompetitionParent? CompetitionParent { get; set; }

        public CompetitionTeamsType CompetitionTeamsType { get; set; }

        public int Priority { get; set; } // Lower number means higher League/Cup in the hierarchy

        public CompetitionType CompetitionType { get; set; }

        public ICollection<Team> Teams { get; set; }
    }
}
