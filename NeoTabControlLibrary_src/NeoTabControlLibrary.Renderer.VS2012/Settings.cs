using System;
using System.Drawing;

namespace NeoTabControlLibrary.Renderer.VS2012
{
    public class Settings : ICloneable
    {
        #region Enum

        public enum PreLoadedStyles
        {
            Red,
            Blue,
            Green,
            OliveGreen
        };

        #endregion

        private PreLoadedStyles loadedStyles = PreLoadedStyles.Red;

        #region Properties

        public Font NeoTabPageItemsFont { get; set; }
        public Color BackColor { get; set; }
        public Color TabPageItemForeColor { get; set; }
        public Color SelectedTabPageItemForeColor { get; set; }
        public Color DisabledTabPageItemForeColor { get; set; }
        public Color MouseOverTabPageItemForeColor { get; set; }
        public Color TabItemFirstColor { get; set; }
        public Color TabItemSecondColor { get; set; }
        public Color TabItemHoverFirstColor { get; set; }
        public Color TabItemHoverSecondColor { get; set; }
        public int ItemObjectsDrawingMargin { get; set; }
        public int TabPageItemsBetweenSpacing { get; set; }
        public PreLoadedStyles LoadedStyles
        {
            get { return loadedStyles; }
            set
            {
                if (!value.Equals(loadedStyles))
                {
                    loadedStyles = value;
                    switch (loadedStyles)
                    {
                        default:
                            this.BackColor = Color.FromArgb(85, 41, 41);
                            this.TabItemFirstColor = Color.FromArgb(130, 77, 77);
                            this.TabItemSecondColor = Color.FromArgb(120, 63, 63);
                            this.TabItemHoverFirstColor = Color.FromArgb(140, 85, 85);
                            this.TabItemHoverSecondColor = Color.FromArgb(130, 75, 75);
                            break;
                        case PreLoadedStyles.Blue:
                            this.BackColor = Color.FromArgb(41, 57, 85);
                            this.TabItemFirstColor = Color.FromArgb(77, 96, 130);
                            this.TabItemSecondColor = Color.FromArgb(61, 82, 119);
                            this.TabItemHoverFirstColor = Color.FromArgb(87, 105, 138);
                            this.TabItemHoverSecondColor = Color.FromArgb(71, 95, 128);
                            break;
                        case PreLoadedStyles.Green:
                            this.BackColor = Color.FromArgb(49, 77, 52);
                            this.TabItemFirstColor = Color.FromArgb(87, 120, 90);
                            this.TabItemSecondColor = Color.FromArgb(72, 108, 76);
                            this.TabItemHoverFirstColor = Color.FromArgb(97, 128, 98);
                            this.TabItemHoverSecondColor = Color.FromArgb(82, 118, 86);
                            break;
                        case PreLoadedStyles.OliveGreen:
                            this.BackColor = Color.FromArgb(85, 74, 41);
                            this.TabItemFirstColor = Color.FromArgb(130, 116, 77);
                            this.TabItemSecondColor = Color.FromArgb(119, 104, 61);
                            this.TabItemHoverFirstColor = Color.FromArgb(140, 124, 87);
                            this.TabItemHoverSecondColor = Color.FromArgb(130, 114, 71);
                            break;
                    }
                    this.TabPageItemForeColor = Color.White;
                    this.SelectedTabPageItemForeColor = Color.White;
                    this.DisabledTabPageItemForeColor = SystemColors.GrayText;
                    this.MouseOverTabPageItemForeColor = Color.White;
                }
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            Settings toBeCloned = new Settings();
            toBeCloned.NeoTabPageItemsFont = this.NeoTabPageItemsFont;
            toBeCloned.BackColor = this.BackColor;
            toBeCloned.TabPageItemForeColor = this.TabPageItemForeColor;
            toBeCloned.SelectedTabPageItemForeColor = this.SelectedTabPageItemForeColor;
            toBeCloned.DisabledTabPageItemForeColor = this.DisabledTabPageItemForeColor;
            toBeCloned.MouseOverTabPageItemForeColor = this.MouseOverTabPageItemForeColor;
            toBeCloned.TabItemFirstColor = this.TabItemFirstColor;
            toBeCloned.TabItemSecondColor = this.TabItemSecondColor;
            toBeCloned.TabItemHoverFirstColor = this.TabItemHoverFirstColor;
            toBeCloned.TabItemHoverSecondColor = this.TabItemHoverSecondColor;
            toBeCloned.ItemObjectsDrawingMargin = this.ItemObjectsDrawingMargin;
            toBeCloned.TabPageItemsBetweenSpacing = this.TabPageItemsBetweenSpacing;
            return toBeCloned;
        }

        #endregion
    }
}