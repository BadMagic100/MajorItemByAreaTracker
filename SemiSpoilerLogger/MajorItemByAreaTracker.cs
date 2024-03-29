﻿using ItemChanger;
using MajorItemByAreaTracker.Settings;
using MajorItemByAreaTracker.UI;
using Modding;
using Newtonsoft.Json;
using RandomizerMod.Logging;
using RandomizerMod.RC;
using Satchel.BetterMenus;
using System;

namespace MajorItemByAreaTracker
{
    public class MajorItemByAreaTracker : Mod, IGlobalSettings<TrackerGlobalSettings>, ICustomMenuMod
    {
        private static MajorItemByAreaTracker? _instance;
        internal static MajorItemByAreaTracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"{nameof(MajorItemByAreaTracker)} was never initialized");
                }
                return _instance;
            }
        }

        internal TrackerGlobalSettings GS = new();

        private Menu? menuRef;

        public bool ToggleButtonInsideMenu => throw new NotImplementedException();

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public MajorItemByAreaTracker() : base()
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            RandoController.OnExportCompleted += OnRandoGameStart;
            RandoController.OnCalculateHash += HashModifier.AdjustHash;
            SettingsLog.AfterLogSettings += AddAllMajorsSettings;

            MenuHolder.Hook();
            if (ModHooks.GetMod("RandoSettingsManager") is Mod)
            {
                SettingsManagement.Hook();
            }

            Log("Initialized");
        }

        private void OnRandoGameStart(RandoController rc)
        {
            if (!GS.Enabled)
            {
                return;
            }

            if (ItemChangerMod.Modules.Get<MajorItemTrackerModule>() == null)
            {
                MajorItemTrackerModule tracker = ItemChangerMod.Modules.GetOrAdd<MajorItemTrackerModule>();
                tracker.PrepareFirstTimeConfig();
            }
        }

        private void AddAllMajorsSettings(LogArguments args, System.IO.TextWriter tw)
        {
            tw.WriteLine("All Majors Settings:");
            using JsonTextWriter jtw = new(tw) { CloseOutput = false };
            RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, GS);
            tw.WriteLine();
        }

        public void OnLoadGlobal(TrackerGlobalSettings s) => GS = s;

        public TrackerGlobalSettings OnSaveGlobal() => GS;

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            menuRef ??= new Menu("Major Item Tracker", new Element[]
            {
                new HorizontalOption("Show UI",
                    "Choose when to display the tracker UI while the connection is enabled",
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