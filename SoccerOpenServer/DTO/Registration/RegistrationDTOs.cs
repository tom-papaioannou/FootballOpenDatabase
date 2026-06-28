// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using SoccerOpenServer.DTO.Teams;
using SoccerOpenServer.Models.Teams;

namespace SoccerOpenServer.DTO.Registration
{
    public class JoinableServerDTO
    {
        public Guid ServerID { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class RegistrationAvailabilityDTO
    {
        public bool IsAvailable { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class RegistrationNationDTO
    {
        public Guid NationID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ISO2 { get; set; } = string.Empty;
        public string? ISO3 { get; set; }
        public string? FlagUrl { get; set; }
    }

    public class RegistrationTeamDTO
    {
        public Guid TeamID { get; set; }
        public string? Name { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid CompetitionID { get; set; }
        public string? CompetitionName { get; set; }
        public Guid NationID { get; set; }
        public string NationName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string? BadgeUrl { get; set; }
        public StadiumDTO? Stadium { get; set; }
        public Kit? Kit { get; set; }
    }

    public class CompleteRegistrationRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public Guid ServerID { get; set; }
        public Guid NationID { get; set; }
        public Guid TeamID { get; set; }
    }

    public class CompleteRegistrationResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid ServerID { get; set; }
        public Guid TeamID { get; set; }
        public string? TeamName { get; set; }
    }

    public class RegistrationConflictDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
