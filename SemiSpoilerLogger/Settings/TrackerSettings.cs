using MenuChanger.Attributes;
using System.Collections.Generic;

namespace MajorItemByAreaTracker.Settings
{
    public class TrackerSettings
    {
        public bool IncludeUniqueKeys { get; set; } = false;

        public bool IncludeCharmNotches { get; set; } = false;

        [MenuLabel("Include Key-like Charms")]
        public bool IncludeKeyLikeCharms { get; set; } = false;

        public Dictionary<string, int> ItemByNameCounter { get; set; } = new();

        public Dictionary<string, int> ItemByAreaCounter { get; set; } = new();
    }
}
