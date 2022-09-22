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
        internal MenuPage? trackerPage;
        internal MenuElementFactory<TrackerGlobalSettings>? mef;
        internal VerticalItemPanel? vip;

        internal SmallButton? jumpToTrackerButton;

        private static MenuHolder? instance = null;
        internal static MenuHolder Instance => instance ??= new();

        private static void OnExitMenu()
        {
            instance = null;
        }

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(Instance.Construct, Instance.HandleButton);
            MenuChangerMod.OnExitMainMenu += OnExitMenu;
        }

        private bool HandleButton(MenuPage landing, out SmallButton btn)
        {
            jumpToTrackerButton = new SmallButton(landing, Localization.Localize("All Major Items"));
            jumpToTrackerButton.AddHideAndShowEvent(landing, trackerPage);
            SetTopLevelButtonColor();
            btn = jumpToTrackerButton;
            return true;
        }

        private void Construct(MenuPage landing)
        {
            trackerPage = new MenuPage(Localization.Localize("All Major Items"), landing);
            mef = new MenuElementFactory<TrackerGlobalSettings>(trackerPage, MajorItemByAreaTracker.Instance.GS);
            ToggleButton enabledControl = (ToggleButton)mef.ElementLookup[nameof(MajorItemByAreaTracker.Instance.GS.Enabled)];
            enabledControl.SelfChanged += EnabledChanged;
            vip = new VerticalItemPanel(trackerPage, new(0, 300), 50f, true, mef.Elements);
            Localization.Localize(mef);
        }

        private void EnabledChanged(IValueElement obj)
        {
            SetTopLevelButtonColor();
        }

        private void SetTopLevelButtonColor()
        {
            if (jumpToTrackerButton != null)
            {
                jumpToTrackerButton.Text.color = MajorItemByAreaTracker.Instance.GS.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }
        }
    }
}
