// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿namespace SoccerOpenServer.Models.Teams
{
    public enum Formation
    {
        None = 0,

        // Classic formations (100+)
        Four_Four_Two = 100,
        Four_Three_Three = 101,
        Three_Five_Two = 102,
        Five_Three_Two = 103,
        Four_Five_One = 104,

        // 4 at the back variations (200+)
        Four_Two_Three_One = 200,
        Four_Three_Two_One = 201,
        Four_One_Four_One = 202,
        Four_Four_One_One = 203,
        Four_Two_Two_Two = 204,

        // 3 at the back (300+)
        Three_Four_Three = 300,
        Three_Four_Two_One = 301,
        Three_Four_One_Two = 302,
        Three_Three_Four = 303,

        // 5 at the back / wingbacks (400+)
        Five_Four_One = 400,
        Five_Two_Three = 401,
        Five_Three_One_One = 402,

        // Uncommon / historical (900+)
        Four_Six_Zero = 900,
        Two_Three_Five = 901
    }
}