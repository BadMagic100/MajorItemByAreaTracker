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
            AddCuteSpellingToggle((ToggleButton)mef.ElementLookup[nameof(MajorItemByAreaTracker.Instance.GS.IncludeGrubs)]);
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

        private void AddCuteSpellingToggle(ToggleButton button)
        {
            const int ClicksToToggle = 7;
            const float ConsecutiveClickInterval = .5f;

            // This should happen before the localization formatter is
            // installed.
            button.Formatter = new CuteSpellingFormatter(button.Formatter);
            
            int numConsecutiveClicks = 0;
            float timeOfLastClick = float.NegativeInfinity;
            button.OnClick += () =>
            {
                float now = UnityEngine.Time.time;
                if (now - timeOfLastClick > ConsecutiveClickInterval)
                {
                    numConsecutiveClicks = 0;
                }
                timeOfLastClick = now;
                numConsecutiveClicks++;
                if (numConsecutiveClicks == ClicksToToggle)
                {
                    MajorItemByAreaTracker.Instance.GS.CuteSpelling = !MajorItemByAreaTracker.Instance.GS.CuteSpelling;
                    // Trigger a text refresh, which will cause the
                    // CuteSpellingFormatter to return a new string.
                    button.Formatter = button.Formatter;
                    numConsecutiveClicks = 0;
                }
            };
        }
    }

    internal class CuteSpellingFormatter : MenuItemFormatter
    {
        private MenuItemFormatter orig;

        public CuteSpellingFormatter(MenuItemFormatter orig)
        {
            this.orig = orig;
        }

        public override string GetText(string prefix, object value)
        {
            if (MajorItemByAreaTracker.Instance.GS.CuteSpelling)
            {
                return "Include Grubbies";
            }
            return orig.GetText(prefix, value);
        }
    }
}
