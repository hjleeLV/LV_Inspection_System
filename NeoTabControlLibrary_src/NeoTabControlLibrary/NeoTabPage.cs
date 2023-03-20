using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace NeoTabControlLibrary
{
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [Designer(typeof(NeoTabControlLibrary.Design.NeoTabPageDesigner))]
    [DefaultEvent("Click"), DefaultProperty("Text")]
    public class NeoTabPage : Panel, IFormattable, IComparable<NeoTabPage>
    {
        #region Events

        /// <summary>
        /// Event raised when the value of the Text property is changed on Control.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler TextChanged;

        #endregion

        #region Enums

        #endregion

        #region Symbolic Constants

        #endregion

        #region Static Members Of The Class

        #endregion

        #region Instance Members

        #endregion

        #region Constructor

        public NeoTabPage()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.Selectable, false);
            this.IsCloseable = true;
            this.IsSelectable = true;
        }

        #endregion

        #region Destructor

        ~NeoTabPage()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Property

        /// <summary>
        /// Determines whether this NeoTabPage control is closeable or not.
        /// </summary>
        [Description("Determines whether this NeoTabPage control is closeable or not")]
        [DefaultValue(true)]
        public bool IsCloseable { get; set; }

        /// <summary>
        /// Determines whether this NeoTabPage control is selectable or not.
        /// </summary>
        [Description("Determines whether this NeoTabPage control is selectable or not")]
        [DefaultValue(true)]
        public bool IsSelectable { get; set; }

        /// <summary>
        /// Gets or sets to be displayed text for this page.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                try
                {
                    if (!value.Equals(base.Text))
                    {
                        base.Text = value;
                    }
                }
                catch (NullReferenceException)
                {
                    base.Text = String.Empty;
                }
            }
        }

        /// <summary>
        /// The text that is shown when the mouse hovers over this tab.
        /// </summary>
        [Description("The text that is shown when the mouse hovers over this tab")]
        public string ToolTipText { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ProgressUsedValue { get; set; }

        #endregion

        #region Override Methods

        protected override void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
                TextChanged(this, e);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            /* Allow a null parent to support drag & drop
                from one NeoTabWindow control to another at design time. */
            if (this.Parent == null || this.Parent is NeoTabWindow)
                base.OnParentChanged(e);
            else
                throw new NotSupportedException("A NeoTabPage control can only be added to a NeoTabWindow control.");
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Managed Resources
                foreach (Control control in this.Controls)
                    control.Dispose();
            }

            base.Dispose(disposing);
        }

        public override string ToString()
        {
            return ToString("T", null);
        }

        #endregion

        #region Virtual Methods

        #endregion

        #region Helper Methods

        #endregion

        #region General Methods

        #endregion

        #region IFormattable Members

        public string ToString(string format,
            IFormatProvider formatProvider)
        {
            switch (format.ToUpper())
            {
                case null:
                case "T":   // Text
                    return this.Text.Replace("&", String.Empty);
                case "N":   // Name
                    return this.Name;
                case "O":   // Tooltip Text
                    return this.ToolTipText;
                case "A":   // All
                    return String.Format("Text: {0}, Name: {1}, TooltipText: {2}",
                        this.Text.Replace("&", String.Empty), this.Name, this.ToolTipText);
                default:
                    throw new FormatException(String.Format(
                        formatProvider,
                        "Format {0} is not supported.",
                        format));
            }
        }

        #endregion

        #region IComparable<NeoTabPage> Members

        public int CompareTo(NeoTabPage other)
        {
            /* If the int value returned from CompareTo() is less than 0, it means that the current instance is less than obj.
               If the return value is positive, the current instance is greater than obj.
               If the return value is 0, the two instances are equal. */
            int compare = this.Text.Replace("&", String.Empty).CompareTo(
                other.Text.Replace("&", String.Empty));
            if (compare == 0)
                return this.Name.CompareTo(other.Name);
            return compare;
        }

        #endregion
    }
}