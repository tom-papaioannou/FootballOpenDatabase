// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System.Text.Json.Serialization;

namespace SoccerOpenServer.Models.Teams
{
    public class Kit
    {
        public Guid KitID { get; set; }
        public string HomeShirtColor { get; set; } = string.Empty;
        public string HomeShortsColor { get; set; } = string.Empty;
        public string AwayShirtColor { get; set; } = string.Empty;
        public string AwayShortsColor { get; set; } = string.Empty;
        public KitShapeEnum KitShape { get; set; } = KitShapeEnum.Empty;

        [JsonIgnore]
        public virtual Team? Team { get; set; }
    }
}
