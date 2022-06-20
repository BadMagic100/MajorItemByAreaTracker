using ConnectionMetadataInjector.Util;
using MagicUI.Core;
using MagicUI.Elements;
using MajorItemByAreaTracker.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RandomizerMod;
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
        private Dictionary<string, TextFormatter<int>> counterLookup = new();
        private TrackerSettings model;

        private string TryLocalize(string area)
        {
            area = area == MapArea.FORGOTTEN_CROSSROADS ? "Crossroads" : area;
            try
            {
                return Localization.Localize(area);
            }
            catch
            {
                return area;
            }
        }

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
                TextFormatter<int> counter = new(layout, 0, (i) => $"{TryLocalize(area)}: {i}")
                {
                    Text = new(layout)
                    {
                        FontSize = 15
                    },
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                counterLookup[area] = counter;
                grid.Children.Add(counter);
            }
        }

        public void Refresh()
        {
            int sum = 0;
            foreach (string area in MapArea.AllMapAreas)
            {
                if (counterLookup.TryGetValue(area, out TextFormatter<int> counter)
                    && model.ItemByAreaCounter.TryGetValue(area, out int count))
                {
                    counter.Data = count;
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
