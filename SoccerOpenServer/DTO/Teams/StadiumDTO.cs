// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

namespace SoccerOpenServer.DTO.Teams
{
    public class StadiumDTO
    {
        public Guid StadiumID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public double Latitude { get; set; } = 0.0;
        public double Longitude { get; set; } = 0.0;
    }
}
