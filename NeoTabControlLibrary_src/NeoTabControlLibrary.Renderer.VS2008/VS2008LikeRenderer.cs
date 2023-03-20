using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.VS2008
{
    [AddInRenderer("VS2008Like",
        "VS2008Like renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class VS2008LikeRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static VS2008LikeRenderer()
        {
            MY_FONT = new Font("Tahoma", 8.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(5, 5, 5, 5),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 5, 0, 0) };

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
                Color.Black
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 3, 0 };
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
            Rectangle rct = tabPageAreaRct;
            SmoothingMode mode = gfx.SmoothingMode;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            rct.Inflate(-2, -2);
            rct.Width -= 1;
            rct.Height -= 1;
            using (Brush brush = new SolidBrush(Color.FromArgb(194, 207, 229)))
            {
                using (Pen pen = new Pen(brush, 4))
                    gfx.DrawRectangle(pen, rct);
            }
            rct = tabPageAreaRct;
            rct.Inflate(-4, -4);
            rct.Width -= 1;
            rct.Height -= 1;
            using (Pen pen = new Pen(Color.FromArgb(161, 180, 214)))
                gfx.DrawRectangle(pen, rct);
            rct = tabPageAreaRct;
            rct.Width -= 1;
            rct.Height -= 1;
            using (GraphicsPath path = CreateRoundRect(rct, 4))
            {
                using (LinearGradientBrush brush =
                    new LinearGradientBrush(Point.Empty, new Point(0, 1),
                        Color.FromArgb(157, 177, 212),
                        Color.FromArgb(153, 175, 212)))
                {
                    Blend bl = new Blend(2);
                    bl.Factors = new float[] { 0.3F, 1.0F };
                    bl.Positions = new float[] { 0.0F, 1.0F };
                    brush.Blend = bl;
                    using (Pen pen = new Pen(brush))
                        gfx.DrawPath(pen, path);
                }
            }
            rct.Inflate(-1, -1);
            using (GraphicsPath path = CreateRoundRect(rct, 4))
            {
                using (Pen pen = new Pen(Color.FromArgb(225, 230, 232)))
                    gfx.DrawPath(pen, path);
            }
            gfx.SmoothingMode = mode;
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle itemRct = tabPageItemRct;
            itemRct.Y += 2;
            itemRct.Height -= 2;
            SmoothingMode mode = gfx.SmoothingMode;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                using (GraphicsPath path = new GraphicsPath())
                {
                    int xOffset;
                    bool isSelected = false;
                    Color textColor = DisabledTabPageItemForeColor;
                    switch (btnState)
                    {
                        case CommonObjects.ButtonState.Normal:
                            textColor = TabPageItemForeColor;
                            goto case CommonObjects.ButtonState.Disabled;
                        case CommonObjects.ButtonState.Pressed:
                            isSelected = true;
                            itemRct.Y -= 2;
                            itemRct.Height += 2;
                            textColor = SelectedTabPageItemForeColor;
                            using (Brush brush = new SolidBrush(BackColor))
                                gfx.FillRectangle(brush, tabPageItemRct.Left - 1, tabPageItemRct.Top - 1, tabPageItemRct.Width + 1, tabPageItemRct.Height + 1);
                            goto case CommonObjects.ButtonState.Disabled;
                        case CommonObjects.ButtonState.Disabled:
                            if (index == 0)
                            {
                                xOffset = itemRct.Left + 10 + (itemRct.Height / 2);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left, itemRct.Bottom),
                                    new Point(itemRct.Left, itemRct.Bottom - 3),
                                    new Point(itemRct.Left + 8, itemRct.Bottom - 17),
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
                                    new Point(itemRct.Left - 10, itemRct.Bottom - 3),
                                    new Point(itemRct.Left - 2, itemRct.Bottom - 17),
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
                            path.AddLine(xOffset + 1, itemRct.Top, itemRct.Right - 4, itemRct.Top);
                            path.AddLine(itemRct.Right - 1, itemRct.Top + 2, itemRct.Right - 1, itemRct.Bottom);
                            path.CloseFigure();
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.White,
                                isSelected ? Color.FromArgb(194, 207, 229) : Color.FromArgb(238, 236, 221), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.4F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillPath(brush, path);
                            }
                            using (Pen pen = new Pen(isSelected ? Color.FromArgb(153, 175, 212) : Color.FromArgb(172, 168, 153)))
                            {
                                gfx.DrawPath(pen, path);
                                if (isSelected)
                                {
                                    pen.Color = Color.FromArgb(194, 207, 229);
                                    gfx.DrawLine(pen, index == 0 ? itemRct.Left + 1 : itemRct.Left - 9, itemRct.Bottom, itemRct.Right - 2, itemRct.Bottom);
                                    gfx.DrawLine(pen, index == 0 ? itemRct.Left + 1 : itemRct.Left - 9, itemRct.Bottom + 1, itemRct.Right - 2, itemRct.Bottom + 1);
                                }
                                else
                                {
                                    pen.Color = Color.FromArgb(156, 176, 212);
                                    gfx.DrawLine(pen, itemRct.Left, itemRct.Bottom, itemRct.Right - 1, itemRct.Bottom);
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
                        case CommonObjects.ButtonState.Hover:
                            if (index == 0)
                            {
                                xOffset = itemRct.Left + 10 + (itemRct.Height / 2);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left, itemRct.Bottom),
                                    new Point(itemRct.Left, itemRct.Bottom - 3),
                                    new Point(itemRct.Left + 8, itemRct.Bottom - 17),
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
                            path.AddLine(xOffset + 1, itemRct.Top, itemRct.Right - 4, itemRct.Top);
                            path.AddLine(itemRct.Right - 1, itemRct.Top + 2, itemRct.Right - 1, itemRct.Bottom);
                            path.CloseFigure();
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(220,226,231),
                                Color.FromArgb(162, 187, 226), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.3F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillPath(brush, path);
                            }
                            using (Pen pen = new Pen(Color.FromArgb(153, 175, 212)))
                            {
                                gfx.DrawPath(pen, path);
                                pen.Color = Color.FromArgb(194, 207, 229);
                                gfx.DrawLine(pen, itemRct.Left, itemRct.Bottom, itemRct.Right - 1, itemRct.Bottom);
                            }
                            using (Font font = new Font(NeoTabPageItemsFont, FontStyle.Regular))
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
                                using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
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
}