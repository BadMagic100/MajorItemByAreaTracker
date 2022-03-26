using ConnectionMetadataInjector;
using ConnectionMetadataInjector.Util;
using ItemChanger;
using ItemChanger.Internal;
using RandomizerMod.IC;
using System.Collections.Generic;

namespace MajorItemByAreaTracker.Settings
{
    public class TrackerSettings
    {
        public bool Enabled { get; set; } = false;

        public bool IncludeUniqueKeys { get; set; } = false;

        public bool IncludeCharmNotches { get; set; } = false;

        public Dictionary<string, int> ItemByNameCounter { get; set; } = new();

        public Dictionary<string, int> ItemByAreaCounter { get; set; } = new();

        public void InitCounts()
        {
            ItemByAreaCounter = new();
            ItemByNameCounter = new();
            foreach (AbstractItem item in Ref.Settings.GetItems())
            {
                IncrementItemCount(item);
            }
        }

        private bool IsEligible(AbstractItem item) => MajorItemByAreaTracker.Instance.LS.Enabled &&
            item.HasTag<RandoItemTag>() && SupplementalMetadata.Of(item).Get(MajorItemHandler.IsMajorItem);

        public void IncrementItemCount(AbstractItem item)
        {
            if (!IsEligible(item))
            {
                return;
            }
            MajorItemByAreaTracker.Instance.Log($"Item {item.name} is eligible to count");

            string name = SupplementalMetadata.Of(item).Get(MajorItemHandler.MajorItemName);
            string area = item.RandoLocation()!.LocationDef?.MapArea ?? SubcategoryFinder.OTHER;

            if (!ItemByAreaCounter.ContainsKey(area))
            {
                ItemByAreaCounter[area] = 0;
            }
            ItemByAreaCounter[area]++;

            if (!ItemByNameCounter.ContainsKey(name))
            {
                ItemByNameCounter[name] = 0;
            }
            ItemByNameCounter[name]++;
        }

        public void DecrementItemCount(AbstractItem item)
        {
            if (!IsEligible(item))
            {
                return;
            }
            MajorItemByAreaTracker.Instance.Log($"Item {item.name} is eligible to count");

            string name = SupplementalMetadata.Of(item).Get(MajorItemHandler.MajorItemName);
            string area = item.RandoLocation()!.LocationDef?.MapArea ?? SubcategoryFinder.OTHER;

            if (ItemByAreaCounter.ContainsKey(area))
            {
                ItemByAreaCounter[area]--;
            }

            if (ItemByNameCounter.ContainsKey(name))
            {
                ItemByNameCounter[name]--;
            }
        }
    }
}
