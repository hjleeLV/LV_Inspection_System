using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.MenuBar
{
    [AddInRenderer("MenuBar",
    "MenuBar renderer class, TabPageLayout: Top(bottom margin 6px), TabPageItemStyle: TextAnd16x16_Image.",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class MenuBarRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static MenuBarRenderer()
        {
            MY_FONT = new Font("Verdana", 8.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(2, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 6, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.White,
                // TabPageItemForeColor
                Color.FromArgb(206, 221, 240),
                // SelectedTabPageItemForeColor
                Color.FromArgb(206, 221, 240),
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.FromArgb(206, 221, 240)
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 12, 0 };
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
            // Draw top 2px line.
            using (Brush brush = new SolidBrush(Color.FromArgb(2, 111, 194)))
                gfx.FillRectangle(brush, linesRct.Left, linesRct.Top, linesRct.Width, 2);
            // Draw left, right and bottom 1px lines.
            using (Pen pen = new Pen(Color.FromArgb(183, 192, 197)))
            {
                // Create border points.
                Point[] points = new Point[]
                    {
                        // Left point.
                        new Point(linesRct.Left, linesRct.Top + 2),
                        // Bottom points.
                        new Point(linesRct.Left, linesRct.Bottom - 1),
                        new Point(linesRct.Right - 1, linesRct.Bottom - 1),
                        // Right point.
                        new Point(linesRct.Right - 1, linesRct.Top + 2)
                    };
                gfx.DrawLines(pen, points);
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            Bitmap bmp = null;
            using (ImageAttributes attributes = new ImageAttributes())
            {
                /* Draw menu item bitmaps. */
                for (int i = 0; i <= 2; i++)
                {
                    switch (i)
                    {
                        case 0:
                            bmp = Resources.menu_back_left;
                            rct = new Rectangle(rct.Left, rct.Top, bmp.Width, rct.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 1:
                            bmp = Resources.menu_back;
                            rct = new Rectangle(rct.Left + 12, rct.Top, rct.Width - 24, rct.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 2:
                            bmp = Resources.menu_back_right;
                            rct = new Rectangle(rct.Right - 12, rct.Top, bmp.Width, rct.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                    }
                    rct = tabPageItemRct;
                    attributes.SetWrapMode(WrapMode.Clamp);
                }
            }
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                bmp = Resources.DropDown;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                    case CommonObjects.ButtonState.Pressed:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            ColorMap[] map = new ColorMap[2];
                            map[0] = new ColorMap();
                            map[0].OldColor = Color.White;
                            map[0].NewColor = Color.Transparent;
                            map[1] = new ColorMap();
                            map[1].OldColor = Color.Black;
                            map[1].NewColor = Color.White;
                            attributes.SetRemapTable(map);
                            rct = new Rectangle(rct.Right - ItemObjectsDrawingMargin - bmp.Width - 1, rct.Height / 2 - bmp.Height / 2,
                                    bmp.Width, bmp.Height);
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            rct = tabPageItemRct;
                            rct.Width -= (ItemObjectsDrawingMargin + bmp.Width);
                            using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                                gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        }
                        break;
                    case CommonObjects.ButtonState.Normal:
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
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

    [AddInRenderer("MenuBar",
    "MenuBar renderer class, TabPageLayout: Left(right margin 6px), TabPageItemStyle: TextAnd16x16_Image.",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.1.0.0")]
    public sealed class MenuBarRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static MenuBarRendererVS2()
        {
            MY_FONT = new Font("Verdana", 8.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(2, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, 6) };

            COLORS = new Color[]{
                // BackColor
                Color.White,
                // TabPageItemForeColor
                Color.FromArgb(206, 221, 240),
                // SelectedTabPageItemForeColor
                Color.FromArgb(206, 221, 240),
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.FromArgb(206, 221, 240)
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 12, 2 };
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
            // Draw top 2px line.
            using (Brush brush = new SolidBrush(Color.FromArgb(2, 111, 194)))
                gfx.FillRectangle(brush, linesRct.Left, linesRct.Top, linesRct.Width, 2);
            // Draw left, right and bottom 1px lines.
            using (Pen pen = new Pen(Color.FromArgb(183, 192, 197)))
            {
                // Create border points.
                Point[] points = new Point[]
                    {
                        // Left point.
                        new Point(linesRct.Left, linesRct.Top + 2),
                        // Bottom points.
                        new Point(linesRct.Left, linesRct.Bottom - 1),
                        new Point(linesRct.Right - 1, linesRct.Bottom - 1),
                        // Right point.
                        new Point(linesRct.Right - 1, linesRct.Top + 2)
                    };
                gfx.DrawLines(pen, points);
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            Bitmap bmp = null;
            using (ImageAttributes attributes = new ImageAttributes())
            {
                /* Draw menu item bitmaps. */
                for (int i = 0; i <= 2; i++)
                {
                    switch (i)
                    {
                        case 0:
                            bmp = Resources.menu_back_left;
                            rct = new Rectangle(rct.Left, rct.Top, bmp.Width, rct.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 1:
                            bmp = Resources.menu_back;
                            rct = new Rectangle(rct.Left + 12, rct.Top, rct.Width - 24, rct.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 2:
                            bmp = Resources.menu_back_right;
                            rct = new Rectangle(rct.Right - 12, rct.Top, bmp.Width, rct.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                    }
                    rct = tabPageItemRct;
                    attributes.SetWrapMode(WrapMode.Clamp);
                }
            }
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Far;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                bmp = Resources.DropDownRight;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                    case CommonObjects.ButtonState.Pressed:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            ColorMap[] map = new ColorMap[2];
                            map[0] = new ColorMap();
                            map[0].OldColor = Color.White;
                            map[0].NewColor = Color.Transparent;
                            map[1] = new ColorMap();
                            map[1].OldColor = Color.Black;
                            map[1].NewColor = Color.White;
                            attributes.SetRemapTable(map);
                            rct = new Rectangle(rct.Right - ItemObjectsDrawingMargin - bmp.Width - 1, rct.Top + rct.Height / 2 - bmp.Height / 2,
                                    bmp.Width, bmp.Height);
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            rct = tabPageItemRct;
                            rct.Width -= (ItemObjectsDrawingMargin + 20);
                            using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                                gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        }
                        break;
                    case CommonObjects.ButtonState.Normal:
                        rct.Width -= (ItemObjectsDrawingMargin + 20);
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        rct.Width -= (ItemObjectsDrawingMargin + 20);
                        using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
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