using ConnectionMetadataInjector.Util;
using MagicUI.Core;
using MagicUI.Elements;
using MajorItemByAreaTracker.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ItemChanger;
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
        private Dictionary<string, TextFormatter<int>?> optionalCounterLookup = new();
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
                counterLookup[area] = CreateCounter(grid, area);
            }
            optionalCounterLookup.Add(SubcategoryFinder.OTHER, null);
        }

        private TextFormatter<int> CreateCounter(DynamicUniformGrid? parent, string area)
        {
            TextFormatter<int> counter = new(layout, 0, (i) => $"{TryLocalize(area)}: {i}")
            {
                Text = new(layout)
                {
                    FontSize = 15
                },
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            parent!.Children.Add(counter);
            return counter;
        }

        public void Refresh()
        {
            int sum = 0;
            foreach (var pair in counterLookup)
            {
                if (model.ItemByAreaCounter.TryGetValue(pair.Key, out int count))
                {
                    pair.Value.Data = count;
                    sum += count;
                }
            }
            List<(string, TextFormatter<int>)> queuedChanges = new();
            foreach (string area in optionalCounterLookup.Keys)
            {
                if (model.ItemByAreaCounter.TryGetValue(area, out int count))
                {
                    TextFormatter<int> counter = optionalCounterLookup[area]
                        ?? CreateCounter(layout.GetElement<DynamicUniformGrid>("Area Tracker Grid"), area);
                    counter.Data = count;
                    sum += count;
                    if (optionalCounterLookup[area] == null)
                    {
                        queuedChanges.Add((area, counter));
                    }
                }
            }
            foreach ((string area, TextFormatter<int> counter) in queuedChanges)
            {
                optionalCounterLookup[area] = counter;
            }
            SetLabelText(sum);
        }

        private void SetLabelText(int total)
        {
            TextObject? label = layout?.GetElement<TextObject>("Area Tracker Label");
            if (label != null)
            {
                label.Text = $"{MainLabel()} Remaining ({total})";
            }
        }

        private string MainLabel()
        {
            bool grubsOnly = model.ItemByNameCounter.Count == 1 && model.ItemByNameCounter.ContainsKey(ItemNames.Grub);
            if (!grubsOnly)
            {
                return "Major Items";
            }
            if (MajorItemByAreaTracker.Instance.GS.CuteSpelling)
            {
                return "Grubbies";
            }
            return "Grubs";
        }

        public void Destroy()
        {
            layout.Destroy();
        }
    }
}
