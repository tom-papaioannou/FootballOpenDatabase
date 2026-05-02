using System.Text.Json.Serialization;

namespace FootballOpenServer.Models.Teams
{
    public class Kit
    {
        public Guid KitID { get; set; }
        public string HomeShirtColor { get; set; } = string.Empty;
        public string HomeShortsColor { get; set; } = string.Empty;
        public string AwayShirtColor { get; set; } = string.Empty;
        public string AwayShortsColor { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual Team? Team { get; set; }
    }
}
