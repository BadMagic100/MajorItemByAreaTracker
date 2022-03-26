using ConnectionMetadataInjector.Util;
using MagicUI.Core;
using MagicUI.Elements;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace MajorItemByAreaTracker.UI
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UIDisplayType
    {
        Always, Paused, None
    }

    public class TrackerUI
    {
        private LayoutRoot layout;
        private bool isInGame = false;
        private Dictionary<string, AreaCounterContainer> counterLookup = new();

        private bool IsVisible()
        {
            return isInGame && MajorItemByAreaTracker.Instance.LS.Enabled && MajorItemByAreaTracker.Instance.GS.ShowUI switch
            {
                UIDisplayType.Always => true,
                UIDisplayType.Paused => GameManager.instance.IsGamePaused(),
                _ => false
            };
        }

        public TrackerUI()
        {
            layout = new(true, "Major Item Tracker");
            layout.VisibilityCondition = IsVisible;

            StackLayout labeledGrid = new(layout, "Area Tracker Grid With Label")
            {
                Orientation = Orientation.Vertical,
                Spacing = 7,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            labeledGrid.Children.Add(new TextObject(layout, "Area Tracker Label")
            {
                Text = "Major Items Remaining",
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            });

            DynamicUniformGrid grid = new(layout, "Area Tracker Grid")
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical,
                HorizontalSpacing = 10,
                VerticalSpacing = 5,
            };
            labeledGrid.Children.Add(grid);

            foreach (string area in MapArea.AllMapAreas)
            {
                AreaCounterContainer counter = new(layout, area);
                counterLookup[area] = counter;
                grid.Children.Add(counter);
            }
        }

        public void StartGame()
        {
            isInGame = true;
        }

        public void EndGame()
        {
            isInGame = false;
        }

        public void Refresh()
        {
            foreach (string area in MapArea.AllMapAreas)
            {
                if (counterLookup.TryGetValue(area, out AreaCounterContainer counter)
                    && MajorItemByAreaTracker.Instance.LS.ItemByAreaCounter.TryGetValue(area, out int count))
                {
                    counter.Count = count;
                }
            }
        }
    }
}
