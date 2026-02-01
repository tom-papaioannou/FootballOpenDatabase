using FootballOpenServer.Models.People;

namespace FootballOpenServer.Models.Users
{
    public class AppUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public Guid? PersonID { get; set; }
        public Person? Person { get; set; }

        public List<AppUserClaim> Claims { get; set; } = new();
    }
}
