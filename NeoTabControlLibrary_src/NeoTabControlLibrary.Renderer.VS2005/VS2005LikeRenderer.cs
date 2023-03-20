using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.VS2005
{
    [AddInRenderer("VS2005Like",
        "VS2005Like renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class VS2005LikeRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static VS2005LikeRenderer()
        {
            MY_FONT = new Font("Tahoma", 8.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 1, 0, 0) };

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
                Color.FromArgb(47, 87, 188)
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 4, 0 };
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

        public override DrawingOffset SmartButtonsAreaOffset
        {
            get { return new DrawingOffset(0, 0, 4, 6); }
        }
        
        public override void InvokeEditor()
        {
            throw new NotImplementedException();
        }

        public override void OnRendererBackground(Graphics gfx, Rectangle clientRct)
        {
            Rectangle linesRct = clientRct;
            linesRct.Width -= 1;
            linesRct.Height -= 1;
            // Draw 1px border rectangle.
            using (Pen pen = new Pen(Color.FromArgb(172, 168, 153)))
                gfx.DrawRectangle(pen, linesRct);
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            Rectangle linesRct = tabPageAreaRct;
            linesRct.Width -= 1;
            // Draw 1px border rectangle.
            using (Pen pen = new Pen(Color.FromArgb(172, 168, 153)))
                gfx.DrawLine(pen, linesRct.Left + 1, linesRct.Top, linesRct.Right - 1, linesRct.Top);
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle itemRct = tabPageItemRct;
            itemRct.Y += 3;
            itemRct.Height -= 3;
            SmoothingMode mode = gfx.SmoothingMode;
            gfx.SmoothingMode = SmoothingMode.HighQuality;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.Trimming = StringTrimming.EllipsisCharacter;
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
                            textColor = SelectedTabPageItemForeColor;
                            using (Brush brush = new SolidBrush(BackColor))
                                gfx.FillRectangle(brush, tabPageItemRct.Left, tabPageItemRct.Top + 1, tabPageItemRct.Width, tabPageItemRct.Height);
                            goto case CommonObjects.ButtonState.Disabled;
                        case CommonObjects.ButtonState.Disabled:
                            if (index == 0)
                            {
                                xOffset = itemRct.Left + 10 + (itemRct.Height / 2);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left, itemRct.Bottom),
                                    new Point(itemRct.Left, itemRct.Bottom - 3),
                                    new Point(itemRct.Left + 8, itemRct.Bottom - 15),
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
                                    new Point(itemRct.Left - 2, itemRct.Bottom - 15),
                                    new Point(xOffset, itemRct.Top),
                                };
                                path.AddBeziers(points);
                            }
                            else
                            {
                                xOffset = itemRct.Left + (itemRct.Height / 2);
                                path.AddLine(itemRct.Left, itemRct.Bottom, itemRct.Left,
                                    itemRct.Top + (itemRct.Height / 2) + 6);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left, itemRct.Top + 10),
                                    new Point(itemRct.Left + 1, itemRct.Top + 9),
                                    new Point(itemRct.Left + 2, itemRct.Top + 1),
                                    new Point(xOffset, itemRct.Top),
                                };
                                path.AddBeziers(points);
                            }
                            path.AddLine(xOffset + 1, itemRct.Top, itemRct.Right - 4, itemRct.Top);
                            path.AddLine(itemRct.Right - 1, itemRct.Top + 2, itemRct.Right - 1, itemRct.Bottom);
                            path.CloseFigure();
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, isSelected ? Color.White : Color.FromArgb(254, 254, 253),
                                isSelected ? Color.White : Color.FromArgb(238, 236, 221), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.4F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillPath(brush, path);
                            }
                            using (Pen pen = new Pen(Color.FromArgb(172, 168, 153)))
                            {
                                gfx.DrawPath(pen, path);
                                if (isSelected)
                                {
                                    pen.Color = Color.WhiteSmoke;
                                    gfx.DrawLine(pen, xOffset, itemRct.Top + 1, itemRct.Right - 4, itemRct.Top + 1);
                                    pen.Color = Color.FromArgb(248, 248, 248);
                                    gfx.DrawLine(pen, xOffset - 1, itemRct.Top + 2, itemRct.Right - 3, itemRct.Top + 2);
                                    pen.Color = Color.FromArgb(251, 251, 251);
                                    gfx.DrawLine(pen, xOffset - 2, itemRct.Top + 3, itemRct.Right - 2, itemRct.Top + 3);

                                    pen.Color = Color.White;
                                    gfx.DrawLine(pen, index == 0 ? itemRct.Left + 1 : itemRct.Left - 9, itemRct.Bottom, itemRct.Right - 2, itemRct.Bottom);
                                }
                            }
                            using (Font font = new Font(NeoTabPageItemsFont, 
                                isSelected ? FontStyle.Bold : FontStyle.Regular))
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
                                    new Point(itemRct.Left + 8, itemRct.Bottom - 15),
                                    new Point(xOffset, itemRct.Top),
                                };
                                path.AddBeziers(points);
                            }
                            else
                            {
                                xOffset = itemRct.Left + (itemRct.Height / 2);
                                path.AddLine(itemRct.Left, itemRct.Bottom, itemRct.Left,
                                    itemRct.Top + (itemRct.Height / 2) + 6);
                                Point[] points = new Point[]
                                {
                                    new Point(itemRct.Left, itemRct.Top + 10),
                                    new Point(itemRct.Left + 1, itemRct.Top + 9),
                                    new Point(itemRct.Left + 2, itemRct.Top + 1),
                                    new Point(xOffset, itemRct.Top),
                                };
                                path.AddBeziers(points);
                            }
                            path.AddLine(xOffset + 1, itemRct.Top, itemRct.Right - 4, itemRct.Top);
                            path.AddLine(itemRct.Right - 1, itemRct.Top + 2, itemRct.Right - 1, itemRct.Bottom);
                            path.CloseFigure();
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(244, 248, 249),
                                Color.FromArgb(242, 239, 249), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.4F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillPath(brush, path);
                            }
                            using (Pen pen = new Pen(Color.FromArgb(172, 168, 153)))
                                gfx.DrawPath(pen, path);
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