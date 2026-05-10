// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using System.Security.Cryptography;
using System.Text;

namespace FootballOpenServer.Services
{
    public interface IPasswordHasherService
    {
        void CreateHash(string password, out byte[] hash, out byte[] salt);
        bool Verify(string password, byte[] storedHash, byte[] storedSalt);
    }

    public class PasswordHasherService : IPasswordHasherService
    {
        public void CreateHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public bool Verify(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computed.SequenceEqual(storedHash);
        }
    }
}
