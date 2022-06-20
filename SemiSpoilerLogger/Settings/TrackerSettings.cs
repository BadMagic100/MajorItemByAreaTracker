using System.Collections.Generic;

namespace MajorItemByAreaTracker.Settings
{
    public class TrackerSettings
    {
        public bool IncludeUniqueKeys { get; set; } = false;

        public bool IncludeSimpleKeys { get; set; } = false;

        public bool IncludeKeyLikeCharms { get; set; } = false;

        public bool IncludeStags { get; set; } = false;

        public Dictionary<string, int> ItemByNameCounter { get; set; } = new();

        public Dictionary<string, int> ItemByAreaCounter { get; set; } = new();
    }
}
