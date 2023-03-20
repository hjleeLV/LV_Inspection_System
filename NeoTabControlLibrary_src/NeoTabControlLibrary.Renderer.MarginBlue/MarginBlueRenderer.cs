using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.MarginBlue
{
    [AddInRenderer("MarginBlue",
        "MarginBlue renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class MarginBlueRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static MarginBlueRenderer()
        {
            MY_FONT = new Font("Arial", 9.25f);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(2, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 4, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.White,
                // TabPageItemForeColor
                Color.Black,
                // SelectedTabPageItemForeColor
                Color.White,
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.Black
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 8, 4 };
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
            Rectangle itemRct = tabPageItemRct;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(155, 179, 217),
                            Color.FromArgb(148, 188, 223), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.4F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        itemRct.Width -= 1;
                        itemRct.Height -= 1;
                        // Draw a border rectangle 1px line.
                        using (Pen pen = new Pen(Color.FromArgb(242, 242, 242)))
                            gfx.DrawRectangle(pen, itemRct);
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Normal:
                        using (Brush brush = new SolidBrush(Color.White))
                            gfx.FillRectangle(brush, itemRct);
                        itemRct.Width -= 1;
                        itemRct.Height -= 1;
                        // Draw a border rectangle 1px line.
                        using (Pen pen = new Pen(Color.FromArgb(242, 242, 242)))
                            gfx.DrawRectangle(pen, itemRct);
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(1, 110, 193),
                            Color.FromArgb(4, 111, 191), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (Brush brush = new SolidBrush(SystemColors.InactiveBorder))
                            gfx.FillRectangle(brush, itemRct);
                        itemRct.Width -= 1;
                        itemRct.Height -= 1;
                        // Draw a border rectangle 1px line.
                        using (Pen pen = new Pen(Color.FromArgb(242, 242, 242)))
                            gfx.DrawRectangle(pen, itemRct);
                        using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
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
            get { return TabPageItemStyle.OnlyText; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    [AddInRenderer("MarginBlue",
    "MarginBlue renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText(uppercase item strings), Version: 2.0.0.0",
    DeveloperName = "Burak Özdiken", VersionNumber = "2.0.0.0")]
    public sealed class MarginBlueRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static MarginBlueRendererVS2()
        {
            MY_FONT = new Font("Arial", 9.25f);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(2, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                SystemColors.Control,
                // TabPageItemForeColor
                Color.Black,
                // SelectedTabPageItemForeColor
                Color.White,
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.Black
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 10, 1 };
        }

        #endregion
        
        public override bool IsSupportSmartCloseButton
        {
            get { return true; }
        }

        public override bool IsSupportSmartDropDownButton
        {
            get { return true; }
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
            tabPageText = tabPageText.ToUpperInvariant();
            Rectangle itemRct = tabPageItemRct;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                        itemRct.Y += 1;
                        itemRct.Height -= 1;
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(155, 179, 217),
                            Color.FromArgb(148, 188, 223), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.4F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        // Draw left, top and right 1px lines.
                        using (Pen pen = new Pen(Color.FromArgb(242, 242, 242)))
                        {
                            // Create border points.
                            Point[] points = new Point[]
                            {
                                // Left point.
                                new Point(itemRct.Left, itemRct.Bottom - 1),
                                // Top points.
                                new Point(itemRct.Left, itemRct.Top),
                                new Point(itemRct.Right - 2, itemRct.Top),
                                // Right point.
                                new Point(itemRct.Right - 2, itemRct.Bottom - 1)
                            };
                            gfx.DrawLines(pen, points);
                        }
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Normal:
                        itemRct.Y += 1;
                        itemRct.Height -= 1;
                        using (Brush brush = new SolidBrush(Color.FromArgb(241, 239, 226)))
                            gfx.FillRectangle(brush, itemRct);
                        // Draw left, top and right 1px lines.
                        using (Pen pen = new Pen(Color.FromArgb(242, 242, 242)))
                        {
                            // Create border points.
                            Point[] points = new Point[]
                            {
                                // Left point.
                                new Point(itemRct.Left, itemRct.Bottom - 1),
                                // Top points.
                                new Point(itemRct.Left, itemRct.Top),
                                new Point(itemRct.Right - 2, itemRct.Top),
                                // Right point.
                                new Point(itemRct.Right - 2, itemRct.Bottom - 1)
                            };
                            gfx.DrawLines(pen, points);
                        }
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(1, 110, 193),
                            Color.FromArgb(4, 111, 191), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        itemRct.Y += 1;
                        itemRct.Height -= 1;
                        using (Brush brush = new SolidBrush(SystemColors.InactiveBorder))
                            gfx.FillRectangle(brush, itemRct);
                        // Draw left, top and right 1px lines.
                        using (Pen pen = new Pen(Color.FromArgb(242, 242, 242)))
                        {
                            // Create border points.
                            Point[] points = new Point[]
                            {
                                // Left point.
                                new Point(itemRct.Left, itemRct.Bottom - 1),
                                // Top points.
                                new Point(itemRct.Left, itemRct.Top),
                                new Point(itemRct.Right - 2, itemRct.Top),
                                // Right point.
                                new Point(itemRct.Right - 2, itemRct.Bottom - 1)
                            };
                            gfx.DrawLines(pen, points);
                        }
                        using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
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
            get { return TabPageItemStyle.OnlyText; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    [AddInRenderer("MarginGray",
    "MarginGray renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class MarginGrayRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static MarginGrayRenderer()
        {
            MY_FONT = new Font("Tahoma", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(0, 5, 5, 5),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(245, 245, 245),
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
            INTEGERARRAY = new int[] { 12, 6 };
        }

        #endregion

        public override void InvokeEditor()
        {
            throw new NotImplementedException();
        }

        public override void OnRendererBackground(Graphics gfx, Rectangle clientRct)
        {
            Rectangle rct = clientRct;
            // Draw top 2px line.
            using (Brush brush = new SolidBrush(Color.FromArgb(221, 221, 221)))
                gfx.FillRectangle(brush, rct);
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            // Do Nothing.
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle itemRct = tabPageItemRct;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Normal:
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Hover:
                    case CommonObjects.ButtonState.Pressed:
                        using (Brush brush = new SolidBrush(Color.FromArgb(221, 221, 221)))
                            gfx.FillRectangle(brush, itemRct);
                        using (Brush brush = new SolidBrush(Color.White))
                            gfx.FillRectangle(brush, index == 0 ? itemRct.Left + 5 : itemRct.Left, itemRct.Bottom - 6, index == 0 ? itemRct.Width - 5 : itemRct.Width, 6);
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
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
            get { return TabPageItemStyle.OnlyText; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    [AddInRenderer("MarginGray",
    "MarginGray renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.1.0.0")]
    public sealed class MarginGrayRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static MarginGrayRendererVS2()
        {
            MY_FONT = new Font("Tahoma", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(0, 5, 5, 5),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 18, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(245, 245, 245),
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
            INTEGERARRAY = new int[] { 12, 6 };
        }

        #endregion
        
        #region Helper Methods

        private GraphicsPath CreateRoundRect(Rectangle rect, int radius)
        {
            GraphicsPath gp = new GraphicsPath();
            int x = rect.X;
            int y = rect.Y;
            int width = rect.Width;
            int height = rect.Height;
            if (radius > 0)
            {
                radius = Math.Min(radius, height / 2 - 1);
                radius = Math.Min(radius, width / 2 - 1);
                gp.AddLine(x + radius, y, x + width - (radius * 2), y);
                gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90);
                gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2));
                gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
                gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height);
                gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
                gp.AddLine(x, y + height - (radius * 2), x, y + radius);
                gp.AddArc(x, y, radius * 2, radius * 2, 180, 90);
            }
            else
            {
                gp.AddRectangle(rect);
            }
            gp.CloseFigure();
            return gp;
        }

        #endregion

        public override void InvokeEditor()
        {
            throw new NotImplementedException();
        }

        public override void OnRendererBackground(Graphics gfx, Rectangle clientRct)
        {
            Rectangle rct = clientRct;
            // Draw top 2px line.
            using (Brush brush = new SolidBrush(Color.FromArgb(221, 221, 221)))
                gfx.FillRectangle(brush, rct);
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            // Do Nothing.
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle itemRct = tabPageItemRct;
            SmoothingMode mode = gfx.SmoothingMode;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Normal:
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Hover:
                    case CommonObjects.ButtonState.Pressed:
                        using (Brush brush = new SolidBrush(Color.FromArgb(221, 221, 221)))
                            gfx.FillRectangle(brush, itemRct);
                        gfx.SmoothingMode = SmoothingMode.AntiAlias;
                        itemRct.Inflate(-7, -7);
                        using (GraphicsPath path = CreateRoundRect(itemRct, 8))
                        {
                            using (Brush brush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.White, Color.WhiteSmoke))
                                gfx.FillPath(brush, path);
                        }
                        itemRct = tabPageItemRct;
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
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
}