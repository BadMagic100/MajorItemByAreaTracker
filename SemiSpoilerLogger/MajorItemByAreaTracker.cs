using ItemChanger;
using MajorItemByAreaTracker.Settings;
using MajorItemByAreaTracker.UI;
using Modding;
using RandomizerMod.IC;
using Satchel.BetterMenus;
using System;

namespace MajorItemByAreaTracker
{
    public class MajorItemByAreaTracker : Mod, IGlobalSettings<TrackerGlobalSettings>, ILocalSettings<TrackerSettings>, ICustomMenuMod
    {
        internal static MajorItemByAreaTracker Instance;

        internal TrackerGlobalSettings GS = new();
        internal TrackerSettings LS = new();

        private TrackerUI ui;
        private Menu? menuRef;

        public bool ToggleButtonInsideMenu => throw new NotImplementedException();

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public MajorItemByAreaTracker() : base()
        {
            Instance = this;
            ui = new();
        }

        public override void Initialize()
        {
            Log("Initializing");

            Events.AfterStartNewGame += OnSaveCreated;
            RandoItemTag.AfterRandoItemGive += OnItemGive;

            On.HeroController.Awake += OnGameStart;
            On.GameCompletionScreen.Start += OnCompletion;
            On.QuitToMenu.Start += OnQuitGame;

            MenuHolder.Hook();
            HashModifier.Hook();

            Log("Initialized");
        }

        private void OnGameStart(On.HeroController.orig_Awake orig, HeroController self)
        {
            orig(self);
            ui.StartGame();
        }

        private void OnCompletion(On.GameCompletionScreen.orig_Start orig, GameCompletionScreen self)
        {
            ui.EndGame();
            orig(self);
        }

        private System.Collections.IEnumerator OnQuitGame(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            ui.EndGame();
            return orig(self);
        }

        private void OnItemGive(int idx, ReadOnlyGiveEventArgs obj)
        {
            LS.DecrementItemCount(obj.Orig);
            ui.Refresh();
        }

        private void OnSaveCreated()
        {
            LS = GS.ToTrackerSettings();
            LS.InitCounts();
            MajorItemByAreaLogger.Log();
            ui.Refresh();
        }

        public void OnLoadGlobal(TrackerGlobalSettings s) => GS = s;

        public void OnLoadLocal(TrackerSettings s)
        {
            LS = s;
            ui.Refresh();
        }

        public TrackerGlobalSettings OnSaveGlobal() => GS;

        public TrackerSettings OnSaveLocal() => LS;

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            menuRef ??= new Menu("Major Item Tracker", new Element[]
            {
                new HorizontalOption("Show UI",
                    "Choose when to display the tracker UI while the connecion is enabled",
                    Enum.GetNames(typeof(UIDisplayType)),
                    (i) => GS.ShowUI = (UIDisplayType)i,
                    () => (int)GS.ShowUI),
                new TextPanel(""),
                new TextPanel("Mod by BadMagic"),
                new TextPanel("Credit to Numberplay for the original format idea")
            });

            return menuRef.GetMenuScreen(modListMenu);
        }
    }
}