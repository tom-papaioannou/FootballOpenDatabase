// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using SoccerOpenServer.Models.People;

namespace SoccerOpenServer.Models.Teams
{
    public class PlayerTactic
    {
        public Guid PlayerTacticID { get; set; }
        public Guid TacticID { get; set; }
        public Guid PersonID { get; set; }
        public virtual Person? Person { get; set; }
        public PlayerPosition PlayerPosition { get; set; }
        public PlayerRole PlayerRole { get; set; }
        public SquadUnit SquadUnit { get; set; }
        public int? SubstituteOrder { get; set; }
    }
}
