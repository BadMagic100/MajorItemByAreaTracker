using MajorItemByAreaTracker.Settings;
using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod;
using RandomizerMod.Menu;

namespace MajorItemByAreaTracker
{
    public class MenuHolder
    {
        private MenuPage trackerPage;
        private MenuElementFactory<TrackerGlobalSettings> mef;
        private VerticalItemPanel vip;

        private SmallButton jumpToTrackerButton;

        internal static MenuHolder? Instance { get; private set; }

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(Construct, HandleButton);
            MenuChangerMod.OnExitMainMenu += () => Instance = null;
        }

        private static bool HandleButton(MenuPage connectionPage, out SmallButton btn)
        {
            btn = Instance!.jumpToTrackerButton;
            return true;
        }

        private static void Construct(MenuPage connectionPage)
        {
            Instance = new(connectionPage);
        }

        private MenuHolder(MenuPage connectionPage)
        {
            trackerPage = new MenuPage(Localization.Localize("All Major Items"), connectionPage);
            mef = new MenuElementFactory<TrackerGlobalSettings>(trackerPage, MajorItemByAreaTracker.Instance.GS);
            ToggleButton enabledControl = (ToggleButton)mef.ElementLookup[nameof(MajorItemByAreaTracker.Instance.GS.Enabled)];
            enabledControl.SelfChanged += EnabledChanged;
            vip = new VerticalItemPanel(trackerPage, new(0, 300), 50f, true, mef.Elements);
            Localization.Localize(mef);

            jumpToTrackerButton = new SmallButton(connectionPage, Localization.Localize("All Major Items"));
            jumpToTrackerButton.AddHideAndShowEvent(connectionPage, trackerPage!);
            SetTopLevelButtonColor();
        }

        private void EnabledChanged(IValueElement obj)
        {
            SetTopLevelButtonColor();
        }

        private void SetTopLevelButtonColor()
        {
            jumpToTrackerButton.Text.color = MajorItemByAreaTracker.Instance.GS.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
        }

        public void Disable()
        {
            IValueElement elem = mef.ElementLookup[nameof(TrackerGlobalSettings.Enabled)];
            elem.SetValue(false);
        }

        public void ApplySettingsToMenu(TrackerGlobalSettings settings)
        {
            mef.SetMenuValues(settings);
        }
    }
}
