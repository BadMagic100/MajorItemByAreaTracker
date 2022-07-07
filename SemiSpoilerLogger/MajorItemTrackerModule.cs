using ConnectionMetadataInjector;
using ConnectionMetadataInjector.Util;
using ItemChanger;
using ItemChanger.Internal;
using ItemChanger.Modules;
using MajorItemByAreaTracker.Settings;
using MajorItemByAreaTracker.UI;
using Newtonsoft.Json;
using RandomizerMod.Extensions;
using RandomizerMod.IC;
using CMI = ConnectionMetadataInjector.ConnectionMetadataInjector;

namespace MajorItemByAreaTracker
{
    internal class MajorItemTrackerModule : Module
    {
        private readonly MetadataProperty<AbstractItem, bool> IsMajorItem;
        private readonly MetadataProperty<AbstractItem, string> MajorItemName;

        public TrackerSettings Config { get; set; }

        [JsonIgnore]
        private TrackerUI? ui;

        public MajorItemTrackerModule()
        {
            Config = new();

            IsMajorItem = new("IsMajorItem", GetIsMajorItemDefault);
            MajorItemName = new("MajorItemName", ResolveItemName);
        }

        public void PrepareFirstTimeConfig()
        {
            Config = MajorItemByAreaTracker.Instance.GS.ToTrackerSettings();
            Config.ItemByAreaCounter = new();
            Config.ItemByNameCounter = new();
            foreach (AbstractItem item in Ref.Settings.GetItems())
            {
                IncrementItemCount(item);
            }
        }

        public override void Initialize()
        {
            Events.AfterStartNewGame += OnNewGameStart;
            On.GameCompletionScreen.Start += OnGoToCompletionScreen;
            RandoItemTag.AfterRandoItemGive += OnGetItem;

            ui = new(Config);
            ui.Refresh();
        }

        private void OnNewGameStart()
        {
            MajorItemByAreaTracker.Instance.Log("New game started, generating logs");
            MajorItemByAreaLogger.Log(Config);
        }

        private void OnGetItem(int idx, ReadOnlyGiveEventArgs args)
        {
            DecrementItemCount(args.Orig);
            ui?.Refresh();
        }

        private void OnGoToCompletionScreen(On.GameCompletionScreen.orig_Start orig, GameCompletionScreen self)
        {
            orig(self);
            ui?.Destroy();
            ui = null;
        }

        public override void Unload()
        {
            Events.AfterStartNewGame -= OnNewGameStart;
            On.GameCompletionScreen.Start -= OnGoToCompletionScreen;
            RandoItemTag.AfterRandoItemGive -= OnGetItem;

            ui?.Destroy();
            ui = null;
        }

        private bool IsSkill(string poolGroup, string itemName) => poolGroup == PoolGroup.Skills.FriendlyName();

        private bool IsUniqueKey(string poolGroup, string itemName) => poolGroup == PoolGroup.Keys.FriendlyName()
            && !itemName.IsOneOf(ItemNames.Simple_Key, ItemNames.Collectors_Map, ItemNames.Godtuner);

        private bool IsSimpleKey(string poolGroup, string itemName) => itemName == ItemNames.Simple_Key;

        private bool IsDreamer(string poolGroup, string itemName) => poolGroup == PoolGroup.Dreamers.FriendlyName() && itemName != ItemNames.World_Sense;

        private bool IsWhiteFragment(string poolGroup, string itemName) => poolGroup == PoolGroup.Charms.FriendlyName()
            && itemName.IsOneOf(ItemNames.Queen_Fragment, ItemNames.King_Fragment, ItemNames.Void_Heart);

        private bool IsKeyLikeCharm(string poolGroup, string itemName) => itemName.IsOneOf(ItemNames.Defenders_Crest, ItemNames.Spore_Shroom,
            ItemNames.Grimmchild1, ItemNames.Grimmchild2);

        private bool IsStag(string poolGroup, string itemName) => poolGroup.Equals(PoolGroup.Stags.FriendlyName());

        private bool GetIsMajorItemDefault(AbstractItem item)
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
                || (Config.IncludeUniqueKeys && IsUniqueKey(poolGroup, itemName))
                || (Config.IncludeSimpleKeys && IsSimpleKey(poolGroup, itemName))
                || (Config.IncludeKeyLikeCharms && IsKeyLikeCharm(poolGroup, itemName))
                || (Config.IncludeStags && IsStag(poolGroup, itemName));
        }

        private static string ResolveItemName(AbstractItem item) => item.RandoItem()!.Name
            .Merge(ItemNames.Mothwing_Cloak, ItemNames.Shade_Cloak, ItemNames.Left_Mothwing_Cloak, ItemNames.Right_Mothwing_Cloak)
            .Merge(ItemNames.Mantis_Claw, ItemNames.Left_Mantis_Claw, ItemNames.Right_Mantis_Claw)
            .Merge(ItemNames.Crystal_Heart, ItemNames.Left_Crystal_Heart, ItemNames.Right_Crystal_Heart)
            .Merge(ItemNames.Vengeful_Spirit, ItemNames.Shade_Soul)
            .Merge(ItemNames.Desolate_Dive, ItemNames.Descending_Dark)
            .Merge(ItemNames.Howling_Wraiths, ItemNames.Abyss_Shriek)
            .Merge(ItemNames.Dream_Nail, ItemNames.Dream_Gate, ItemNames.Awoken_Dream_Nail)
            .Merge(ItemNames.Dreamer, ItemNames.Lurien, ItemNames.Monomon, ItemNames.Herrah)
            .Merge("White Fragment", ItemNames.King_Fragment, ItemNames.Queen_Fragment, ItemNames.Void_Heart)
            .Merge("Nail Art", ItemNames.Great_Slash, ItemNames.Dash_Slash, ItemNames.Cyclone_Slash)
            .Merge("Grimmchild", ItemNames.Grimmchild1, ItemNames.Grimmchild2)
            .Replace('_', ' ');

        private bool IsEligible(AbstractItem item) => item.HasTag<RandoItemTag>() && SupplementalMetadata.Of(item).Get(IsMajorItem);

        private void IncrementItemCount(AbstractItem item)
        {
            if (!IsEligible(item))
            {
                return;
            }
            MajorItemByAreaTracker.Instance.Log($"Item {item.name} is eligible to count");

            string name = SupplementalMetadata.Of(item).Get(MajorItemName);
            string area = item.RandoLocation()!.LocationDef?.MapArea ?? SubcategoryFinder.OTHER;

            if (!Config.ItemByAreaCounter.ContainsKey(area))
            {
                Config.ItemByAreaCounter[area] = 0;
            }
            Config.ItemByAreaCounter[area]++;

            if (!Config.ItemByNameCounter.ContainsKey(name))
            {
                Config.ItemByNameCounter[name] = 0;
            }
            Config.ItemByNameCounter[name]++;
        }

        private void DecrementItemCount(AbstractItem item)
        {
            if (!IsEligible(item))
            {
                return;
            }
            MajorItemByAreaTracker.Instance.Log($"Item {item.name} is eligible to count");

            string name = SupplementalMetadata.Of(item).Get(MajorItemName);
            string area = item.RandoLocation()!.LocationDef?.MapArea ?? SubcategoryFinder.OTHER;

            if (Config.ItemByAreaCounter.ContainsKey(area))
            {
                Config.ItemByAreaCounter[area]--;
            }

            if (Config.ItemByNameCounter.ContainsKey(name))
            {
                Config.ItemByNameCounter[name]--;
            }
        }
    }
}
