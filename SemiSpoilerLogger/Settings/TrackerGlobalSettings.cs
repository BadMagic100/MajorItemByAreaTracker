using MajorItemByAreaTracker.UI;
using MenuChanger.Attributes;

namespace MajorItemByAreaTracker.Settings
{

    public class TrackerGlobalSettings
    {
        public bool Enabled { get; set; } = false;

        public bool IncludeSkills { get; set; } = true;

        public bool IncludeDreamers { get; set; } = true;

        public bool IncludeWhiteFragments { get; set; } = true;

        public bool IncludeGrubs { get; set; } = false;

        public bool IncludeUniqueKeys { get; set; } = false;

        public bool IncludeSimpleKeys { get; set; } = false;

        [MenuLabel("Include Key-like Charms")]
        public bool IncludeKeyLikeCharms { get; set; } = false;

        public bool IncludeFragileCharms { get; set; } = false;

        public bool IncludeStags { get; set; } = false;

        [MenuIgnore]
        [HashModifier.HashIgnore]
        public UIDisplayType ShowUI { get; set; } = UIDisplayType.Always;

        [MenuIgnore]
        [HashModifier.HashIgnore]
        public bool CuteSpelling { get; set; } = false;

        public TrackerSettings ToTrackerSettings()
        {
            return new()
            {
                IncludeSkills = IncludeSkills,
                IncludeDreamers = IncludeDreamers,
                IncludeWhiteFragments = IncludeWhiteFragments,
                IncludeGrubs = IncludeGrubs,
                IncludeUniqueKeys = IncludeUniqueKeys,
                IncludeSimpleKeys = IncludeSimpleKeys,
                IncludeKeyLikeCharms = IncludeKeyLikeCharms,
                IncludeFragileCharms = IncludeFragileCharms,
                IncludeStags = IncludeStags,
            };
        }
    }
}
