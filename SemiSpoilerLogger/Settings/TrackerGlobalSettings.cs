using MajorItemByAreaTracker.UI;
using MenuChanger.Attributes;

namespace MajorItemByAreaTracker.Settings
{

    public class TrackerGlobalSettings
    {
        public bool Enabled { get; set; } = false;

        public bool IncludeUniqueKeys { get; set; } = false;

        public bool IncludeCharmNotches { get; set; } = false;

        [MenuLabel("Include Key-like Charms")]
        public bool IncludeKeyLikeCharms { get; set; } = false;

        [MenuIgnore]
        [HashModifier.HashIgnore]
        public UIDisplayType ShowUI { get; set; } = UIDisplayType.Always;

        public TrackerSettings ToTrackerSettings()
        {
            return new()
            {
                IncludeCharmNotches = IncludeCharmNotches,
                IncludeUniqueKeys = IncludeUniqueKeys,
                IncludeKeyLikeCharms = IncludeKeyLikeCharms,
            };
        }
    }
}
