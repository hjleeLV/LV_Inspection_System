using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary.Renderer.VS2012
{
    [AddInRenderer("VS2012Like",
        "VS2012Like renderer class, TabPageLayout: Top, TabPageItemStyle: OnlyText, IsSupportEditor = true",
        DeveloperName = "Burak Özdiken", VersionNumber = "1.0.0.0", IsSupportEditor = true)]
    public sealed class VS2012LikeRenderer : RendererBase
    {
        #region Symbolic Constants

        private static readonly DrawingOffset[] OFFSETS;
        private static readonly Settings settings;

        #endregion

        #region Static Constructor

        static VS2012LikeRenderer()
        {
            OFFSETS = new DrawingOffset[] {
                // PAGE_AREA_OFFSET
                new DrawingOffset(2, 0, 0, 0),
                // ITEM_AREA_OFFSET
                new DrawingOffset(0, 0, 0, 0) };

            settings = new Settings()
            {
                NeoTabPageItemsFont = new Font("Arial", 8.25f),
                BackColor = Color.FromArgb(85, 41, 41),
                TabPageItemForeColor = Color.White,
                SelectedTabPageItemForeColor = Color.White,
                DisabledTabPageItemForeColor = SystemColors.GrayText,
                MouseOverTabPageItemForeColor = Color.White,
                TabItemFirstColor = Color.FromArgb(130, 77, 77),
                TabItemSecondColor = Color.FromArgb(120, 63, 63),
                TabItemHoverFirstColor = Color.FromArgb(140, 85, 85),
                TabItemHoverSecondColor = Color.FromArgb(130, 75, 75),
                ItemObjectsDrawingMargin = 4,
                TabPageItemsBetweenSpacing = 1
            };
        }

        #endregion

        public override void InvokeEditor()
        {
            using (Editor editor = new Editor())
            {
                editor.TemplateSettings = settings.Clone() as Settings;
                if (editor.ShowDialog() 
                    == System.Windows.Forms.DialogResult.OK)
                {
                    settings.NeoTabPageItemsFont = editor.TemplateSettings.NeoTabPageItemsFont;
                    settings.BackColor = editor.TemplateSettings.BackColor;
                    settings.TabPageItemForeColor = editor.TemplateSettings.TabPageItemForeColor;
                    settings.SelectedTabPageItemForeColor = editor.TemplateSettings.SelectedTabPageItemForeColor;
                    settings.DisabledTabPageItemForeColor = editor.TemplateSettings.DisabledTabPageItemForeColor;
                    settings.MouseOverTabPageItemForeColor = editor.TemplateSettings.MouseOverTabPageItemForeColor;
                    settings.TabItemFirstColor = editor.TemplateSettings.TabItemFirstColor;
                    settings.TabItemSecondColor = editor.TemplateSettings.TabItemSecondColor;
                    settings.TabItemHoverFirstColor = editor.TemplateSettings.TabItemHoverFirstColor;
                    settings.TabItemHoverSecondColor = editor.TemplateSettings.TabItemHoverSecondColor;
                    settings.ItemObjectsDrawingMargin = editor.TemplateSettings.ItemObjectsDrawingMargin;
                    settings.TabPageItemsBetweenSpacing = editor.TemplateSettings.TabPageItemsBetweenSpacing;
                    base.OnRendererUpdated();
                }
            }
        }

        public override void OnRendererBackground(Graphics gfx, Rectangle clientRct)
        {
            // Do nothing.
        }

        public override void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct)
        {
            Rectangle linesRct = tabPageAreaRct;
            // Draw top 2px line.
            using (Brush brush = new SolidBrush(settings.TabItemSecondColor))
                gfx.FillRectangle(brush, linesRct.Left, linesRct.Top, linesRct.Width, 2);
        }

        public override void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, CommonObjects.ButtonState btnState)
        {
            Rectangle itemRct = tabPageItemRct;
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
            {
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
                switch (btnState)
                {
                    case CommonObjects.ButtonState.Hover:
                        itemRct.Y += 1;
                        itemRct.Height -= 1;
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, settings.TabItemHoverFirstColor,
                            settings.TabItemHoverSecondColor, LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        itemRct.X += 5;
                        itemRct.Width -= 5;
                        using (Brush brush = new SolidBrush(MouseOverTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Normal:
                        itemRct.Y += 1;
                        itemRct.Height -= 1;
                        itemRct.X += 5;
                        itemRct.Width -= 5;
                        using (Brush brush = new SolidBrush(TabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Pressed:
                        using (LinearGradientBrush brush = new LinearGradientBrush(itemRct, settings.TabItemFirstColor,
                            settings.TabItemSecondColor, LinearGradientMode.Vertical))
                        {
                            Blend bl = new Blend(2);
                            bl.Factors = new float[] { 0.3F, 1.0F };
                            bl.Positions = new float[] { 0.0F, 1.0F };
                            brush.Blend = bl;
                            gfx.FillRectangle(brush, itemRct);
                        }
                        itemRct.X += 5;
                        itemRct.Width -= 5;
                        using (Brush brush = new SolidBrush(SelectedTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                    case CommonObjects.ButtonState.Disabled:
                        itemRct.Y += 1;
                        itemRct.Height -= 1;
                        itemRct.X += 5;
                        itemRct.Width -= 5;
                        using (Brush brush = new SolidBrush(DisabledTabPageItemForeColor))
                            gfx.DrawString(tabPageText, NeoTabPageItemsFont, brush, itemRct, format);
                        break;
                }
            }
        }

        public override int ItemObjectsDrawingMargin
        {
            get { return settings.ItemObjectsDrawingMargin; }
        }

        public override int TabPageItemsBetweenSpacing
        {
            get { return settings.TabPageItemsBetweenSpacing; }
        }

        public override Color BackColor
        {
            get { return settings.BackColor; }
        }

        public override Color TabPageItemForeColor
        {
            get { return settings.TabPageItemForeColor; }
        }

        public override Color SelectedTabPageItemForeColor
        {
            get { return settings.SelectedTabPageItemForeColor; }
        }

        public override Color DisabledTabPageItemForeColor
        {
            get { return settings.DisabledTabPageItemForeColor; }
        }

        public override Color MouseOverTabPageItemForeColor
        {
            get { return settings.MouseOverTabPageItemForeColor; }
        }

        public override Font NeoTabPageItemsFont
        {
            get { return settings.NeoTabPageItemsFont; }
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