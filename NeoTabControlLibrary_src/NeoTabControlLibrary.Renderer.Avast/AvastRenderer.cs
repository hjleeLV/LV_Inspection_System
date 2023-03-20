using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.Avast
{
    [AddInRenderer("Avast",
    "Avast renderer class, TabPageLayout: Left(top margin 8px), TabPageItemStyle: TextAnd16x16_Image.",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class AvastRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static AvastRenderer()
        {
            MY_FONT = new Font("Malgun Gothic", 9.25f);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(8, 8, 8, 8),
                // ITEM_AREA_OFFSET
                new DrawingOffset(8, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(55, 62, 72),
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
            INTEGERARRAY = new int[] { 18, 0 };
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
            // Do nothing.
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            Rectangle rct = tabPageAreaRct;
            rct.Width -= 1;
            rct.Height -= 1;
            using (GraphicsPath path = CreateRoundRect(rct, 8))
            {
                using (Brush brush = new SolidBrush(Color.White))
                    gfx.FillPath(brush, path);
                using (Pen pen = new Pen(Color.FromArgb(253, 152, 0)))
                    gfx.DrawPath(pen, path);
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            Bitmap itemBitmap = null;
            tabPageText = tabPageText.ToUpperInvariant();
            switch (tabPageText)
            {
                case "Camera":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.camera_0;
                    else
                        itemBitmap = Resources.camera_1;
                    break;
                case "Comm.":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.plc_0;
                    else
                        itemBitmap = Resources.plc_1;
                    break;
                case "통신설정":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.plc_0;
                    else
                        itemBitmap = Resources.plc_1;
                    break;
                case "LOG":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.log_0;
                    else
                        itemBitmap = Resources.log_1;
                    break;
                case "LOG DATA":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.data_1;
                    else
                        itemBitmap = Resources.data_0;
                    break;
                case "S/W HISTORY":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.History_1;
                    else
                        itemBitmap = Resources.History_0;
                    break;

                case "SOLUTION &EXPLORER":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_292;
                    else
                        itemBitmap = Resources.Data_291;
                    break;
                case "PRO&PERTIES":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_295;
                    else
                        itemBitmap = Resources.Data_294;
                    break;
                case "&TOOLBOX":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_311;
                    else
                        itemBitmap = Resources.Data_310;
                    break;
                case "ERROR &LIST":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_313;
                    else
                        itemBitmap = Resources.Data_312;
                    break;
                case "&OUTPUT":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_315;
                    else
                        itemBitmap = Resources.Data_314;
                    break;
                default:
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_292;
                    else
                        itemBitmap = Resources.Data_291;
                    break;
            }
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                int offset;
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            Bitmap bmp = Resources.TPageItemNormalKoyu;
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            bmp = Resources.HoverEffect;
                            rct = new Rectangle(rct.Left, rct.Bottom - 1 - bmp.Height,
                                rct.Width, bmp.Height);
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        rct = tabPageItemRct;
                        rct = new Rectangle(10, rct.Top + (rct.Height / 2) - itemBitmap.Height / 2,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = tabPageItemRct;
                        offset = 10 + itemBitmap.Width + 8;
                        rct.X += offset;
                        rct.Width -= offset;
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Normal:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            Bitmap bmp = Resources.TPageItemNormalKoyu;
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        rct = new Rectangle(10, rct.Top + (rct.Height / 2) - itemBitmap.Height / 2,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = tabPageItemRct;
                        offset = 10 + itemBitmap.Width + 8;
                        rct.X += offset;
                        rct.Width -= offset;
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            Bitmap bmp = Resources.TPageItemSelectedBack;
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        rct = new Rectangle(10, rct.Top + (rct.Height / 2) - itemBitmap.Height / 2,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = tabPageItemRct;
                        offset = 10 + itemBitmap.Width + 8;
                        rct.X += offset;
                        rct.Width -= offset;
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            Bitmap bmp = Resources.TPageItemNormalKoyu;
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        rct = new Rectangle(10, rct.Top + (rct.Height / 2) - itemBitmap.Height / 2,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = tabPageItemRct;
                        offset = 10 + itemBitmap.Width + 8;
                        rct.X += offset;
                        rct.Width -= offset;
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

    [AddInRenderer("Avast",
    "Avast renderer class, TabPageLayout: Right(top margin 8px), TabPageItemStyle: TextAnd16x16_Image.",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.1.0.0")]
    public sealed class AvastRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static AvastRendererVS2()
        {
            MY_FONT = new Font("Malgun Gothic", 9.25f,FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(8, 8, 8, 8),
                // ITEM_AREA_OFFSET
                new DrawingOffset(8, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(55, 62, 72),
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
            INTEGERARRAY = new int[] { 18, 0 };
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
            // Do nothing.
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            Rectangle rct = tabPageAreaRct;
            rct.Width -= 1;
            rct.Height -= 1;
            using (GraphicsPath path = CreateRoundRect(rct, 8))
            {
                using (Brush brush = new SolidBrush(Color.White))
                    gfx.FillPath(brush, path);
                using (Pen pen = new Pen(Color.FromArgb(253, 152, 0)))
                    gfx.DrawPath(pen, path);
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            Bitmap itemBitmap = null;
            tabPageText = tabPageText.ToUpperInvariant();
            switch (tabPageText)
            {
                case "Camera":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.camera_0;
                    else
                        itemBitmap = Resources.camera_1;
                    break;
                case "Comm.":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.plc_0;
                    else
                        itemBitmap = Resources.plc_0;
                    break;
                case "통신설정":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.plc_0;
                    else
                        itemBitmap = Resources.plc_0;
                    break;
                case "LOG":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.log_0;
                    else
                        itemBitmap = Resources.log_0;
                    break;
                case "MODEL":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.model_0;
                    else
                        itemBitmap = Resources.model_0;
                    break;
                case "마크등록":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.mark_0;
                    else
                        itemBitmap = Resources.mark_1;
                    break;
                case "LOG DATA":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.data_1;
                    else
                        itemBitmap = Resources.data_0;
                    break;
                case "S/W HISTORY":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.History_1;
                    else
                        itemBitmap = Resources.History_0;
                    break;

                case "SOLUTION &EXPLORER":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_292;
                    else
                        itemBitmap = Resources.Data_291;
                    break;
                case "PRO&PERTIES":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_295;
                    else
                        itemBitmap = Resources.Data_294;
                    break;
                case "&TOOLBOX":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_311;
                    else
                        itemBitmap = Resources.Data_310;
                    break;
                case "ERROR &LIST":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_313;
                    else
                        itemBitmap = Resources.Data_312;
                    break;
                case "&OUTPUT":
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_315;
                    else
                        itemBitmap = Resources.Data_314;
                    break;
                default:
                    if (btnState == ButtonState.Pressed)
                        itemBitmap = Resources.Data_292;
                    else
                        itemBitmap = Resources.Data_291;
                    break;
            }
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                int offset;
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            Bitmap bmp = Resources.TPageItemNormalKoyu;
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                            bmp = Resources.HoverEffect;
                            rct = new Rectangle(rct.Left, rct.Bottom - 1 - bmp.Height,
                                rct.Width, bmp.Height);
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        rct = tabPageItemRct;
                        rct = new Rectangle(rct.Left + 10, rct.Top + (rct.Height / 2) - itemBitmap.Height / 2,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = tabPageItemRct;
                        offset = 10 + itemBitmap.Width + 8;
                        rct.X += offset;
                        rct.Width -= offset;
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Normal:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            Bitmap bmp = Resources.TPageItemNormalKoyu;
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        rct = new Rectangle(rct.Left + 10, rct.Top + (rct.Height / 2) - itemBitmap.Height / 2,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = tabPageItemRct;
                        offset = 10 + itemBitmap.Width + 8;
                        rct.X += offset;
                        rct.Width -= offset;
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            Bitmap bmp = Resources.TPageItemSelectedBackRight;
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        rct = new Rectangle(rct.Left + 10, rct.Top + (rct.Height / 2) - itemBitmap.Height / 2,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = tabPageItemRct;
                        offset = 10 + itemBitmap.Width + 8;
                        rct.X += offset;
                        rct.Width -= offset;
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            Bitmap bmp = Resources.TPageItemNormalKoyu;
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        rct = new Rectangle(rct.Left + 10, rct.Top + (rct.Height / 2) - itemBitmap.Height / 2,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = tabPageItemRct;
                        offset = 10 + itemBitmap.Width + 8;
                        rct.X += offset;
                        rct.Width -= offset;
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
            get { return TabPageLayout.Right; }
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

    [AddInRenderer("Avast",
    "Avast renderer class, TabPageLayout: Left(top margin 8px), TabPageItemsBetweenSpacing: 2px, TabPageItemStyle: OnlyText.",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.2.0.0")]
    public sealed class AvastRendererVS3 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static AvastRendererVS3()
        {
            MY_FONT = new Font("Arial", 9.25f);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(8, 8, 8, 8),
                // ITEM_AREA_OFFSET
                new DrawingOffset(8, 0, 0, 0) };

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
            INTEGERARRAY = new int[] { 6, 2 };
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
            // Do nothing.
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            Rectangle rct = tabPageAreaRct;
            rct.Width -= 1;
            rct.Height -= 1;
            using (GraphicsPath path = CreateRoundRect(rct, 8))
            {
                using (Brush brush = new SolidBrush(Color.White))
                    gfx.FillPath(brush, path);
                using (Pen pen = new Pen(Color.FromArgb(253, 152, 0)))
                    gfx.DrawPath(pen, path);
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Far;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Normal:
                        rct.Width -= 8;
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Hover:
                    case CommonObjects.ButtonState.Pressed:
                        rct.Width += 1;
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            Bitmap bmp = Resources.SmallTPItem;
                            gfx.DrawImage(bmp, rct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        rct.Width -= 9;
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        rct.Width -= 8;
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
            get { return TabPageItemStyle.OnlyText; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}