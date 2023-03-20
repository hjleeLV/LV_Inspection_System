using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.SizDotNET
{
    [AddInRenderer("SizDotNET",
        "SizDotNET renderer class, TabPageLayout: Top(tab page items bottom margin 3px, tab page items between spacing 3px), TabPageItemStyle: OnlyText.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class SizDotNETRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static SizDotNETRenderer()
        {
            MY_FONT = new Font("Tahoma", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 3, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.White,
                // TabPageItemForeColor
                Color.FromArgb(97, 53, 64),
                // SelectedTabPageItemForeColor
                Color.Black,
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.White
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 6, 3 };
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
            using (Pen pen = new Pen(Color.FromArgb(202, 202, 214)))
                gfx.DrawRectangle(pen, linesRct);
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
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(255, 192, 16),
                            Color.FromArgb(248, 157, 28), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Normal:
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(255, 192, 16),
                            Color.FromArgb(248, 157, 28), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        itemRct.Height += 4;
                        using (Brush brush = new SolidBrush(Color.White))
                            gfx.FillRectangle(brush, itemRct);
                        // Draw left, top and right 1px lines.
                        using (Pen pen = new Pen(Color.FromArgb(207, 207, 207)))
                        {
                            // Create border points.
                            Point[] points = new Point[]
                            {
                                // Left point.
                                new Point(itemRct.Left, itemRct.Bottom - 1),
                                // Top points.
                                new Point(itemRct.Left, itemRct.Top),
                                new Point(itemRct.Right - 1, itemRct.Top),
                                // Right point.
                                new Point(itemRct.Right - 1, itemRct.Bottom - 1)
                            };
                            gfx.DrawLines(pen, points);
                        }
                        itemRct.Height -= 4;
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (Brush brush = new SolidBrush(SystemColors.InactiveBorder))
                            gfx.FillRectangle(brush, itemRct);
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

    [AddInRenderer("SizDotNET",
        "SizDotNET renderer class with right alignment, TabPageLayout: Right(tab page items left margin 3px, tab page items between spacing 3px), TabPageItemStyle: OnlyText.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.1.0.0")]
    public sealed class SizDotNETRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static SizDotNETRendererVS2()
        {
            MY_FONT = new Font("Tahoma", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 3, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.White,
                // TabPageItemForeColor
                Color.FromArgb(97, 53, 64),
                // SelectedTabPageItemForeColor
                Color.Black,
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.White
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 6, 3 };
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
            using (Pen pen = new Pen(Color.FromArgb(202, 202, 214)))
                gfx.DrawRectangle(pen, linesRct);
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
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(255, 192, 16),
                            Color.FromArgb(248, 157, 28), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Normal:
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(255, 192, 16),
                            Color.FromArgb(248, 157, 28), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        itemRct.X -= 4;
                        itemRct.Width += 4;
                        using (Brush brush = new SolidBrush(Color.White))
                            gfx.FillRectangle(brush, itemRct);
                        // Draw left, top and right 1px lines.
                        using (Pen pen = new Pen(Color.FromArgb(207, 207, 207)))
                        {
                            // Create border points.
                            Point[] points = new Point[]
                            {
                                // Top point.
                                new Point(itemRct.Left, itemRct.Top),
                                // Right points.
                                new Point(itemRct.Right - 1, itemRct.Top),
                                new Point(itemRct.Right - 1, itemRct.Bottom - 1),
                                // Bottom point.
                                new Point(itemRct.Left, itemRct.Bottom - 1)
                            };
                            gfx.DrawLines(pen, points);
                        }
                        itemRct.X += 4;
                        itemRct.Width -= 4;
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (Brush brush = new SolidBrush(SystemColors.InactiveBorder))
                            gfx.FillRectangle(brush, itemRct);
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
            get { return TabPageLayout.Right; }
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