// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿namespace SoccerOpenServer.Models.Users
{
    public class RefreshToken
    {
        public Guid RefreshTokenID { get; set; } = Guid.NewGuid();

        public Guid AppUserID { get; set; }
        public AppUser AppUser { get; set; } = default!;

        public string TokenHash { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresUtc { get; set; }

        public DateTime? RevokedUtc { get; set; }
        public string? ReplacedByTokenHash { get; set; }

        public bool IsActive => RevokedUtc == null && DateTime.UtcNow < ExpiresUtc;
    }
}