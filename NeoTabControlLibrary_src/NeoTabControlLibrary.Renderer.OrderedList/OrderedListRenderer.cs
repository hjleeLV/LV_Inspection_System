using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.OrderedList
{
    [AddInRenderer("OrderedList",
        "OrderedList renderer class, TabPageLayout: Left",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class OrderedListRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static OrderedListRenderer()
        {
            MY_FONT = new Font("Tahoma", 9.25f, FontStyle.Bold);

            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(1, 1, 1, 1),
                // ITEM_AREA_OFFSET
                new DrawingOffset(4, 0, 0, 8) };

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
                Color.White
            };

            // ItemObjectsDrawingMargin, TabPageItemsBetweenSpacing
            INTEGERARRAY = new int[] { 4, 8 };
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
            SmoothingMode mode = gfx.SmoothingMode;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(itemRct);
                path.CloseFigure();
                using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    switch (btnState)
                    {
                        case CommonObjects.ButtonState.Hover:
                        case CommonObjects.ButtonState.Normal:
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(204, 199, 186),
                                Color.FromArgb(204, 199, 186), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.3F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillPath(brush, path);
                            }
                            using (Brush brush = new SolidBrush(TabPageItemForeColor))
                                gfx.DrawString(String.Format("{0}", index + 1), NeoTabPageItemsFont, brush, itemRct, format);
                            break;
                        case CommonObjects.ButtonState.Pressed:
                            using (Brush brush = new SolidBrush(BackColor))
                                gfx.FillRectangle(brush, itemRct.Left - 1, itemRct.Top - 1, itemRct.Width + 1, itemRct.Height + 1);
                            using (PathGradientBrush brush = new PathGradientBrush(path))
                            {
                                brush.CenterColor = Color.Blue;
                                brush.SurroundColors = new Color[]
                                {
                                    Color.LightSteelBlue
                                };
                                gfx.FillPath(brush, path);
                                gfx.DrawPath(Pens.LightGray, path);
                            }
                            using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                                gfx.DrawString(String.Format("{0}", index + 1), NeoTabPageItemsFont, brush, itemRct, format);
                            break;
                        case CommonObjects.ButtonState.Disabled:
                            using (Brush brush = new SolidBrush(SystemColors.InactiveBorder))
                                gfx.FillPath(brush, path);
                            using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                                gfx.DrawString(String.Format("{0}", index + 1), NeoTabPageItemsFont, brush, itemRct, format);
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