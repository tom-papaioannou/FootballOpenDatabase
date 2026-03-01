// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

namespace FootballOpenServer.Models.World
{
    public class Continent
    {
        public Guid ContinentID { get; set; }
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
        public string? SymbolUrl { get; set; }
        public ICollection<Nation> Nations { get; set; } = new List<Nation>();
    }
}
