using ConnectionMetadataInjector.Util;
using MagicUI.Core;
using MagicUI.Elements;
using MajorItemByAreaTracker.Settings;
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

    internal class TrackerUI
    {
        private LayoutRoot layout;
        private Dictionary<string, AreaCounterContainer> counterLookup = new();
        private TrackerSettings model;

        private bool IsVisible()
        {
            return MajorItemByAreaTracker.Instance.GS.ShowUI switch
            {
                UIDisplayType.Always => true,
                UIDisplayType.Paused => GameManager.instance.IsGamePaused(),
                _ => false
            };
        }

        public TrackerUI(TrackerSettings model)
        {
            this.model = model;

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

        public void Refresh()
        {
            int sum = 0;
            foreach (string area in MapArea.AllMapAreas)
            {
                if (counterLookup.TryGetValue(area, out AreaCounterContainer counter)
                    && model.ItemByAreaCounter.TryGetValue(area, out int count) == true)
                {
                    counter.Count = count;
                    sum += count;
                }
            }
            SetLabelText(sum);
        }

        private void SetLabelText(int total)
        {
            TextObject? label = layout?.GetElement<TextObject>("Area Tracker Label");
            if (label != null)
            {
                label.Text = $"Major Items Remaining ({total})";
            }
        }

        public void Destroy()
        {
            layout.Destroy();
        }
    }
}
