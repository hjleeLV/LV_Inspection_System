using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.Telerik
{
    [AddInRenderer("Telerik",
        "Telerik renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class TelerikRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static TelerikRenderer()
        {
            MY_FONT = new Font("Arial", 8.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(2, 2, 2, 2),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 6, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(187, 216, 253),
                // TabPageItemForeColor
                Color.FromArgb(105, 127, 196),
                // SelectedTabPageItemForeColor
                Color.FromArgb(47, 87, 188),
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.FromArgb(47, 87, 188)
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 6, 0 };
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

        public override void OnDrawSmartCloseButton(Graphics gfx,
            Rectangle closeButtonRct, ButtonState btnState)
        {
            if (!IsSupportSmartCloseButton)
                return;
            Pen pen = null;
            Pen borderPen = null;
            Brush brush = null;
            switch (btnState)
            {
                case ButtonState.Normal:
                    pen = new Pen(Color.FromArgb(21, 66, 139));
                    break;
                case ButtonState.Hover:
                    pen = new Pen(Color.Black);
                    borderPen = new Pen(Color.FromArgb(229, 195, 101));
                    brush = new SolidBrush(Color.FromArgb(255, 252, 244));
                    break;
                case ButtonState.Pressed:
                    pen = new Pen(Color.White);
                    borderPen = new Pen(Color.LightGray);
                    brush = new SolidBrush(Color.FromArgb(41, 57, 85));
                    break;
                case ButtonState.Disabled:
                    pen = new Pen(SystemColors.GrayText);
                    break;
            }
            if (brush != null)
            {
                gfx.FillRectangle(brush, closeButtonRct);
                brush.Dispose();
                if (borderPen != null)
                {
                    gfx.DrawRectangle(borderPen, closeButtonRct.Left, closeButtonRct.Top,
                        closeButtonRct.Width - 1, closeButtonRct.Height - 1);
                    borderPen.Dispose();
                }
            }
            if (pen != null)
            {
                // Draw close button lines.
                gfx.DrawLine(pen, closeButtonRct.Left + 3, closeButtonRct.Top + 4,
                    closeButtonRct.Right - 5, closeButtonRct.Bottom - 3);
                gfx.DrawLine(pen, closeButtonRct.Left + 4, closeButtonRct.Top + 4,
                    closeButtonRct.Right - 4, closeButtonRct.Bottom - 3);
                gfx.DrawLine(pen, closeButtonRct.Right - 4, closeButtonRct.Top + 4,
                    closeButtonRct.Left + 4, closeButtonRct.Bottom - 3);
                gfx.DrawLine(pen, closeButtonRct.Right - 5, closeButtonRct.Top + 4,
                    closeButtonRct.Left + 3, closeButtonRct.Bottom - 3);
                pen.Dispose();
            }
        }

        public override void OnDrawSmartDropDownButton(Graphics gfx,
            Rectangle dropdownButtonRct, ButtonState btnState)
        {
            if (!IsSupportSmartDropDownButton)
                return;
            Pen pen = null;
            Pen borderPen = null;
            Brush brush = null;
            switch (btnState)
            {
                case ButtonState.Normal:
                    pen = new Pen(Color.FromArgb(21, 66, 139));
                    break;
                case ButtonState.Hover:
                    pen = new Pen(Color.Black);
                    borderPen = new Pen(Color.FromArgb(229, 195, 101));
                    brush = new SolidBrush(Color.FromArgb(255, 252, 244));
                    break;
                case ButtonState.Pressed:
                    pen = new Pen(Color.White);
                    borderPen = new Pen(Color.LightGray);
                    brush = new SolidBrush(Color.FromArgb(41, 57, 85));
                    break;
                case ButtonState.Disabled:
                    pen = new Pen(SystemColors.GrayText);
                    break;
            }
            if (brush != null)
            {
                gfx.FillRectangle(brush, dropdownButtonRct);
                brush.Dispose();
                if (borderPen != null)
                {
                    gfx.DrawRectangle(borderPen, dropdownButtonRct.Left, dropdownButtonRct.Top,
                        dropdownButtonRct.Width - 1, dropdownButtonRct.Height - 1);
                    borderPen.Dispose();
                }
            }
            if (pen != null)
            {
                using (Brush fill = new SolidBrush(pen.Color))
                {
                    gfx.FillPolygon(fill, new Point[]
                    {
                        new Point(dropdownButtonRct.Left + 3, dropdownButtonRct.Top + 6),
                        new Point(dropdownButtonRct.Right - 3, dropdownButtonRct.Top + 6),
                        new Point(dropdownButtonRct.Left + dropdownButtonRct.Width / 2, dropdownButtonRct.Bottom - 3)});
                }
                pen.Dispose();
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
            Rectangle rct = tabPageAreaRct;
            SmoothingMode mode = gfx.SmoothingMode;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            rct.Width -= 1;
            rct.Height -= 1;
            using (GraphicsPath path = CreateRoundRect(rct, 4))
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(233, 240, 249)))
                {
                    gfx.FillPath(brush, path);
                    using (Pen pen = new Pen(Color.FromArgb(110, 153, 210)))
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
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, isSelected ? Color.FromArgb(214, 228, 249) : Color.FromArgb(184, 214, 251),
                                isSelected ? Color.FromArgb(232, 239, 249) : Color.FromArgb(170, 202, 240), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.4F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillPath(brush, path);
                            }
                            using (Pen pen = new Pen(Color.FromArgb(110, 153, 210)))
                            {
                                gfx.DrawPath(pen, path);
                                if (isSelected)
                                {
                                    pen.Color = Color.FromArgb(234, 241, 250);
                                    gfx.DrawLine(pen, xOffset, itemRct.Top + 1, itemRct.Right - 4, itemRct.Top + 1);
                                    pen.Color = Color.FromArgb(234, 241, 249);
                                    gfx.DrawLine(pen, xOffset - 1, itemRct.Top + 2, itemRct.Right - 3, itemRct.Top + 2);
                                    pen.Color = Color.FromArgb(233, 240, 249);
                                    gfx.DrawLine(pen, xOffset - 2, itemRct.Top + 3, itemRct.Right - 2, itemRct.Top + 3);

                                    pen.Color = Color.FromArgb(232, 239, 249);
                                    gfx.DrawLine(pen, index == 0 ? itemRct.Left : itemRct.Left - 10, itemRct.Bottom, itemRct.Right - 2, itemRct.Bottom);
                                    gfx.DrawLine(pen, index == 0 ? itemRct.Left : itemRct.Left - 10, itemRct.Bottom + 1, itemRct.Right - 2, itemRct.Bottom + 1);
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
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(214, 228, 249),
                                Color.FromArgb(232, 239, 249), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.4F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillPath(brush, path);
                            }
                            using (Pen pen = new Pen(Color.FromArgb(110, 153, 210)))
                            {
                                gfx.DrawPath(pen, path);
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