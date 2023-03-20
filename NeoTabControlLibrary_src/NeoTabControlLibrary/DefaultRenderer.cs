using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary
{
    [AddInRenderer("DefaultRenderer",
        "System tab control renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0")]
    public sealed class DefaultRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly Font MY_FONT;
        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Color[] COLORS;
        private static readonly int[] INTEGERARRAY;

        #endregion

        #region Static Constructor

        static DefaultRenderer()
        {
            MY_FONT = new Font("Tahoma", 9.25f);
            
            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(2, 4, 4, 4),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, 0) };
            
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
            INTEGERARRAY = new int[] { 2, 0 };
        }

        #endregion

        public override bool IsSupportSmartCloseButton
        {
            get
            {
                return true;
            }
        }

        public override bool IsSupportSmartDropDownButton
        {
            get
            {
                return true;
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
            if (Application.RenderWithVisualStyles)
            {
                VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.Tab.Pane.Normal);
                renderer.DrawBackground(gfx, tabPageAreaRct);
            }
            else
            {
                Rectangle linesRct = tabPageAreaRct;
                // Draw top 2px line.
                using (Brush brush = new SolidBrush(Color.FromArgb(2, 111, 194)))
                    gfx.FillRectangle(brush, linesRct.Left, linesRct.Top, linesRct.Width, 2);
                // Draw left, right and bottom 1px lines.
                using (Pen pen = new Pen(Color.FromArgb(183, 192, 197)))
                {
                    // Create border points.
                    Point[] points = new Point[]
                    {
                        // Left point.
                        new Point(linesRct.Left, linesRct.Top + 2),
                        // Bottom points.
                        new Point(linesRct.Left, linesRct.Bottom - 1),
                        new Point(linesRct.Right - 1, linesRct.Bottom - 1),
                        // Right point.
                        new Point(linesRct.Right - 1, linesRct.Top + 2)
                    };
                    gfx.DrawLines(pen, points);
                }
            }
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            if (Application.RenderWithVisualStyles)
            {
                Rectangle itemRct = tabPageItemRct;
                VisualStyleRenderer renderer = null;
                using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                    switch (btnState)
                    {
                        case CommonObjects.ButtonState.Hover:
                            itemRct.Y += 2;
                            itemRct.Height -= 2;
                            renderer = new VisualStyleRenderer(VisualStyleElement.Tab.TopTabItemLeftEdge.Hot);
                            renderer.DrawBackground(gfx, itemRct);
                            using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                                gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                            break;
                        case CommonObjects.ButtonState.Normal:
                            itemRct.Y += 2;
                            itemRct.Height -= 2;
                            renderer = new VisualStyleRenderer(VisualStyleElement.Tab.TopTabItemLeftEdge.Normal);
                            renderer.DrawBackground(gfx, itemRct);
                            using (Brush brush = new SolidBrush(TabPageItemForeColor))
                                gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                            break;
                        case CommonObjects.ButtonState.Pressed:
                            itemRct.Height += 2;
                            if (index == 0)
                                renderer = new VisualStyleRenderer(VisualStyleElement.Tab.TopTabItemLeftEdge.Pressed);
                            else
                                renderer = new VisualStyleRenderer(VisualStyleElement.Tab.TopTabItem.Pressed);
                            renderer.DrawBackground(gfx, itemRct);
                            using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                                gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                            break;
                        case CommonObjects.ButtonState.Disabled:
                            itemRct.Y += 2;
                            itemRct.Height -= 2;
                            renderer = new VisualStyleRenderer(VisualStyleElement.Tab.TopTabItemLeftEdge.Disabled);
                            renderer.DrawBackground(gfx, itemRct);
                            using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                                gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                            break;
                    }
                }
            }
            else
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
                            itemRct.Y += 1;
                            itemRct.Height -= 1;
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(155, 179, 217),
                                Color.FromArgb(148, 188, 223), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.4F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillRectangle(brush, itemRct);
                            }
                            // Draw left, top and right 1px lines.
                            using (Pen pen = new Pen(Color.FromArgb(242, 242, 242)))
                            {
                                // Create border points.
                                Point[] points = new Point[]
                                {
                                    // Left point.
                                    new Point(itemRct.Left, itemRct.Bottom - 1),
                                    // Top points.
                                    new Point(itemRct.Left, itemRct.Top),
                                    new Point(itemRct.Right - 2, itemRct.Top),
                                    // Right point.
                                    new Point(itemRct.Right - 2, itemRct.Bottom - 1)
                                };
                                gfx.DrawLines(pen, points);
                            }
                            using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                                gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                            break;
                        case CommonObjects.ButtonState.Normal:
                            itemRct.Y += 1;
                            itemRct.Height -= 1;
                            using (Brush brush = new SolidBrush(Color.White))
                                gfx.FillRectangle(brush, itemRct);
                            // Draw left, top and right 1px lines.
                            using (Pen pen = new Pen(Color.FromArgb(242, 242, 242)))
                            {
                                // Create border points.
                                Point[] points = new Point[]
                                {
                                    // Left point.
                                    new Point(itemRct.Left, itemRct.Bottom - 1),
                                    // Top points.
                                    new Point(itemRct.Left, itemRct.Top),
                                    new Point(itemRct.Right - 2, itemRct.Top),
                                    // Right point.
                                    new Point(itemRct.Right - 2, itemRct.Bottom - 1)
                                };
                                gfx.DrawLines(pen, points);
                            }
                            using (Brush brush = new SolidBrush(TabPageItemForeColor))
                                gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                            break;
                        case CommonObjects.ButtonState.Pressed:
                            using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, Color.FromArgb(1, 110, 193),
                                Color.FromArgb(4, 111, 191), LinearGradientMode.Vertical))
                            {
                                Blend bl = new Blend(2);
                                bl.Factors = new float[] { 0.3F, 1.0F };
                                bl.Positions = new float[] { 0.0F, 1.0F };
                                brush.Blend = bl;
                                gfx.FillRectangle(brush, itemRct);
                            }
                            using (Brush brush = new SolidBrush(Color.White))
                                gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                            break;
                        case CommonObjects.ButtonState.Disabled:
                            itemRct.Y += 1;
                            itemRct.Height -= 1;
                            using (Brush brush = new SolidBrush(SystemColors.InactiveBorder))
                                gfx.FillRectangle(brush, itemRct);
                            // Draw left, top and right 1px lines.
                            using (Pen pen = new Pen(Color.FromArgb(242, 242, 242)))
                            {
                                // Create border points.
                                Point[] points = new Point[]
                                {
                                    // Left point.
                                    new Point(itemRct.Left, itemRct.Bottom - 1),
                                    // Top points.
                                    new Point(itemRct.Left, itemRct.Top),
                                    new Point(itemRct.Right - 2, itemRct.Top),
                                    // Right point.
                                    new Point(itemRct.Right - 2, itemRct.Bottom - 1)
                                };
                                gfx.DrawLines(pen, points);
                            }
                            using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                                gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                            break;
                    }
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
}