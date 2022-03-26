using ConnectionMetadataInjector.Util;
using RandomizerMod.Logging;
using System.Collections.Generic;
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

            sb.AppendLine("----- Item Counts By Map Area -----");
            foreach (string area in MapArea.AllMapAreas)
            {
                MajorItemByAreaTracker.Instance.LS.ItemByAreaCounter.TryGetValue(area, out int count);
                sb.AppendLine($"{area}: {count}");
            }
            sb.AppendLine();
            sb.AppendLine("----- Items To Find -----");
            foreach (KeyValuePair<string, int> itemPair in MajorItemByAreaTracker.Instance.LS.ItemByNameCounter)
            {
                sb.AppendLine($"{itemPair.Value} {itemPair.Key}");
            }

            LogManager.Write(sb.ToString(), "MajorItemCountByAreaLog.txt");
        }
    }
}
