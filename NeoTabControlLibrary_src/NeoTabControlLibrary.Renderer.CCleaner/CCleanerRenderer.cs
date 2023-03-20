using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.CCleaner
{
    [AddInRenderer("CCleaner",
    "CCleaner renderer class, TabPageLayout: Left(top margin 6px, tab page items between spacing 2px), TabPageItemStyle: TextAnd48x48_Image.",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class CCleanerRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static CCleanerRenderer()
        {
            MY_FONT = new Font("Malgun Gothic", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(6, 6, 6, 6),
                // ITEM_AREA_OFFSET
                new DrawingOffset(6, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(103, 103, 103),
                // TabPageItemForeColor
                Color.White,
                // SelectedTabPageItemForeColor
                Color.White,
                // DisabledTabPageItemForeColor
                Color.FromArgb(172, 168, 153),
                // MouseOverTabPageItemForeColor
                Color.White
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 4, 2 };
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
            Rectangle destRect, rct = tabPageAreaRct;
            using (ImageAttributes attributes = new ImageAttributes())
            {
                /* Draw panel corners. */
                Bitmap cornerBmp = null;
                for (int i = 0; i <= 7; i++)
                {
                    switch (i)
                    {
                        case 0:
                            cornerBmp = Resources.panel_left_top;
                            destRect = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 1:
                            cornerBmp = Resources.panel_middle_top;
                            destRect = new Rectangle(rct.Left + 6, rct.Top, rct.Width - 12, cornerBmp.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipX);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 2:
                            cornerBmp = Resources.panel_right_top;
                            destRect = new Rectangle(rct.Right - 6, rct.Top, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 3:
                            cornerBmp = Resources.panel_middle_left;
                            destRect = new Rectangle(rct.Left, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 4:
                            cornerBmp = Resources.panel_middle_right;
                            destRect = new Rectangle(rct.Right - 6, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 5:
                            cornerBmp = Resources.panel_left_bottom;
                            destRect = new Rectangle(rct.Left, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 6:
                            cornerBmp = Resources.panel_middle_bottom;
                            destRect = new Rectangle(rct.Left + 6, rct.Bottom - 6, rct.Width - 12, cornerBmp.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipX);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 7:
                            cornerBmp = Resources.panel_right_bottom;
                            destRect = new Rectangle(rct.Right - 6, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                    }
                    attributes.SetWrapMode(WrapMode.Clamp);
                }
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            Bitmap itemBitmap = null;
            switch (tabPageText.ToUpperInvariant())
            {
                case "MAIN":
                    itemBitmap = Resources.Display;
                    break;
                case "PARAMETERS":
                    itemBitmap = Resources.Pram;
                    break;
                case "SYSTEM":
                    itemBitmap = Resources.System;
                    break;
                case "LOG":
                    itemBitmap = Resources.data;
                    break;
                case "TEST":
                    itemBitmap = Resources.Manual;
                    break;
                case "MODEL":
                    itemBitmap = Resources.Model;
                    break;
                case "LOGIN":
                    itemBitmap = Resources.Admin;
                    break;
                case "LOGOUT":
                    itemBitmap = Resources.Logoff;
                    break;
                case "HELP":
                    itemBitmap = Resources.Help;
                    break;
                case "SPEC":
                    itemBitmap = Resources.Spec;
                    break;
                default:
                    itemBitmap = Resources.data;
                    break;
            }

            switch (tabPageText)
            {
                case "메인화면":
                    itemBitmap = Resources.Display;
                    break;
                case "검사설정":
                    itemBitmap = Resources.Pram;
                    break;
                case "장비설정":
                    itemBitmap = Resources.System;
                    break;
                case "로그":
                    itemBitmap = Resources.data;
                    break;
                case "수동검사":
                    itemBitmap = Resources.Manual;
                    break;
                case "모델":
                    itemBitmap = Resources.Model;
                    break;
                case "로그인":
                    itemBitmap = Resources.Admin;
                    break;
                case "로그아웃":
                    itemBitmap = Resources.Logoff;
                    break;
                case "도움말":
                    itemBitmap = Resources.Help;
                    break;
                case "스펙설정":
                    itemBitmap = Resources.Spec;
                    break;

                case "主要":
                    itemBitmap = Resources.Display;
                    break;
                case "参数":
                    itemBitmap = Resources.Pram;
                    break;
                case "系统":
                    itemBitmap = Resources.System;
                    break;
                case "日志":
                    itemBitmap = Resources.data;
                    break;
                case "模型":
                    itemBitmap = Resources.Model;
                    break;
                case "测试":
                    itemBitmap = Resources.Manual;
                    break;

                case "登录":
                    itemBitmap = Resources.Admin;
                    break;
                case "注销":
                    itemBitmap = Resources.Logoff;
                    break;
                case "帮助":
                    itemBitmap = Resources.Help;
                    break;
                case "规范":
                    itemBitmap = Resources.Spec;
                    break;
            }
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            /* Draw corners. */
                            Bitmap cornerBmp = null;
                            for (int i = 0; i <= 4; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        cornerBmp = Resources.normal_left_top;
                                        rct = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 1:
                                        cornerBmp = Resources.normal_middle_top;
                                        rct = new Rectangle(rct.Left + 6, rct.Top, rct.Width - 6, cornerBmp.Height);
                                        attributes.SetWrapMode(WrapMode.TileFlipX);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 2:
                                        cornerBmp = Resources.normal_middle_left;
                                        rct = new Rectangle(rct.Left, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                                        attributes.SetWrapMode(WrapMode.TileFlipY);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 3:
                                        cornerBmp = Resources.normal_left_bottom;
                                        rct = new Rectangle(rct.Left, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 4:
                                        cornerBmp = Resources.normal_middle_bottom;
                                        rct = new Rectangle(rct.Left + 6, rct.Bottom - 6, rct.Width - 6, cornerBmp.Height);
                                        attributes.SetWrapMode(WrapMode.TileFlipX);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                }
                                rct = tabPageItemRct;
                                attributes.SetWrapMode(WrapMode.Clamp);
                            }
                        }
                        rct = new Rectangle(rct.Width / 2 - itemBitmap.Width / 2, rct.Top + ItemObjectsDrawingMargin,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = new Rectangle(tabPageItemRct.Left, rct.Bottom, tabPageItemRct.Width, tabPageItemRct.Height - ItemObjectsDrawingMargin - 48);
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Normal:
                        rct = new Rectangle(rct.Width / 2 - itemBitmap.Width / 2, rct.Top + ItemObjectsDrawingMargin,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = new Rectangle(tabPageItemRct.Left, rct.Bottom, tabPageItemRct.Width, tabPageItemRct.Height - ItemObjectsDrawingMargin - 48);
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            /* Draw corners. */
                            Bitmap cornerBmp = null;
                            for (int i = 0; i <= 4; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        cornerBmp = Resources.active_left_top_dark;
                                        rct = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 1:
                                        cornerBmp = Resources.active_middle_top_dark;
                                        rct = new Rectangle(rct.Left + 6, rct.Top, rct.Width - 6, cornerBmp.Height);
                                        attributes.SetWrapMode(WrapMode.TileFlipX);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 2:
                                        cornerBmp = Resources.active_middle_left_dark;
                                        rct = new Rectangle(rct.Left, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                                        attributes.SetWrapMode(WrapMode.TileFlipY);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 3:
                                        cornerBmp = Resources.active_left_bottom_dark;
                                        rct = new Rectangle(rct.Left, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 4:
                                        cornerBmp = Resources.active_middle_bottom_dark;
                                        rct = new Rectangle(rct.Left + 6, rct.Bottom - 6, rct.Width - 6, cornerBmp.Height);
                                        attributes.SetWrapMode(WrapMode.TileFlipX);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                }
                                rct = tabPageItemRct;
                                attributes.SetWrapMode(WrapMode.Clamp);
                            }
                        }
                        rct.Inflate(0, -6);
                        rct.X += 6;
                        rct.Width -= 6;
                        using (Brush brush = new SolidBrush(Color.FromArgb(81, 81, 81)))
                            gfx.FillRectangle(brush, rct);
                        rct = tabPageItemRct;
                        rct = new Rectangle((rct.Width / 2 - itemBitmap.Width / 2) + 1, rct.Top + ItemObjectsDrawingMargin + 1,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = new Rectangle(tabPageItemRct.Left, rct.Bottom, tabPageItemRct.Width, tabPageItemRct.Height - ItemObjectsDrawingMargin - 48);
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        rct = new Rectangle(rct.Width / 2 - itemBitmap.Width / 2, rct.Top + ItemObjectsDrawingMargin,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = new Rectangle(tabPageItemRct.Left, rct.Bottom, tabPageItemRct.Width, tabPageItemRct.Height - ItemObjectsDrawingMargin - 48);
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
            get { return TabPageItemStyle.TextAnd48x48_Image; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    [AddInRenderer("CCleaner",
    "CCleaner renderer class, TabPageLayout: Left(top margin 6px, tab page items between spacing 6px), TabPageItemStyle: Only48x48_Image.",
    DeveloperName = "Burak Özdiken", VersionNumber = "2.0.0.0")]
    public sealed class CCleanerRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static CCleanerRendererVS2()
        {
            MY_FONT = new Font("Malgun Gothic", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(6, 6, 6, 6),
                // ITEM_AREA_OFFSET
                new DrawingOffset(6, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(103, 103, 103),
                // TabPageItemForeColor
                Color.White,
                // SelectedTabPageItemForeColor
                Color.White,
                // DisabledTabPageItemForeColor
                Color.FromArgb(172, 168, 153),
                // MouseOverTabPageItemForeColor
                Color.White
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 6, 6 };
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
            Rectangle destRect, rct = tabPageAreaRct;
            using (ImageAttributes attributes = new ImageAttributes())
            {
                /* Draw panel corners. */
                Bitmap cornerBmp = null;
                for (int i = 0; i <= 7; i++)
                {
                    switch (i)
                    {
                        case 0:
                            cornerBmp = Resources.panel_left_top;
                            destRect = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 1:
                            cornerBmp = Resources.panel_middle_top;
                            destRect = new Rectangle(rct.Left + 6, rct.Top, rct.Width - 12, cornerBmp.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipX);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 2:
                            cornerBmp = Resources.panel_right_top;
                            destRect = new Rectangle(rct.Right - 6, rct.Top, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 3:
                            cornerBmp = Resources.panel_middle_left;
                            destRect = new Rectangle(rct.Left, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 4:
                            cornerBmp = Resources.panel_middle_right;
                            destRect = new Rectangle(rct.Right - 6, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 5:
                            cornerBmp = Resources.panel_left_bottom;
                            destRect = new Rectangle(rct.Left, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 6:
                            cornerBmp = Resources.panel_middle_bottom;
                            destRect = new Rectangle(rct.Left + 6, rct.Bottom - 6, rct.Width - 12, cornerBmp.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipX);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 7:
                            cornerBmp = Resources.panel_right_bottom;
                            destRect = new Rectangle(rct.Right - 6, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                    }
                    attributes.SetWrapMode(WrapMode.Clamp);
                }
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            Bitmap itemBitmap = null;
            switch (tabPageText.ToUpperInvariant())
            {
                case "Judgement Setting":
                    itemBitmap = Resources._01;
                    break;
                case "Algorithm Setting":
                    itemBitmap = Resources._02;
                    break;
                case "Camera Setting":
                    itemBitmap = Resources._01;
                    break;
                case "PLC Setting":
                    itemBitmap = Resources._02;
                    break;
                case "LOG Setting":
                    itemBitmap = Resources._03;
                    break;
                case "Data Log":
                    itemBitmap = Resources._01;
                    break;
                case "System Log":
                    itemBitmap = Resources._02;
                    break;
                default:
                    itemBitmap = Resources._01;
                    break;
            }
            switch (btnState)
            {
                case CommonObjects.ButtonState.Hover:
                    using (ImageAttributes attributes = new ImageAttributes())
                    {
                        /* Draw corners. */
                        Bitmap cornerBmp = null;
                        for (int i = 0; i <= 4; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    cornerBmp = Resources.normal_left_top;
                                    rct = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, cornerBmp.Height);
                                    gfx.DrawImage(cornerBmp, rct);
                                    break;
                                case 1:
                                    cornerBmp = Resources.normal_middle_top;
                                    rct = new Rectangle(rct.Left + 6, rct.Top, rct.Width - 6, cornerBmp.Height);
                                    attributes.SetWrapMode(WrapMode.TileFlipX);
                                    gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                    break;
                                case 2:
                                    cornerBmp = Resources.normal_middle_left;
                                    rct = new Rectangle(rct.Left, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                                    attributes.SetWrapMode(WrapMode.TileFlipY);
                                    gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                    break;
                                case 3:
                                    cornerBmp = Resources.normal_left_bottom;
                                    rct = new Rectangle(rct.Left, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                                    gfx.DrawImage(cornerBmp, rct);
                                    break;
                                case 4:
                                    cornerBmp = Resources.normal_middle_bottom;
                                    rct = new Rectangle(rct.Left + 6, rct.Bottom - 6, rct.Width - 6, cornerBmp.Height);
                                    attributes.SetWrapMode(WrapMode.TileFlipX);
                                    gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                    break;
                            }
                            rct = tabPageItemRct;
                            attributes.SetWrapMode(WrapMode.Clamp);
                        }
                    }
                    rct = new Rectangle(rct.Left + ItemObjectsDrawingMargin, rct.Top + ItemObjectsDrawingMargin,
                        itemBitmap.Width, itemBitmap.Height);
                    gfx.DrawImage(itemBitmap, rct);
                    break;
                case CommonObjects.ButtonState.Pressed:
                    using (ImageAttributes attributes = new ImageAttributes())
                    {
                        /* Draw corners. */
                        Bitmap cornerBmp = null;
                        for (int i = 0; i <= 4; i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    cornerBmp = Resources.active_left_top_dark;
                                    rct = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, cornerBmp.Height);
                                    gfx.DrawImage(cornerBmp, rct);
                                    break;
                                case 1:
                                    cornerBmp = Resources.active_middle_top_dark;
                                    rct = new Rectangle(rct.Left + 6, rct.Top, rct.Width - 6, cornerBmp.Height);
                                    attributes.SetWrapMode(WrapMode.TileFlipX);
                                    gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                    break;
                                case 2:
                                    cornerBmp = Resources.active_middle_left_dark;
                                    rct = new Rectangle(rct.Left, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                                    attributes.SetWrapMode(WrapMode.TileFlipY);
                                    gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                    break;
                                case 3:
                                    cornerBmp = Resources.active_left_bottom_dark;
                                    rct = new Rectangle(rct.Left, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                                    gfx.DrawImage(cornerBmp, rct);
                                    break;
                                case 4:
                                    cornerBmp = Resources.active_middle_bottom_dark;
                                    rct = new Rectangle(rct.Left + 6, rct.Bottom - 6, rct.Width - 6, cornerBmp.Height);
                                    attributes.SetWrapMode(WrapMode.TileFlipX);
                                    gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                    break;
                            }
                            rct = tabPageItemRct;
                            attributes.SetWrapMode(WrapMode.Clamp);
                        }
                    }
                    rct.Inflate(0, -6);
                    rct.X += 6;
                    rct.Width -= 6;
                    using (Brush brush = new SolidBrush(Color.FromArgb(81, 81, 81)))
                        gfx.FillRectangle(brush, rct);
                    rct = tabPageItemRct;
                    rct = new Rectangle(rct.Left + ItemObjectsDrawingMargin + 1, rct.Top + ItemObjectsDrawingMargin + 1,
                        itemBitmap.Width, itemBitmap.Height);
                    gfx.DrawImage(itemBitmap, rct);
                    break;
                case CommonObjects.ButtonState.Normal:
                case CommonObjects.ButtonState.Disabled:
                    rct = new Rectangle(rct.Left + ItemObjectsDrawingMargin, rct.Top + ItemObjectsDrawingMargin,
                        itemBitmap.Width, itemBitmap.Height);
                    gfx.DrawImage(itemBitmap, rct);
                   break;
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
            get { return TabPageItemStyle.Only48x48_Image; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    [AddInRenderer("CCleaner",
    "CCleaner renderer class, TabPageLayout: Top(left margin 6px, tab page items between spacing 2px), TabPageItemStyle: TextAnd48x48_Image.",
    DeveloperName = "Burak Özdiken", VersionNumber = "3.0.0.0")]
    public sealed class CCleanerRendererVS3 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static CCleanerRendererVS3()
        {
            MY_FONT = new Font("Malgun Gothic", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(6, 6, 6, 6),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 6, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(103, 103, 103),
                // TabPageItemForeColor
                Color.White,
                // SelectedTabPageItemForeColor
                Color.White,
                // DisabledTabPageItemForeColor
                Color.FromArgb(172, 168, 153),
                // MouseOverTabPageItemForeColor
                Color.White
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 4, 2 };
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
            Rectangle destRect, rct = tabPageAreaRct;
            using (ImageAttributes attributes = new ImageAttributes())
            {
                /* Draw panel corners. */
                Bitmap cornerBmp = null;
                for (int i = 0; i <= 7; i++)
                {
                    switch (i)
                    {
                        case 0:
                            cornerBmp = Resources.panel_left_top;
                            destRect = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 1:
                            cornerBmp = Resources.panel_middle_top;
                            destRect = new Rectangle(rct.Left + 6, rct.Top, rct.Width - 12, cornerBmp.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipX);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 2:
                            cornerBmp = Resources.panel_right_top;
                            destRect = new Rectangle(rct.Right - 6, rct.Top, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 3:
                            cornerBmp = Resources.panel_middle_left;
                            destRect = new Rectangle(rct.Left, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 4:
                            cornerBmp = Resources.panel_middle_right;
                            destRect = new Rectangle(rct.Right - 6, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 5:
                            cornerBmp = Resources.panel_left_bottom;
                            destRect = new Rectangle(rct.Left, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 6:
                            cornerBmp = Resources.panel_middle_bottom;
                            destRect = new Rectangle(rct.Left + 6, rct.Bottom - 6, rct.Width - 12, cornerBmp.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipX);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 7:
                            cornerBmp = Resources.panel_right_bottom;
                            destRect = new Rectangle(rct.Right - 6, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                    }
                    attributes.SetWrapMode(WrapMode.Clamp);
                }
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            Bitmap itemBitmap = null;
            switch (tabPageText.ToUpperInvariant())
            {
                case "JUDGEMENT":
                    itemBitmap = Resources._01;
                    break;
                case "ALGORITHM":
                    itemBitmap = Resources._02;
                    break;
                case "CAMERA":
                    itemBitmap = Resources._01;
                    break;
                case "PLC":
                    itemBitmap = Resources._02;
                    break;
                case "LOG":
                    itemBitmap = Resources._03;
                    break;
                case "DATA":
                    itemBitmap = Resources._01;
                    break;
                case "SYSTEM":
                    itemBitmap = Resources._02;
                    break;
                default:
                    itemBitmap = Resources._01;
                    break;
            }
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            /* Draw corners. */
                            Bitmap cornerBmp = null;
                            for (int i = 0; i <= 4; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        cornerBmp = Resources.normal_left_top;
                                        rct = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 1:
                                        cornerBmp = Resources.normal_middle_top;
                                        rct = new Rectangle(rct.Left + 6, rct.Top, rct.Width - 12, cornerBmp.Height);
                                        attributes.SetWrapMode(WrapMode.TileFlipX);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 2:
                                        cornerBmp = Resources.normal_right_top;
                                        rct = new Rectangle(rct.Right - 6, rct.Top, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 3:
                                        cornerBmp = Resources.normal_middle_left;
                                        rct = new Rectangle(rct.Left, rct.Top + 6, cornerBmp.Width, rct.Height - 6);
                                        attributes.SetWrapMode(WrapMode.TileFlipY);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 4:
                                        cornerBmp = Resources.normal_middle_right;
                                        rct = new Rectangle(rct.Right - 6, rct.Top + 6, cornerBmp.Width, rct.Height - 6);
                                        attributes.SetWrapMode(WrapMode.TileFlipY);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                }
                                rct = tabPageItemRct;
                                attributes.SetWrapMode(WrapMode.Clamp);
                            }
                        }
                        rct = new Rectangle(rct.Left + (rct.Width / 2 - itemBitmap.Width / 2), rct.Top + ItemObjectsDrawingMargin,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = new Rectangle(tabPageItemRct.Left, rct.Bottom, tabPageItemRct.Width, tabPageItemRct.Height - ItemObjectsDrawingMargin - 48);
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Normal:
                        rct = new Rectangle(rct.Left + (rct.Width / 2 - itemBitmap.Width / 2), rct.Top + ItemObjectsDrawingMargin,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = new Rectangle(tabPageItemRct.Left, rct.Bottom, tabPageItemRct.Width, tabPageItemRct.Height - ItemObjectsDrawingMargin - 48);
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            /* Draw corners. */
                            Bitmap cornerBmp = null;
                            for (int i = 0; i <= 4; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        cornerBmp = Resources.active_left_top_dark;
                                        rct = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 1:
                                        cornerBmp = Resources.active_middle_top_dark;
                                        rct = new Rectangle(rct.Left + 6, rct.Top, rct.Width - 12, cornerBmp.Height);
                                        attributes.SetWrapMode(WrapMode.TileFlipX);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 2:
                                        cornerBmp = Resources.active_right_top_dark;
                                        rct = new Rectangle(rct.Right - 6, rct.Top, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 3:
                                        cornerBmp = Resources.active_middle_left_dark;
                                        rct = new Rectangle(rct.Left, rct.Top + 6, cornerBmp.Width, rct.Height - 6);
                                        attributes.SetWrapMode(WrapMode.TileFlipY);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 4:
                                        cornerBmp = Resources.active_middle_right_dark;
                                        rct = new Rectangle(rct.Right - 6, rct.Top + 6, cornerBmp.Width, rct.Height - 6);
                                        attributes.SetWrapMode(WrapMode.TileFlipY);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                }
                                rct = tabPageItemRct;
                                attributes.SetWrapMode(WrapMode.Clamp);
                            }
                        }
                        rct.Inflate(-6, 0);
                        rct.Y += 6;
                        rct.Height -= 6;
                        using (Brush brush = new SolidBrush(Color.FromArgb(81, 81, 81)))
                            gfx.FillRectangle(brush, rct);
                        rct = tabPageItemRct;
                        rct = new Rectangle(rct.Left + (rct.Width / 2 - itemBitmap.Width / 2) + 1, rct.Top + ItemObjectsDrawingMargin + 1,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = new Rectangle(tabPageItemRct.Left, rct.Bottom, tabPageItemRct.Width, tabPageItemRct.Height - ItemObjectsDrawingMargin - 48);
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        rct = new Rectangle(rct.Left + (rct.Width / 2 - itemBitmap.Width / 2), rct.Top + ItemObjectsDrawingMargin,
                            itemBitmap.Width, itemBitmap.Height);
                        gfx.DrawImage(itemBitmap, rct);
                        rct = new Rectangle(tabPageItemRct.Left, rct.Bottom, tabPageItemRct.Width, tabPageItemRct.Height - ItemObjectsDrawingMargin - 48);
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
            get { return TabPageItemStyle.TextAnd48x48_Image; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    [AddInRenderer("CCleaner",
    "CCleaner renderer class, TabPageLayout: Bottom(left margin 6px, tab page items between spacing 2px), TabPageItemStyle: OnlyText.",
    DeveloperName = "Burak Özdiken", VersionNumber = "4.0.0.0")]
    public sealed class CCleanerRendererVS4 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static CCleanerRendererVS4()
        {
            MY_FONT = new Font("Malgun Gothic", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(6, 6, 6, 6),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 6, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(103, 103, 103),
                // TabPageItemForeColor
                Color.White,
                // SelectedTabPageItemForeColor
                Color.White,
                // DisabledTabPageItemForeColor
                Color.FromArgb(172, 168, 153),
                // MouseOverTabPageItemForeColor
                Color.White
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 6, 2 };
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
            Rectangle destRect, rct = tabPageAreaRct;
            using (ImageAttributes attributes = new ImageAttributes())
            {
                /* Draw panel corners. */
                Bitmap cornerBmp = null;
                for (int i = 0; i <= 7; i++)
                {
                    switch (i)
                    {
                        case 0:
                            cornerBmp = Resources.panel_left_top;
                            destRect = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 1:
                            cornerBmp = Resources.panel_middle_top;
                            destRect = new Rectangle(rct.Left + 6, rct.Top, rct.Width - 12, cornerBmp.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipX);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 2:
                            cornerBmp = Resources.panel_right_top;
                            destRect = new Rectangle(rct.Right - 6, rct.Top, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 3:
                            cornerBmp = Resources.panel_middle_left;
                            destRect = new Rectangle(rct.Left, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 4:
                            cornerBmp = Resources.panel_middle_right;
                            destRect = new Rectangle(rct.Right - 6, rct.Top + 6, cornerBmp.Width, rct.Height - 12);
                            attributes.SetWrapMode(WrapMode.TileFlipY);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 5:
                            cornerBmp = Resources.panel_left_bottom;
                            destRect = new Rectangle(rct.Left, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                        case 6:
                            cornerBmp = Resources.panel_middle_bottom;
                            destRect = new Rectangle(rct.Left + 6, rct.Bottom - 6, rct.Width - 12, cornerBmp.Height);
                            attributes.SetWrapMode(WrapMode.TileFlipX);
                            gfx.DrawImage(cornerBmp, destRect, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                            break;
                        case 7:
                            cornerBmp = Resources.panel_right_bottom;
                            destRect = new Rectangle(rct.Right - 6, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                            gfx.DrawImage(cornerBmp, destRect);
                            break;
                    }
                    attributes.SetWrapMode(WrapMode.Clamp);
                }
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            /* Draw corners. */
                            Bitmap cornerBmp = null;
                            for (int i = 0; i <= 4; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        cornerBmp = Resources.normal_left_bottom;
                                        rct = new Rectangle(rct.Left, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 1:
                                        cornerBmp = Resources.normal_middle_bottom;
                                        rct = new Rectangle(rct.Left + 6, rct.Bottom - 6, rct.Width - 12, cornerBmp.Height);
                                        attributes.SetWrapMode(WrapMode.TileFlipX);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 2:
                                        cornerBmp = Resources.normal_right_bottom;
                                        rct = new Rectangle(rct.Right - 6, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 3:
                                        cornerBmp = Resources.normal_middle_left;
                                        rct = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, rct.Height - 6);
                                        attributes.SetWrapMode(WrapMode.TileFlipY);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 4:
                                        cornerBmp = Resources.normal_middle_right;
                                        rct = new Rectangle(rct.Right - 6, rct.Top, cornerBmp.Width, rct.Height - 6);
                                        attributes.SetWrapMode(WrapMode.TileFlipY);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                }
                                rct = tabPageItemRct;
                                attributes.SetWrapMode(WrapMode.Clamp);
                            }
                        }
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Normal:
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            /* Draw corners. */
                            Bitmap cornerBmp = null;
                            for (int i = 0; i <= 4; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        cornerBmp = Resources.active_left_bottom_dark;
                                        rct = new Rectangle(rct.Left, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 1:
                                        cornerBmp = Resources.active_middle_bottom_dark;
                                        rct = new Rectangle(rct.Left + 6, rct.Bottom - 6, rct.Width - 12, cornerBmp.Height);
                                        attributes.SetWrapMode(WrapMode.TileFlipX);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 2:
                                        cornerBmp = Resources.active_right_bottom_dark;
                                        rct = new Rectangle(rct.Right - 6, rct.Bottom - 6, cornerBmp.Width, cornerBmp.Height);
                                        gfx.DrawImage(cornerBmp, rct);
                                        break;
                                    case 3:
                                        cornerBmp = Resources.active_middle_left_dark;
                                        rct = new Rectangle(rct.Left, rct.Top, cornerBmp.Width, rct.Height - 6);
                                        attributes.SetWrapMode(WrapMode.TileFlipY);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                    case 4:
                                        cornerBmp = Resources.active_middle_right_dark;
                                        rct = new Rectangle(rct.Right - 6, rct.Top, cornerBmp.Width, rct.Height - 6);
                                        attributes.SetWrapMode(WrapMode.TileFlipY);
                                        gfx.DrawImage(cornerBmp, rct, 0, 0, cornerBmp.Width, cornerBmp.Height, GraphicsUnit.Pixel, attributes);
                                        break;
                                }
                                rct = tabPageItemRct;
                                attributes.SetWrapMode(WrapMode.Clamp);
                            }
                        }
                        rct.Inflate(-6, 0);
                        rct.Height -= 6;
                        using (Brush brush = new SolidBrush(Color.FromArgb(81, 81, 81)))
                            gfx.FillRectangle(brush, rct);
                        rct = tabPageItemRct;
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
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
            get { return TabPageLayout.Bottom; }
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