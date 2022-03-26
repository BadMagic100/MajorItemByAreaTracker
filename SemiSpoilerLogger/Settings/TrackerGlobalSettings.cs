using MajorItemByAreaTracker.UI;
using MenuChanger.Attributes;

namespace MajorItemByAreaTracker.Settings
{

    public class TrackerGlobalSettings
    {
        public bool Enabled { get; set; } = false;

        public bool IncludeUniqueKeys { get; set; } = false;

        public bool IncludeCharmNotches { get; set; } = false;

        [MenuIgnore]
        public UIDisplayType ShowUI { get; set; } = UIDisplayType.Always;

        public TrackerSettings ToTrackerSettings()
        {
            return new()
            {
                Enabled = Enabled,
                IncludeCharmNotches = Enabled && IncludeCharmNotches,
                IncludeUniqueKeys = Enabled && IncludeUniqueKeys
            };
        }
    }
}
