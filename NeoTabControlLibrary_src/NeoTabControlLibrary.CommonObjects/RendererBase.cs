using System;
using System.Drawing;

namespace NeoTabControlLibrary.CommonObjects
{
    #region Struct

    public struct DrawingOffset
    {
        /// <summary>
        /// Top Pixel Offset.
        /// </summary>
        public int Top;
        
        /// <summary>
        /// Left Pixel Offset.
        /// </summary>
        public int Left;
        
        /// <summary>
        /// Bottom Pixel Offset.
        /// </summary>
        public int Bottom;
        
        /// <summary>
        /// Right Pixel Offset.
        /// </summary>
        public int Right;

        public DrawingOffset(int top, int left, int bottom, int right)
        {
            Top = top;
            Left = left;
            Bottom = bottom;
            Right = right;
        }

        public override string ToString()
        {
            return String.Format("Top: {0}px; Left: {1}px; Bottom: {2}px; Right: {3}px",
                Top, Left, Bottom, Right);
        }
    };

    #endregion

    #region Enums

    public enum TabPageItemStyle
    {
        OnlyText,
        TextAnd16x16_Image,
        TextAnd32x32_Image,
        TextAnd48x48_Image,
        Only16x16_Image,
        Only32x32_Image,
        Only48x48_Image
    };

    public enum ButtonState
    {
        Hover,
        Normal,
        Pressed,
        Disabled
    };

    public enum TabPageLayout
    {
        Top,
        Left,
        Bottom,
        Right
    };

    #endregion

    public abstract class RendererBase : IDisposable
    {
        #region Event

        public event EventHandler RendererUpdated;

        #endregion

        #region Destructor

        ~RendererBase()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        protected void OnRendererUpdated()
        {
            if (RendererUpdated != null)
                RendererUpdated(this, EventArgs.Empty);
        }

        #endregion

        #region Virtual Methods

        public virtual void OnDrawSmartCloseButton(Graphics gfx, Rectangle closeButtonRct, ButtonState btnState)
        {
            if (!IsSupportSmartCloseButton)
                return;
            Pen pen = null;
            Pen borderPen = null;
            Brush brush = null;
            switch (btnState)
            {
                case ButtonState.Normal:
                    pen = new Pen(Color.Black);
                    break;
                case ButtonState.Hover:
                    pen = new Pen(Color.Black);
                    borderPen = new Pen(Color.FromArgb(49, 106, 197));
                    brush = new SolidBrush(Color.FromArgb(195, 211, 237));
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

        public virtual void OnDrawSmartDropDownButton(Graphics gfx, Rectangle dropdownButtonRct, ButtonState btnState)
        {
            if (!IsSupportSmartDropDownButton)
                return;
            Pen pen = null;
            Pen borderPen = null;
            Brush brush = null;
            switch (btnState)
            {
                case ButtonState.Normal:
                    pen = new Pen(Color.Black);
                    break;
                case ButtonState.Hover:
                    pen = new Pen(Color.Black);
                    borderPen = new Pen(Color.FromArgb(49, 106, 197));
                    brush = new SolidBrush(Color.FromArgb(195, 211, 237));
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

        #endregion

        #region Abstract Methods

        public abstract void InvokeEditor();

        public abstract void OnRendererBackground(Graphics gfx, Rectangle clientRct);

        public abstract void OnRendererTabPageArea(Graphics gfx, Rectangle tabPageAreaRct);

        public abstract void OnRendererTabPageItem(Graphics gfx, Rectangle tabPageItemRct, string tabPageText, int index, ButtonState btnState);

        #endregion

        #region Virtual Properties

        public virtual int SmartButtonsBetweenSpacing { get { return 2; } }

        public virtual Size SmartButtonsSize { get { return new Size(14, 13); } }
        
        public virtual bool IsSupportSmartCloseButton { get { return false; } }

        public virtual bool IsSupportSmartDropDownButton { get { return false; } }

        public virtual DrawingOffset SmartButtonsAreaOffset 
        {
            get { return new DrawingOffset(0, 0, 3, 6); }
        }

        #endregion

        #region Abstract Properties

        public abstract int ItemObjectsDrawingMargin { get; }

        public abstract int TabPageItemsBetweenSpacing { get; }

        public abstract Color BackColor { get; }

        public abstract Color TabPageItemForeColor { get; }

        public abstract Color SelectedTabPageItemForeColor { get; }

        public abstract Color DisabledTabPageItemForeColor { get; }

        public abstract Color MouseOverTabPageItemForeColor { get; }

        public abstract Font NeoTabPageItemsFont { get; }

        public abstract DrawingOffset TabPageAreaCornerOffset { get; }

        public abstract DrawingOffset TabPageItemsAreaOffset { get; }

        public abstract TabPageLayout NeoTabPageItemsSide { get; }

        public abstract TabPageItemStyle NeoTabPageItemsStyle { get; }

        #endregion

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AddInRendererAttribute : Attribute
    {
        #region Destructor

        ~AddInRendererAttribute()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        // Attribute constructor for positional parameters.
        public AddInRendererAttribute(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        // Accessors.
        public string Name { get; private set; }
        public string Description { get; private set; }

        // Property for named parameters.
        public bool IsSupportEditor { get; set; }
        public string DeveloperName { get; set; }
        public string VersionNumber { get; set; }
    }
}