namespace FootballOpenServer.Models.Users
{
    public class AppUserClaim
    {
        public Guid AppUserClaimID { get; set; }
        public Guid AppUserID { get; set; }
        public AppUser AppUser { get; set; } = default!;
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

}
