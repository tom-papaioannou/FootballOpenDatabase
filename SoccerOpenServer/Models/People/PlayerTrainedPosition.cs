// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using System.ComponentModel.DataAnnotations;

namespace SoccerOpenServer.Models.People
{
    public class PlayerTrainedPosition
    {
        public Guid PlayerTrainedPositionID { get; set; }
        public Guid PersonID { get; set; }
        public PlayerPosition PlayerPosition { get; set; }

        [Range(1, 100)]
        public byte PlayerTrainedPositionAdaptation { get; set; }
    }
}
