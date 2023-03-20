using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.VS2010
{
    [AddInRenderer("VS2010Like",
        "VS2010Like renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class VS2010LikeRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static VS2010LikeRenderer()
        {
            MY_FONT = new Font("Arial", 8.25f);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(4, 0, 4, 0),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(41, 57, 85),
                // TabPageItemForeColor
                Color.White,
                // SelectedTabPageItemForeColor
                Color.Black,
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.White
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 4, 0 };
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
                    pen = new Pen(Color.FromArgb(206, 212, 221));
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
                    pen = new Pen(Color.FromArgb(206, 212, 221));
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
            using (GraphicsPath path = CreateRoundRect(rct, 2))
            {
                using (Brush brush = new SolidBrush(Color.FromArgb(255, 243, 205)))
                    gfx.FillPath(brush, path);
                using (Pen pen = new Pen(Color.FromArgb(255, 243, 205)))
                    gfx.DrawPath(pen, path);
            }
            gfx.SmoothingMode = mode;
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
                    case CommonObjects.ButtonState.Pressed:
                        itemRct.Height += 2;
                        using (Brush brush = new SolidBrush(BackColor))
                            gfx.FillRectangle(brush, itemRct);
                        gfx.SmoothingMode = SmoothingMode.AntiAlias;
                        using (GraphicsPath path = new GraphicsPath())
                        {
                            path.AddLine(itemRct.Left, itemRct.Bottom, itemRct.Left, itemRct.Top + 2);
                            path.AddLine(itemRct.Left, itemRct.Top + 1, itemRct.Left + 1, itemRct.Top);
                            path.AddLine(itemRct.Left + 2, itemRct.Top, itemRct.Right - 3, itemRct.Top);
                            path.AddLine(itemRct.Right - 2, itemRct.Top, itemRct.Right - 1, itemRct.Top + 1);
                            path.AddLine(itemRct.Right - 1, itemRct.Top + 2, itemRct.Right - 1, itemRct.Bottom);
                            path.CloseFigure();
                            using (Brush brush = new SolidBrush(Color.FromArgb(255, 243, 205)))
                                gfx.FillPath(brush, path);
                            using (Pen pen = new Pen(Color.FromArgb(255, 243, 205)))
                                gfx.DrawPath(pen, path);
                        }
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Hover:
                        gfx.SmoothingMode = SmoothingMode.AntiAlias;
                        using (GraphicsPath path = new GraphicsPath())
                        {
                            path.AddLine(itemRct.Left, itemRct.Bottom, itemRct.Left, itemRct.Top + 2);
                            path.AddLine(itemRct.Left, itemRct.Top + 1, itemRct.Left + 1, itemRct.Top);
                            path.AddLine(itemRct.Left + 2, itemRct.Top, itemRct.Right - 3, itemRct.Top);
                            path.AddLine(itemRct.Right - 2, itemRct.Top, itemRct.Right - 1, itemRct.Top + 1);
                            path.AddLine(itemRct.Right - 1, itemRct.Top + 2, itemRct.Right - 1, itemRct.Bottom);
                            path.CloseFigure();
                            using (Brush brush = new SolidBrush(Color.FromArgb(75, 92, 116)))
                                gfx.FillPath(brush, path);
                            using (Pen pen = new Pen(Color.FromArgb(155, 167, 183)))
                                gfx.DrawPath(pen, path);
                        }
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
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