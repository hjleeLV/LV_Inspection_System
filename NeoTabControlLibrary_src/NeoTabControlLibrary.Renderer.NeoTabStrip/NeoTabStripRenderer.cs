using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.NeoTabStrip
{
    [AddInRenderer("NeoTabStrip",
        "NeoTabStrip renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText, Hover style is not implemented.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class NeoTabStripRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static NeoTabStripRenderer()
        {
            MY_FONT = new Font("Tahoma", 8.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                SystemColors.Control,
                // TabPageItemForeColor
                Color.Black,
                // SelectedTabPageItemForeColor
                Color.Black,
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.Empty
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 5, 0 };
        }

        #endregion

        public override bool IsSupportSmartCloseButton
        {
            get
            {
                return true;
            }
        }

        public override bool IsSupportSmartDropDownButton
        {
            get
            {
                return true;
            }
        }

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
            using (Pen pen = new Pen(SystemColors.ControlDark))
                gfx.DrawRectangle(pen, linesRct);
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle itemRct = tabPageItemRct;
            SmoothingMode mode = gfx.SmoothingMode;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                using (GraphicsPath path = new GraphicsPath())
                {
                    bool isSelected = false;
                    Color textColor = DisabledTabPageItemForeColor;
                    switch (btnState)
                    {
                        case CommonObjects.ButtonState.Hover:
                        case CommonObjects.ButtonState.Normal:
                            textColor = TabPageItemForeColor;
                            goto case CommonObjects.ButtonState.Disabled;
                        case CommonObjects.ButtonState.Pressed:
                            isSelected = true;
                            textColor = SelectedTabPageItemForeColor;
                            using (Brush brush = new SolidBrush(BackColor))
                                gfx.FillRectangle(brush, tabPageItemRct.Left - 1, tabPageItemRct.Top - 1, tabPageItemRct.Width + 1, tabPageItemRct.Height + 1);
                            goto case CommonObjects.ButtonState.Disabled;
                        case CommonObjects.ButtonState.Disabled:
                            int xOffset;
                            if (index == 0)
                            {
                                xOffset = itemRct.Left + 10 + (itemRct.Height / 2);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left, itemRct.Bottom),
                                    new Point(itemRct.Left, itemRct.Bottom - 4),
                                    new Point(itemRct.Left + 2, itemRct.Bottom - 11),
                                    new Point(xOffset, itemRct.Top),
                                };
                                path.AddBeziers(points);
                            }
                            else if (isSelected)
                            {
                                xOffset = itemRct.Left + (itemRct.Height / 2);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left - 10, itemRct.Bottom),
                                    new Point(itemRct.Left - 10, itemRct.Bottom - 4),
                                    new Point(itemRct.Left - 8, itemRct.Bottom - 11),
                                    new Point(xOffset, itemRct.Top),
                                };
                                path.AddBeziers(points);
                            }
                            else
                            {
                                xOffset = itemRct.Left + (itemRct.Height / 2);
                                path.AddLine(itemRct.Left, itemRct.Bottom, itemRct.Left,
                                    itemRct.Top + (itemRct.Height / 2) - 3);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left, itemRct.Top + (itemRct.Height / 2) - 4),
                                    new Point(itemRct.Left, itemRct.Top + (itemRct.Height / 2) - 5),
                                    new Point(itemRct.Left + 2, itemRct.Top + 2),
                                    new Point(xOffset, itemRct.Top),
                                };
                                path.AddBeziers(points);
                            }
                            path.AddLine(xOffset + 1, itemRct.Top, itemRct.Right - 5, itemRct.Top);
                            path.AddLine(itemRct.Right - 1, itemRct.Top + 2, itemRct.Right - 1, itemRct.Bottom);
                            path.CloseFigure();
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(248, 247, 242),
                                isSelected ? Color.White : Color.FromArgb(233, 233, 216), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.4F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillPath(brush, path);
                            }
                            using (Pen pen = new Pen(SystemColors.ControlDark))
                            {
                                gfx.DrawPath(pen, path);
                                if(isSelected)
                                {
                                    pen.Color = Color.White;
                                    gfx.DrawLine(pen, index == 0 ? itemRct.Left + 1 : itemRct.Left - 9, itemRct.Bottom, itemRct.Right - 2, itemRct.Bottom);
                                }
                            }
                            using (Font font = new Font(NeoTabPageItemsFont, isSelected ? FontStyle.Bold : FontStyle.Regular))
                            {
                                itemRct.X += 2;
                                itemRct.Width -= 2;
                                itemRct.Y += 2;
                                itemRct.Height -= 2;
                                if (index == 0)
                                {
                                    itemRct.X += 6;
                                    itemRct.Width -= 6;
                                }
                                using (Brush brush = new SolidBrush(textColor))
                                    gfx.DrawString(tabPageText, font, brush, itemRct, format);
                            }
                            break;
                    }
                }
            }
            gfx.SmoothingMode = mode;
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
            get { return TabPageItemStyle.OnlyText; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    [AddInRenderer("NeoTabStrip",
        "NeoTabStrip renderer class, TabPageLayout: Top, TabPageItemStyle: TextAnd16x16_Image, Hover style is not implemented.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.1.0.0")]
    public sealed class NeoTabStripRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static NeoTabStripRendererVS2()
        {
            MY_FONT = new Font("Tahoma", 8.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                SystemColors.Control,
                // TabPageItemForeColor
                Color.Black,
                // SelectedTabPageItemForeColor
                Color.Black,
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.Empty
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 4, 0 };
        }

        #endregion

        public override bool IsSupportSmartCloseButton
        {
            get
            {
                return true;
            }
        }

        public override bool IsSupportSmartDropDownButton
        {
            get
            {
                return true;
            }
        }

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
            using (Pen pen = new Pen(SystemColors.ControlDark))
                gfx.DrawRectangle(pen, linesRct);
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle itemRct = tabPageItemRct;
            SmoothingMode mode = gfx.SmoothingMode;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            Bitmap itemBitmap = null;
            switch (tabPageText.ToUpperInvariant())
            {
                case "SOLUTION &EXPLORER":
                    itemBitmap = Resources.AddToFavoritesHS;
                    break;
                case "PRO&PERTIES":
                    itemBitmap = Resources.AlignTableCellMiddleLeftJustHS;
                    break;
                case "&TOOLBOX":
                    itemBitmap = Resources.compareversionsHS;
                    break;
                case "ERROR &LIST":
                    itemBitmap = Resources.NewWindow;
                    break;
                case "&OUTPUT":
                    itemBitmap = Resources.XSDSchema_RemoveAllButSelectionFromWorkspaceCmd;
                    break;
                default:
                    itemBitmap = Resources.AddToFavoritesHS;
                    break;
            }
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                using (GraphicsPath path = new GraphicsPath())
                {
                    bool isSelected = false;
                    Color textColor = DisabledTabPageItemForeColor;
                    switch (btnState)
                    {
                        case CommonObjects.ButtonState.Hover:
                        case CommonObjects.ButtonState.Normal:
                            textColor = TabPageItemForeColor;
                            goto case CommonObjects.ButtonState.Disabled;
                        case CommonObjects.ButtonState.Pressed:
                            isSelected = true;
                            textColor = SelectedTabPageItemForeColor;
                            using (Brush brush = new SolidBrush(BackColor))
                                gfx.FillRectangle(brush, tabPageItemRct.Left - 1, tabPageItemRct.Top - 1, tabPageItemRct.Width + 1, tabPageItemRct.Height + 1);
                            goto case CommonObjects.ButtonState.Disabled;
                        case CommonObjects.ButtonState.Disabled:
                            int xOffset;
                            if (index == 0)
                            {
                                xOffset = itemRct.Left + 10 + (itemRct.Height / 2);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left, itemRct.Bottom),
                                    new Point(itemRct.Left, itemRct.Bottom - 4),
                                    new Point(itemRct.Left + 2, itemRct.Bottom - 11),
                                    new Point(xOffset, itemRct.Top),
                                };
                                path.AddBeziers(points);
                            }
                            else if (isSelected)
                            {
                                xOffset = itemRct.Left + (itemRct.Height / 2);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left - 10, itemRct.Bottom),
                                    new Point(itemRct.Left - 10, itemRct.Bottom - 4),
                                    new Point(itemRct.Left - 8, itemRct.Bottom - 11),
                                    new Point(xOffset, itemRct.Top),
                                };
                                path.AddBeziers(points);
                            }
                            else
                            {
                                xOffset = itemRct.Left + (itemRct.Height / 2);
                                path.AddLine(itemRct.Left, itemRct.Bottom, itemRct.Left,
                                    itemRct.Top + (itemRct.Height / 2) - 3);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left, itemRct.Top + (itemRct.Height / 2) - 4),
                                    new Point(itemRct.Left, itemRct.Top + (itemRct.Height / 2) - 5),
                                    new Point(itemRct.Left + 2, itemRct.Top + 2),
                                    new Point(xOffset, itemRct.Top),
                                };
                                path.AddBeziers(points);
                            }
                            path.AddLine(xOffset + 1, itemRct.Top, itemRct.Right - 5, itemRct.Top);
                            path.AddLine(itemRct.Right - 1, itemRct.Top + 2, itemRct.Right - 1, itemRct.Bottom);
                            path.CloseFigure();
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(248, 247, 242),
                                isSelected ? Color.White : Color.FromArgb(233, 233, 216), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.4F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillPath(brush, path);
                            }
                            using (Pen pen = new Pen(SystemColors.ControlDark))
                            {
                                gfx.DrawPath(pen, path);
                                if (isSelected)
                                {
                                    pen.Color = Color.White;
                                    gfx.DrawLine(pen, index == 0 ? itemRct.Left + 1 : itemRct.Left - 9, itemRct.Bottom, itemRct.Right - 2, itemRct.Bottom);
                                }
                            }
                            gfx.DrawImage(itemBitmap, xOffset - 3, itemRct.Top + ItemObjectsDrawingMargin + 1, itemBitmap.Width, itemBitmap.Height);
                            xOffset = ItemObjectsDrawingMargin + itemBitmap.Width;
                            itemRct.X += xOffset;
                            itemRct.Width -= xOffset;
                            using (Font font = new Font(NeoTabPageItemsFont, isSelected ? FontStyle.Bold : FontStyle.Regular))
                            {
                                itemRct.X += 2;
                                itemRct.Width -= 2;
                                itemRct.Y += 2;
                                itemRct.Height -= 2;
                                if (index == 0)
                                {
                                    itemRct.X += 10;
                                    itemRct.Width -= 10;
                                }
                                using (Brush brush = new SolidBrush(textColor))
                                    gfx.DrawString(tabPageText, font, brush, itemRct, format);
                            }
                            break;
                    }
                }
            }
            gfx.SmoothingMode = mode;
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
}