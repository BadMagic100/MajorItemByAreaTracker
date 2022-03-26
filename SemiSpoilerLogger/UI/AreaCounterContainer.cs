using ConnectionMetadataInjector.Util;
using MagicUI.Core;
using MagicUI.Elements;
using RandomizerMod;
using UnityEngine;

namespace MajorItemByAreaTracker.UI
{
    public class AreaCounterContainer : Container
    {
        private TextObject? Text => Child as TextObject;
        private readonly string area;

        private int count = 0;
        public int Count
        {
            get => count;
            set
            {
                if (count != value)
                {
                    count = value;
                    if (Text != null)
                    {
                        Text.Text = FormatText();
                    }
                }
            }
        }

        private string TryLocalizeArea()
        {
            try
            {
                return Localization.Localize(area);
            }
            catch
            {
                return area;
            }
        }

        private string FormatText() => $"{TryLocalizeArea()}: {Count}";

        public AreaCounterContainer(LayoutRoot onLayout, string area) : base(onLayout, $"{area} Counter")
        {
            if (area == MapArea.FORGOTTEN_CROSSROADS)
            {
                this.area = "Crossroads";
            }
            else
            {
                this.area = area;
            }

            HorizontalAlignment = HorizontalAlignment.Center;

            Child = new TextObject(onLayout, $"{area} Text")
            {
                Text = FormatText(),
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
        }


        protected override Vector2 MeasureOverride()
        {
            return Child?.Measure() ?? Vector2.zero;
        }

        protected override void ArrangeOverride(Vector2 alignedTopLeftCorner)
        {
            Child?.Arrange(new Rect(alignedTopLeftCorner, Child.EffectiveSize));
        }

        protected override void DestroyOverride()
        {
            Child?.Destroy();
            Child = null;
        }
    }
}
