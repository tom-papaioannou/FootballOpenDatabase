// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.People;

namespace FootballOpenServer.Models.Servers
{
    public class Server
    {
        public Guid ServerID { get; set; }
        public string Name { get; set; }
        public List<Person> Persons { get; set; } = new();
        public List<Competition> Competitions { get; set; } = new();
        public int Season { get; set; } = 0;
    }
}
