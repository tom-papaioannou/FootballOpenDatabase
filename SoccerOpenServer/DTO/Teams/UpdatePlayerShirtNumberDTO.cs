// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System.ComponentModel.DataAnnotations;

namespace SoccerOpenServer.DTO.Teams
{
    public class UpdatePlayerShirtNumberDTO
    {
        public Guid TeamID { get; set; }
        public Guid PersonID { get; set; }

        [Range(1, 99)]
        public byte ShirtNumber { get; set; }
    }
}
