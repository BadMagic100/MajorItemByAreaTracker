using ConnectionMetadataInjector.Util;
using MajorItemByAreaTracker.Settings;
using RandomizerMod.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MajorItemByAreaTracker
{
    internal static class MajorItemByAreaLogger
    {
        public static void Log()
        {
            if (!MajorItemByAreaTracker.Instance.LS.Enabled)
            {
                return;
            }

            StringBuilder sb = new();

            TrackerSettings settings = MajorItemByAreaTracker.Instance.LS;

            sb.AppendLine("----- Item Counts By Map Area -----");
            foreach (string area in MapArea.AllMapAreas)
            {
                settings.ItemByAreaCounter.TryGetValue(area, out int count);
                sb.AppendLine($"{area}: {count}");
            }
            sb.AppendLine();
            sb.AppendLine($"----- Items To Find ({settings.ItemByNameCounter.Values.Sum()}) -----");
            foreach (KeyValuePair<string, int> itemPair in settings.ItemByNameCounter)
            {
                sb.AppendLine($"{itemPair.Value} {itemPair.Key}");
            }

            LogManager.Write(sb.ToString(), "MajorItemCountByAreaLog.txt");
        }
    }
}
