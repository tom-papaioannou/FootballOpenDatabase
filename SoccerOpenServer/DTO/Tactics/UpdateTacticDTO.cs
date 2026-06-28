// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using SoccerOpenServer.Models.Teams;
using System.ComponentModel.DataAnnotations;

namespace SoccerOpenServer.DTO.Tactics
{
    public class UpdateTacticDTO
    {
        [Required]
        [StringLength(30, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        public bool isMain { get; set; }

        [Required]
        public Formation Formation { get; set; }

        [Required]
        public TacticMentality TacticMentality { get; set; } = TacticMentality.Balanced;

        [Required]
        public PassingMentality PassingMentality { get; set; } = PassingMentality.Balanced;

        public Guid? CaptainID { get; set; }

        public Guid? PenaltyTakerID { get; set; }

        public Guid? LeftCornerTakerID { get; set; }

        public Guid? RightCornerTakerID { get; set; }

        public Guid? LeftFreeKickTakerID { get; set; }

        public Guid? RightFreeKickTakerID { get; set; }
    }
}
