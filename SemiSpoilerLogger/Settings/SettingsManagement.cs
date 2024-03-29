﻿using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace MajorItemByAreaTracker.Settings
{
    internal static class SettingsManagement
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new TrackerSettingsProxy());
        }
    }

    internal class TrackerSettingsProxy : RandoSettingsProxy<TrackerGlobalSettings, string>
    {
        public override string ModKey => MajorItemByAreaTracker.Instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; }
            = new StrictModVersioningPolicy(MajorItemByAreaTracker.Instance);

        public override void ReceiveSettings(TrackerGlobalSettings? settings)
        {
            if (settings != null)
            {
                MenuHolder.Instance!.ApplySettingsToMenu(settings);
            }
            else
            {
                MenuHolder.Instance!.Disable();
            }
        }

        public override bool TryProvideSettings(out TrackerGlobalSettings? settings)
        {
            settings = MajorItemByAreaTracker.Instance.GS;
            return settings.Enabled;
        }
    }
}
