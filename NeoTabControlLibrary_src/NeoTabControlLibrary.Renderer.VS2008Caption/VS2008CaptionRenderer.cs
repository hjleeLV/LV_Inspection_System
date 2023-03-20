using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.VS2008Caption
{
    [AddInRenderer("Visual Studio 2008 caption renderer",
        "Visual Studio 2008 caption renderer class, TabPageLayout: Top(tab page items bottom margin 4px, tab page items between spacing 5px), TabPageItemStyle: OnlyText, Hover style is not implemented.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class VS2008CaptionRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static VS2008CaptionRenderer()
        {
            MY_FONT = new Font("Arial", 9.25f);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(2, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 4, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.WhiteSmoke,
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
            INTEGERARRAY = new int[] { 4, 5 };
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
            using (Brush brush = new SolidBrush(Color.LightGray))
                gfx.FillRectangle(brush, linesRct.Left, linesRct.Top, linesRct.Width, 2);
            // Draw left, right and bottom 1px lines.
            using (Pen pen = new Pen(Color.Silver))
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
                    case CommonObjects.ButtonState.Normal:
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(204, 199, 186),
                            Color.FromArgb(204, 199, 186), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.2F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        itemRct.Width -= 1;
                        itemRct.Height -= 1;
                        // Draw a border rectangle 1px line.
                        using (Pen pen = new Pen(Color.Silver))
                            gfx.DrawRectangle(pen, itemRct);
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(61, 149, 255),
                            Color.FromArgb(0, 84, 227), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        itemRct.Width -= 1;
                        itemRct.Height -= 1;
                        // Draw a border rectangle 1px line.
                        using (Pen pen = new Pen(Color.Silver))
                            gfx.DrawRectangle(pen, itemRct);
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (Brush brush = new SolidBrush(SystemColors.InactiveBorder))
                            gfx.FillRectangle(brush, itemRct);
                        itemRct.Width -= 1;
                        itemRct.Height -= 1;
                        // Draw a border rectangle 1px line.
                        using (Pen pen = new Pen(Color.Silver))
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

    [AddInRenderer("Visual Studio 2008 caption renderer",
    "Visual Studio 2008 caption renderer class, TabPageLayout: Left(top margin 4px, tab page items between spacing 4px), TabPageItemStyle: OnlyText, Hover style is not implemented.",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.1.0.0")]
    public sealed class VS2008CaptionRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static VS2008CaptionRendererVS2()
        {
            MY_FONT = new Font("Arial", 9.25f);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 2, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(4, 0, 0, 4) };

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
            INTEGERARRAY = new int[] { 6, 4 };
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
            using (Brush brush = new SolidBrush(Color.LightGray))
                gfx.FillRectangle(brush, linesRct.Left, linesRct.Top, 2, linesRct.Height);
            // Draw left, right and bottom 1px lines.
            using (Pen pen = new Pen(Color.Silver))
            {
                // Create border points.
                Point[] points = new Point[]
                    {
                        // Top point.
                        new Point(linesRct.Left + 2, linesRct.Top),
                        // Right point.
                        new Point(linesRct.Right - 1, linesRct.Top),
                        // Bottom points.
                        new Point(linesRct.Right - 1, linesRct.Bottom - 1),
                        new Point(linesRct.Left + 2, linesRct.Bottom - 1)
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
                    case CommonObjects.ButtonState.Normal:
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(204, 199, 186),
                            Color.FromArgb(204, 199, 186), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.2F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        itemRct.Width -= 1;
                        itemRct.Height -= 1;
                        // Draw a border rectangle 1px line.
                        using (Pen pen = new Pen(Color.Silver))
                            gfx.DrawRectangle(pen, itemRct);
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(61, 149, 255),
                            Color.FromArgb(0, 84, 227), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        itemRct.Width -= 1;
                        itemRct.Height -= 1;
                        // Draw a border rectangle 1px line.
                        using (Pen pen = new Pen(Color.Silver))
                            gfx.DrawRectangle(pen, itemRct);
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (Brush brush = new SolidBrush(SystemColors.InactiveBorder))
                            gfx.FillRectangle(brush, itemRct);
                        itemRct.Width -= 1;
                        itemRct.Height -= 1;
                        // Draw a border rectangle 1px line.
                        using (Pen pen = new Pen(Color.Silver))
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
            get { return TabPageLayout.Left; }
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