// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballOpenServer.Models.Teams
{
    public class Tactic
    {
        public Guid TacticID { get; set; }
        public Guid TeamID {  get; set; }

        [ForeignKey("TeamID")]
        public virtual Team? Team { get; set; }

        [StringLength(30, MinimumLength = 1)]
        public string Name {  get; set; } = string.Empty;

        public bool isMain {  get; set; }

        [Required]
        public Formation? Formation { get; set; }
    }
}
