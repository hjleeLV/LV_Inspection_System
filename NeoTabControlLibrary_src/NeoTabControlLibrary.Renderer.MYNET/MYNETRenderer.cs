using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.MYNET
{
    [AddInRenderer("MYNET",
        "MYNET renderer class, TabPageLayout: Top(tab page items between spacing 2px), TabPageItemStyle: OnlyText, hover and disabled styles are not implemented.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class MYNETRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static MYNETRenderer()
        {
            MY_FONT = new Font("Arial", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(227, 241, 243),
                // TabPageItemForeColor
                Color.FromArgb(94, 94, 94),
                // SelectedTabPageItemForeColor
                Color.FromArgb(94, 94, 94),
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.FromArgb(94, 94, 94),
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 5, 2 };
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
            using (Pen pen = new Pen(Color.FromArgb(185, 216, 230)))
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
                    case CommonObjects.ButtonState.Normal:
                    case CommonObjects.ButtonState.Disabled:
                        itemRct.Height += 1;
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            Bitmap bmp = Resources.normal;
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            gfx.DrawImage(bmp, itemRct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        itemRct.Height += 1;
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            Bitmap bmp = Resources.over;
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            gfx.DrawImage(bmp, itemRct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
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

    [AddInRenderer("MYNET",
        "MYNET renderer class with small icons, TabPageLayout: Top(tab page items between spacing 2px), TabPageItemStyle: TextAnd16x16_Image, hover and disabled styles are not implemented.",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.1.0.0")]
    public sealed class MYNETRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static MYNETRendererVS2()
        {
            MY_FONT = new Font("Arial", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.FromArgb(227, 241, 243),
                // TabPageItemForeColor
                Color.FromArgb(94, 94, 94),
                // SelectedTabPageItemForeColor
                Color.FromArgb(94, 94, 94),
                // DisabledTabPageItemForeColor
                SystemColors.GrayText,
                // MouseOverTabPageItemForeColor
                Color.FromArgb(94, 94, 94),
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 5, 2 };
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
            using (Pen pen = new Pen(Color.FromArgb(185, 216, 230)))
                gfx.DrawRectangle(pen, linesRct);
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle itemRct = tabPageItemRct;
            Bitmap itemBitmap = null;
            switch (tabPageText.ToUpperInvariant())
            {
                case "SOLUTION &EXPLORER":
                    itemBitmap = Resources.AddTableHS;
                    break;
                case "PRO&PERTIES":
                    itemBitmap = Resources.AlignObjectsBottomHS;
                    break;
                case "&TOOLBOX":
                    itemBitmap = Resources.AlignToGridHS;
                    break;
                case "ERROR &LIST":
                    itemBitmap = Resources.OutdentHS;
                    break;
                case "&OUTPUT":
                    itemBitmap = Resources.Size;
                    break;
                default:
                    itemBitmap = Resources.AddTableHS;
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
                    case CommonObjects.ButtonState.Normal:
                    case CommonObjects.ButtonState.Disabled:
                        itemRct.Height += 1;
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            Bitmap bmp = Resources.normal;
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            gfx.DrawImage(bmp, itemRct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        gfx.DrawImage(itemBitmap, itemRct.Left + ItemObjectsDrawingMargin, itemRct.Top + ItemObjectsDrawingMargin, itemBitmap.Width, itemBitmap.Height);
                        int xoffset = ItemObjectsDrawingMargin + itemBitmap.Width;
                        itemRct.X += xoffset;
                        itemRct.Width -= xoffset;
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        itemRct.Height += 1;
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            Bitmap bmp = Resources.over;
                            attributes.SetWrapMode(WrapMode.TileFlipXY);
                            gfx.DrawImage(bmp, itemRct, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        }
                        gfx.DrawImage(itemBitmap, itemRct.Left + ItemObjectsDrawingMargin, itemRct.Top + ItemObjectsDrawingMargin, itemBitmap.Width, itemBitmap.Height);
                        int xoffset2 = ItemObjectsDrawingMargin + itemBitmap.Width;
                        itemRct.X += xoffset2;
                        itemRct.Width -= xoffset2;
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
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
            get { return TabPageItemStyle.TextAnd16x16_Image; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}