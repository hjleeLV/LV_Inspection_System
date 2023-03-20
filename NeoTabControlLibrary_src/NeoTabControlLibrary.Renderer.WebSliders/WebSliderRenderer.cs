using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.WebSliders
{
    [AddInRenderer("WebSlider",
    "WebSlider renderer class, TabPageLayout: Bottom(top margin 8px, left margin 10px, tab page items between spacing 4px), TabPageItemStyle: Only16x16_Image.",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class WebSliderRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static WebSliderRenderer()
        {
            MY_FONT = null;

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(8, 10, 0, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.White,
                // TabPageItemForeColor
                Color.Empty,
                // SelectedTabPageItemForeColor
                Color.Empty,
                // DisabledTabPageItemForeColor
                Color.Empty,
                // MouseOverTabPageItemForeColor
                Color.Empty
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 0, 4 };
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
            if (Application.RenderWithVisualStyles)
            {
                VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Tab.Body.Normal);
                renderer.DrawBackground(gfx, tabPageAreaRct);
            }
            else
            {
                Rectangle linesRct = tabPageAreaRct;
                linesRct.Width -= 1;
                linesRct.Height -= 1;
                // Draw 1px border rectangle.
                using (Pen pen = new Pen(Color.FromArgb(202, 202, 214)))
                    gfx.DrawRectangle(pen, linesRct);
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            Bitmap bmp = null;
            switch (btnState)
            {
                case CommonObjects.ButtonState.Hover:
                    bmp = Resources.RadiobuttonActiveHover;
                    break;
                case CommonObjects.ButtonState.Pressed:
                    bmp = Resources.RadiobuttonActiveSelectedHover;
                    break;
                case CommonObjects.ButtonState.Normal:
                    bmp = Resources.RadiobuttonActive;
                    break;
                case CommonObjects.ButtonState.Disabled:
                    bmp = Resources.RadiobuttonInactive;
                    break;
            }
            using (Brush brush = new SolidBrush(BackColor))
                gfx.FillRectangle(brush, rct);
            gfx.DrawImage(bmp, rct);
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
            get { return TabPageItemStyle.Only16x16_Image; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    [AddInRenderer("WebSlider",
    "WebSlider renderer class, TabPageLayout: Top(bottom margin 8px, left margin 10px, tab page items between spacing 4px), TabPageItemStyle: Only16x16_Image.",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.1.0.0")]
    public sealed class WebSliderRendererVS2 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static WebSliderRendererVS2()
        {
            MY_FONT = null;

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 10, 8, 0) };

            COLORS = new Color[]{
                // BackColor
                Color.White,
                // TabPageItemForeColor
                Color.Empty,
                // SelectedTabPageItemForeColor
                Color.Empty,
                // DisabledTabPageItemForeColor
                Color.Empty,
                // MouseOverTabPageItemForeColor
                Color.Empty
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 0, 4 };
        }

        #endregion

        public override void InvokeEditor()
        {
            throw new NotImplementedException();
        }

        public override void OnRendererBackground(Graphics gfx, Rectangle clientRct)
        {
            // Do Nothing.
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            if (Application.RenderWithVisualStyles)
            {
                VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Tab.Body.Normal);
                renderer.DrawBackground(gfx, tabPageAreaRct);
            }
            else
            {
                Rectangle linesRct = tabPageAreaRct;
                linesRct.Width -= 1;
                linesRct.Height -= 1;
                // Draw 1px border rectangle.
                using (Pen pen = new Pen(Color.FromArgb(202, 202, 214)))
                    gfx.DrawRectangle(pen, linesRct);
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            Bitmap bmp = null;
            switch (btnState)
            {
                case CommonObjects.ButtonState.Hover:
                    bmp = Resources.RadiobuttonActiveHover;
                    break;
                case CommonObjects.ButtonState.Pressed:
                    bmp = Resources.RadiobuttonActiveSelectedHover;
                    break;
                case CommonObjects.ButtonState.Normal:
                    bmp = Resources.RadiobuttonActive;
                    break;
                case CommonObjects.ButtonState.Disabled:
                    bmp = Resources.RadiobuttonInactive;
                    break;
            }
            using (Brush brush = new SolidBrush(BackColor))
                gfx.FillRectangle(brush, rct);
            gfx.DrawImage(bmp, rct);
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
            get { return TabPageItemStyle.Only16x16_Image; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    [AddInRenderer("WebSlider",
    "WebSlider renderer class, TabPageLayout: Left(top margin 8px, right margin 10px, tab page items between spacing 4px), TabPageItemStyle: Only16x16_Image, hover style is not implemented",
    DeveloperName = "Burak Özdiken", VersionNumber = "1.2.0.0")]
    public sealed class WebSliderRendererVS3 : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static WebSliderRendererVS3()
        {
            MY_FONT = new Font("Courier New", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(8, 0, 0, 10) };

            COLORS = new Color[]{
                // BackColor
                Color.White,
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
            INTEGERARRAY = new int[] { 2, 4 };
        }

        #endregion

        public override void InvokeEditor()
        {
            throw new NotImplementedException();
        }

        public override void OnRendererBackground(Graphics gfx, Rectangle clientRct)
        {
            // Do Nothing.
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            if (Application.RenderWithVisualStyles)
            {
                VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Tab.Body.Normal);
                renderer.DrawBackground(gfx, tabPageAreaRct);
            }
            else
            {
                Rectangle linesRct = tabPageAreaRct;
                linesRct.Width -= 1;
                linesRct.Height -= 1;
                // Draw 1px border rectangle.
                using (Pen pen = new Pen(Color.FromArgb(202, 202, 214)))
                    gfx.DrawRectangle(pen, linesRct);
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle rct = tabPageItemRct;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Pressed:
                        using (LinearGradientBrush brush = new LinearGradientBrush(rct, Color.FromArgb(155, 192, 16),
                            Color.FromArgb(148, 157, 28), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, rct);
                        }
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(index.ToString(), NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Hover:
                    case CommonObjects.ButtonState.Normal:
                        using (LinearGradientBrush brush = new LinearGradientBrush(rct, Color.FromArgb(255, 192, 16),
                            Color.FromArgb(248, 157, 28), LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, rct);
                        }
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(index.ToString(), NeoTabPageItemsFont, brush, rct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        using (LinearGradientBrush brush = new LinearGradientBrush(rct, Color.Gainsboro,
                            Color.Silver, LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, rct);
                        }
                        using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                            gfx.DrawString(index.ToString(), NeoTabPageItemsFont, brush, rct, format);
                        break;
                }
                rct.Width -= 1;
                rct.Height -= 1;
                // Draw 1px border rectangle.
                using (Pen pen = new Pen(Color.FromArgb(202, 202, 214)))
                    gfx.DrawRectangle(pen, rct);
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
            get { return TabPageItemStyle.Only16x16_Image; }
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}