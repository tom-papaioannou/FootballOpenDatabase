namespace FootballOpenDatabase.Models.People
{
    public enum PlayerRole
    {
        None = 0,

        // ============================
        // Goalkeepers
        // ============================
        Goalkeeper = 1,             // Traditional shot-stopper
        SweeperKeeper = 2,          // GK who plays high, sweeps behind defense

        // ============================
        // Defenders
        // ============================
        CenterBack = 10,            // Standard central defender
        BallPlayingDefender = 11,   // CB comfortable carrying/passing out
        NoNonsenseCenterBack = 12,  // CB focused only on clearing danger
        Libero = 13,                // Sweeper CB, initiates play
        Stopper = 14,               // Aggressive CB stepping out to challenge
        Cover = 15,                 // Deeper CB sweeping behind others

        FullBack = 20,              // Traditional wide defender
        WingBack = 21,              // Wide defender who also attacks
        CompleteWingBack = 22,      // Attacking + defensive wide all-rounder
        InvertedWingBack = 23,      // Wide defender drifting inside
        WideCenterBack = 24,        // CB in back-three who pushes wide

        // ============================
        // Defensive Midfielders
        // ============================
        DefensiveMidfielder = 30,   // Generic DM protecting defense
        Anchorman = 31,             // Static shield in front of defense
        HalfBack = 32,              // DM dropping into defense in build-up
        DeepLyingPlaymaker = 33,    // DM dictating play from deep
        Regista = 34,               // Creative deep playmaker with freedom
        Volante = 35,               // South American holding DM
        SegundoVolante = 36,        // Hybrid DM with license to attack
        BallWinningMidfielder = 37, // Aggressive DM pressing & tackling

        // ============================
        // Central Midfielders
        // ============================
        CentralMidfielder = 40,     // Generic CM
        BoxToBoxMidfielder = 41,    // CM covering both ends of the pitch
        Mezzala = 42,               // CM drifting wide in half-spaces
        Carrilero = 43,             // Lateral CM shuttling between lines
        AdvancedPlaymaker = 44,     // Creative CM or AM dictating attack
        RoamingPlaymaker = 45,      // Mobile CM linking all phases

        // ============================
        // Wide Midfielders & Wingers
        // ============================
        WideMidfielder = 50,        // Traditional wide midfielder
        WidePlaymaker = 51,         // Playmaker drifting wide
        Winger = 52,                // Out-and-out wide attacker
        InvertedWinger = 53,        // Wide player cutting inside onto strong foot
        InsideForward = 54,         // Wide forward cutting inside to score
        InvertedForward = 55,       // Striker drifting into wide channels
        Raumdeuter = 56,            // "Space interpreter", off-ball runner
        WideTargetMan = 57,         // Tall striker deployed wide
        DefensiveWinger = 58,       // Wide player mainly pressing/tracking back

        // ============================
        // Attacking Midfielders
        // ============================
        AttackingMidfielder = 60,   // Generic AM role
        ShadowStriker = 61,         // AM making late box runs to score
        Enganche = 62,              // Static AM playmaker (pivot point)
        Trequartista = 63,          // Free-roaming creative AM
        SecondStriker = 64,         // Support striker behind main forward
        FalseTen = 65,              // AM dropping deep to link play
        CentralWinger = 66,         // AM drifting wide with ball

        // ============================
        // Forwards
        // ============================
        AdvancedForward = 70,       // Mobile striker spearheading attack
        CompleteForward = 71,       // All-round striker, versatile
        Poacher = 72,               // Pure box striker, finishes chances
        TargetMan = 73,             // Physical striker holding up play
        DeepLyingForward = 74,      // Striker dropping deep to link play
        PressingForward = 75,       // Striker focusing on pressing
        DefensiveForward = 76,      // Forward harrying defenders, defensive focus
        FalseNine = 77,             // Striker dropping into midfield
        TrequartistaForward = 78    // Forward version of free playmaker
    }
}
