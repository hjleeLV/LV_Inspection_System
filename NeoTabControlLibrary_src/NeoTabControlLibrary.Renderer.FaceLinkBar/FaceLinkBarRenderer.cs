using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.FaceLinkBar
{
    [AddInRenderer("FaceLinkBar",
        "FaceLinkBar renderer class, TabPageLayout: Top, TabPageItemStyle: TextAnd16x16_Image.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class FaceLinkBarRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static FaceLinkBarRenderer()
        {
            MY_FONT = new Font("Tahoma", 8.45f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 4, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.White,
                // TabPageItemForeColor
                Color.FromArgb(59, 89, 152),
                // SelectedTabPageItemForeColor
                Color.FromArgb(51, 51, 51),
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.FromArgb(59, 89, 152)
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 6, 0 };
        }

        #endregion

        public override void InvokeEditor()
        {
            throw new NotImplementedException();
        }

        public override void OnRendererBackground(Graphics gfx, Rectangle clientRct)
        {
            // Do nothing.
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            Rectangle linesRct = tabPageAreaRct;
            linesRct.Width -= 1;
            linesRct.Height -= 1;
            // Draw 1px border rectangle.
            using (Pen pen = new Pen(Color.FromArgb(180, 187, 205)))
                gfx.DrawRectangle(pen, linesRct);
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle itemRct = tabPageItemRct;
            Bitmap itemBitmap = null;
            switch (tabPageText.ToUpperInvariant())
            {
                case "SOLUTION &EXPLORER":
                    itemBitmap = Resources.AddTableHS;
                    break;
                case "PRO&PERTIES":
                    itemBitmap = Resources.FullScreenHS;
                    break;
                case "&TOOLBOX":
                    itemBitmap = Resources.PushpinHS;
                    break;
                case "ERROR &LIST":
                    itemBitmap = Resources.SendToBackHS;
                    break;
                case "&OUTPUT":
                    itemBitmap = Resources.ViewThumbnailsHS;
                    break;
                default:
                    itemBitmap = Resources.AddTableHS;
                    break;
            }
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                int xoffset;
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                        gfx.DrawImage(itemBitmap, itemRct.Left + ItemObjectsDrawingMargin, itemRct.Top + ItemObjectsDrawingMargin, itemBitmap.Width, itemBitmap.Height);
                        xoffset = itemBitmap.Width;
                        itemRct.X += xoffset;
                        itemRct.Width -= xoffset;
                        using (Font font = new Font(NeoTabPageItemsFont, FontStyle.Bold | FontStyle.Underline))
                        {
                            using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                                gfx.DrawString(tabPageText, font, brush, itemRct, format);
                        }
                        break;
                    case CommonObjects.ButtonState.Normal:
                        gfx.DrawImage(itemBitmap, itemRct.Left + ItemObjectsDrawingMargin, itemRct.Top + ItemObjectsDrawingMargin, itemBitmap.Width, itemBitmap.Height);
                        xoffset = itemBitmap.Width;
                        itemRct.X += xoffset;
                        itemRct.Width -= xoffset;
                        using (Font font = new Font(NeoTabPageItemsFont, FontStyle.Bold))
                        {
                            using (Brush brush = new SolidBrush(TabPageItemForeColor))
                                gfx.DrawString(tabPageText, font, brush, itemRct, format);
                        }
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (Brush brush = new SolidBrush(BackColor))
                            gfx.FillRectangle(brush, itemRct);
                        gfx.DrawImage(itemBitmap, itemRct.Left + ItemObjectsDrawingMargin, itemRct.Top + ItemObjectsDrawingMargin, itemBitmap.Width, itemBitmap.Height);
                        Bitmap triangle = Resources.Triangle;
                        gfx.DrawImage(triangle, itemRct.Left + 4, itemRct.Bottom - 1, triangle.Width, triangle.Height);
                        xoffset = itemBitmap.Width;
                        itemRct.X += xoffset;
                        itemRct.Width -= xoffset;
                        using (Font font = new Font(NeoTabPageItemsFont, FontStyle.Bold))
                        {
                            using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                                gfx.DrawString(tabPageText, font, brush, itemRct, format);
                        }
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        gfx.DrawImage(itemBitmap, itemRct.Left + ItemObjectsDrawingMargin, itemRct.Top + ItemObjectsDrawingMargin, itemBitmap.Width, itemBitmap.Height);
                        xoffset = itemBitmap.Width;
                        itemRct.X += xoffset;
                        itemRct.Width -= xoffset;
                        using (Font font = new Font(NeoTabPageItemsFont, FontStyle.Bold))
                        {
                            using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                                gfx.DrawString(tabPageText, font, brush, itemRct, format);
                        }
                        break;
                }
            }
        }

        public override int ItemObjectsDrawingMargin
        {
            get { return INTEGERARRAY[0]; }
        }

        public override int TabPageItemsBetweenSpacing
        {
            get { return INTEGERARRAY[1]; }
        }

        public override Color BackColor
        {
            get { return COLORS[0]; }
        }

        public override Color TabPageItemForeColor
        {
            get { return COLORS[1]; }
        }

        public override Color SelectedTabPageItemForeColor
        {
            get { return COLORS[2]; }
        }

        public override Color DisabledTabPageItemForeColor
        {
            get { return COLORS[3]; }
        }

        public override Color MouseOverTabPageItemForeColor
        {
            get { return COLORS[4]; }
        }

        public override Font NeoTabPageItemsFont
        {
            get { return MY_FONT; }
        }

        public override DrawingOffset TabPageAreaCornerOffset
        {
            get { return OFFSETS[0]; }
        }

        public override DrawingOffset TabPageItemsAreaOffset
        {
            get { return OFFSETS[1]; }
        }

        public override TabPageLayout NeoTabPageItemsSide
        {
            get { return TabPageLayout.Top; }
        }

        public override TabPageItemStyle NeoTabPageItemsStyle
        {
            get { return TabPageItemStyle.TextAnd16x16_Image; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    [AddInRenderer("FaceLinkBar",
        "FaceLinkBar renderer class with left alignment, TabPageLayout: Left, TabPageItemStyle: TextAnd16x16_Image.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.1.0.0")]
    public sealed class FaceLinkBarRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static FaceLinkBarRendererVS2()
        {
            MY_FONT = new Font("Tahoma", 8.45f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, -8) };

            COLORS = new Color[]{
                // BackColor
                Color.White,
                // TabPageItemForeColor
                Color.FromArgb(59, 89, 152),
                // SelectedTabPageItemForeColor
                Color.FromArgb(51, 51, 51),
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.FromArgb(59, 89, 152)
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 6, 0 };
        }

        #endregion

        public override void InvokeEditor()
        {
            throw new NotImplementedException();
        }

        public override void OnRendererBackground(Graphics gfx, Rectangle clientRct)
        {
            // Do nothing.
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            Rectangle linesRct = tabPageAreaRct;
            linesRct.Width -= 1;
            linesRct.Height -= 1;
            // Draw 1px border rectangle.
            using (Pen pen = new Pen(Color.FromArgb(180, 187, 205)))
                gfx.DrawRectangle(pen, linesRct);
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle itemRct = tabPageItemRct;
            Bitmap itemBitmap = null;
            switch (tabPageText.ToUpperInvariant())
            {
                case "SOLUTION &EXPLORER":
                    itemBitmap = Resources.AddTableHS;
                    break;
                case "PRO&PERTIES":
                    itemBitmap = Resources.FullScreenHS;
                    break;
                case "&TOOLBOX":
                    itemBitmap = Resources.PushpinHS;
                    break;
                case "ERROR &LIST":
                    itemBitmap = Resources.SendToBackHS;
                    break;
                case "&OUTPUT":
                    itemBitmap = Resources.ViewThumbnailsHS;
                    break;
                default:
                    itemBitmap = Resources.AddTableHS;
                    break;
            }
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                int xoffset;
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                        gfx.DrawImage(itemBitmap, itemRct.Left + ItemObjectsDrawingMargin, itemRct.Top + ItemObjectsDrawingMargin, itemBitmap.Width, itemBitmap.Height);
                        xoffset = itemBitmap.Width + ItemObjectsDrawingMargin + 4;
                        itemRct.X += xoffset;
                        itemRct.Width -= xoffset;
                        using (Font font = new Font(NeoTabPageItemsFont, FontStyle.Bold | FontStyle.Underline))
                        {
                            using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                                gfx.DrawString(tabPageText, font, brush, itemRct, format);
                        }
                        break;
                    case CommonObjects.ButtonState.Normal:
                        gfx.DrawImage(itemBitmap, itemRct.Left + ItemObjectsDrawingMargin, itemRct.Top + ItemObjectsDrawingMargin, itemBitmap.Width, itemBitmap.Height);
                        xoffset = itemBitmap.Width + ItemObjectsDrawingMargin + 4;
                        itemRct.X += xoffset;
                        itemRct.Width -= xoffset;
                        using (Font font = new Font(NeoTabPageItemsFont, FontStyle.Bold))
                        {
                            using (Brush brush = new SolidBrush(TabPageItemForeColor))
                                gfx.DrawString(tabPageText, font, brush, itemRct, format);
                        }
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (Brush brush = new SolidBrush(BackColor))
                            gfx.FillRectangle(brush, itemRct.Left, itemRct.Top, itemRct.Width - 8, itemRct.Height);
                        gfx.DrawImage(itemBitmap, itemRct.Left + ItemObjectsDrawingMargin, itemRct.Top + ItemObjectsDrawingMargin, itemBitmap.Width, itemBitmap.Height);
                        xoffset = itemBitmap.Width + ItemObjectsDrawingMargin + 4;
                        itemRct.X += xoffset;
                        itemRct.Width -= xoffset;
                        using (Font font = new Font(NeoTabPageItemsFont, FontStyle.Bold))
                        {
                            using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                                gfx.DrawString(tabPageText, font, brush, itemRct, format);
                        }
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        gfx.DrawImage(itemBitmap, itemRct.Left + ItemObjectsDrawingMargin, itemRct.Top + ItemObjectsDrawingMargin, itemBitmap.Width, itemBitmap.Height);
                        xoffset = itemBitmap.Width + ItemObjectsDrawingMargin + 4;
                        itemRct.X += xoffset;
                        itemRct.Width -= xoffset;
                        using (Font font = new Font(NeoTabPageItemsFont, FontStyle.Bold))
                        {
                            using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                                gfx.DrawString(tabPageText, font, brush, itemRct, format);
                        }
                        break;
                }
            }
        }

        public override int ItemObjectsDrawingMargin
        {
            get { return INTEGERARRAY[0]; }
        }

        public override int TabPageItemsBetweenSpacing
        {
            get { return INTEGERARRAY[1]; }
        }

        public override Color BackColor
        {
            get { return COLORS[0]; }
        }

        public override Color TabPageItemForeColor
        {
            get { return COLORS[1]; }
        }

        public override Color SelectedTabPageItemForeColor
        {
            get { return COLORS[2]; }
        }

        public override Color DisabledTabPageItemForeColor
        {
            get { return COLORS[3]; }
        }

        public override Color MouseOverTabPageItemForeColor
        {
            get { return COLORS[4]; }
        }

        public override Font NeoTabPageItemsFont
        {
            get { return MY_FONT; }
        }

        public override DrawingOffset TabPageAreaCornerOffset
        {
            get { return OFFSETS[0]; }
        }

        public override DrawingOffset TabPageItemsAreaOffset
        {
            get { return OFFSETS[1]; }
        }

        public override TabPageLayout NeoTabPageItemsSide
        {
            get { return TabPageLayout.Left; }
        }

        public override TabPageItemStyle NeoTabPageItemsStyle
        {
            get { return TabPageItemStyle.TextAnd16x16_Image; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}