using ConnectionMetadataInjector;
using ConnectionMetadataInjector.Util;
using ItemChanger;
using RandomizerMod.IC;
using CMI = ConnectionMetadataInjector.ConnectionMetadataInjector;

namespace MajorItemByAreaTracker
{
    public static class MajorItemHandler
    {
        internal static readonly MetadataProperty<AbstractItem, bool> IsMajorItem = new("IsMajorItem", GetIsMajorItemDefault);
        internal static readonly MetadataProperty<AbstractItem, string> MajorItemName = new("MajorItemName", ResolveItemName);

        // yes, itemname is unused... it may be used in the future though, e.g. to filter off swimming items or nail arts
        private static bool IsSkill(string poolGroup, string itemName) => poolGroup == PoolGroup.Skills.FriendlyName();

        private static bool IsUniqueKey(string poolGroup, string itemName) => poolGroup == PoolGroup.Keys.FriendlyName()
            && !itemName.IsOneOf("Simple_Key", "Collector's_Map", "Godtuner");

        private static bool IsDreamer(string poolGroup, string itemName) => poolGroup == PoolGroup.Dreamers.FriendlyName() && itemName != "World_Sense";

        private static bool IsWhiteFragment(string poolGroup, string itemName) => poolGroup == PoolGroup.Charms.FriendlyName()
            && itemName.IsOneOf("Queen_Fragment", "King_Fragment", "Void_Heart");

        private static bool IsCharmNotch(string poolGroup, string itemName) => poolGroup == PoolGroup.CharmNotches.FriendlyName() && itemName != "Salubra's_Blessing";

        private static bool GetIsMajorItemDefault(AbstractItem item)
        {
            // only rando items are allowed
            if (!item.HasTag<RandoItemTag>())
            {
                return false;
            }

            string itemName = item.RandoItem()!.Name;
            SupplementalMetadata<AbstractItem> itemMeta = SupplementalMetadata.Of(item);
            string poolGroup = itemMeta.Get(CMI.ItemPoolGroup);
            return IsSkill(poolGroup, itemName)
                || IsDreamer(poolGroup, itemName)
                || IsWhiteFragment(poolGroup, itemName)
                || (MajorItemByAreaTracker.Instance.LS.IncludeUniqueKeys && IsUniqueKey(poolGroup, itemName))
                || (MajorItemByAreaTracker.Instance.LS.IncludeCharmNotches && IsCharmNotch(poolGroup, itemName));
        }

        private static string ResolveItemName(AbstractItem item) => item.RandoItem()!.Name
            .Merge(ItemNames.Mothwing_Cloak, ItemNames.Shade_Cloak, ItemNames.Left_Mothwing_Cloak, ItemNames.Right_Mothwing_Cloak)
            .Merge(ItemNames.Mantis_Claw, ItemNames.Left_Mantis_Claw, ItemNames.Right_Mantis_Claw)
            .Merge(ItemNames.Crystal_Heart, ItemNames.Left_Crystal_Heart, ItemNames.Right_Mantis_Claw)
            .Merge(ItemNames.Vengeful_Spirit, ItemNames.Shade_Soul)
            .Merge(ItemNames.Desolate_Dive, ItemNames.Descending_Dark)
            .Merge(ItemNames.Howling_Wraiths, ItemNames.Abyss_Shriek)
            .Merge(ItemNames.Dream_Nail, ItemNames.Dream_Gate, ItemNames.Awoken_Dream_Nail)
            .Merge(ItemNames.Dreamer, ItemNames.Lurien, ItemNames.Monomon, ItemNames.Herrah)
            .Merge("White Fragment", ItemNames.King_Fragment, ItemNames.Queen_Fragment, ItemNames.Void_Heart)
            .Merge("Nail Art", ItemNames.Great_Slash, ItemNames.Dash_Slash, ItemNames.Cyclone_Slash)
            .Replace('_', ' ');
    }
}
