using System;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace NeoTabControlLibrary
{
    [Editor(typeof(TooltipRendererEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(TooltipRendererConverter))]
    public class TooltipRenderer : IDisposable
    {
        #region Instance Members

        private byte radius = 8;
        private Color[] colorArray = {
            Color.FromArgb(118, 118, 118), 
            Color.Black, 
            Color.White,
            Color.Gray,
            Color.DarkGray,
            Color.LightGray,
            Color.DarkGray,
            Color.Gray,
            Color.White,
            Color.WhiteSmoke
        };

        #endregion

        #region Constructor

        public TooltipRenderer() { }

        public TooltipRenderer(Color borderColor, Color titleColor, Color messageColor,
            Color lightBackgroundColor, Color darkBackgroundColor, byte radius,
            Color barBorderColor, Color barBackgroundColorStart, Color barBackgroundColorEnd,
            Color barProgressColorStart, Color barProgressColorEnd)
        {
            this.colorArray[0] = borderColor;
            this.colorArray[1] = titleColor;
            this.colorArray[2] = messageColor;
            this.colorArray[3] = lightBackgroundColor;
            this.colorArray[4] = darkBackgroundColor;
            this.radius = radius;
            this.colorArray[5] = barBorderColor;
            this.colorArray[6] = barBackgroundColorStart;
            this.colorArray[7] = barBackgroundColorEnd;
            this.colorArray[8] = barProgressColorStart;
            this.colorArray[9] = barProgressColorEnd;
        }

        #endregion

        #region Property

        /// <summary>
        /// This property must be in the range of 0 to 8.
        /// </summary>
        [Description("This property must be in the range of 0 to 8")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Byte), "8")]
        public byte Radius
        {
            get { return radius; }
            set 
            {
                if (!value.Equals(radius))
                {
                    if (value > 8)
                        value = 8;
                    radius = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color of the tooltip rectangle.
        /// </summary>
        [Description("Gets or sets the border color of the tooltip rectangle")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "118, 118, 118")]
        public Color BorderColor
        {
            get { return colorArray[0]; }
            set
            {
                if (!value.Equals(colorArray[0]))
                    colorArray[0] = value;
            }
        }

        /// <summary>
        /// Gets or sets the fore color of the tooltip title.
        /// </summary>
        [Description("Gets or sets the fore color of the tooltip title")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "Black")]
        public Color TitleForeColor
        {
            get { return colorArray[1]; }
            set
            {
                if (!value.Equals(colorArray[1]))
                    colorArray[1] = value;
            }
        }

        /// <summary>
        /// Gets or sets the fore color of the tooltip message.
        /// </summary>
        [Description("Gets or sets the fore color of the tooltip message")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "White")]
        public Color MessageForeColor
        {
            get { return colorArray[2]; }
            set
            {
                if (!value.Equals(colorArray[2]))
                    colorArray[2] = value;
            }
        }

        /// <summary>
        /// Gets or sets the light background color of the tooltip rectangle.
        /// </summary>
        [Description("Gets or sets the light background color of the tooltip rectangle")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "Gray")]
        public Color LightBackgroundColor
        {
            get { return colorArray[3]; }
            set
            {
                if (!value.Equals(colorArray[3]))
                    colorArray[3] = value;
            }
        }

        /// <summary>
        /// Gets or sets the dark background color of the tooltip rectangle.
        /// </summary>
        [Description("Gets or sets the dark background color of the tooltip rectangle")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "DarkGray")]
        public Color DarkBackgroundColor
        {
            get { return colorArray[4]; }
            set
            {
                if (!value.Equals(colorArray[4]))
                    colorArray[4] = value;
            }
        }

        /// <summary>
        /// Gets or sets the border color of the ProgressBar.
        /// </summary>
        [Description("Gets or sets the border color of the ProgressBar")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "LightGray")]
        public Color BarBorderColor
        {
            get { return colorArray[5]; }
            set
            {
                if (!value.Equals(colorArray[5]))
                    colorArray[5] = value;
            }
        }

        /// <summary>
        /// Gets or sets the first background color of the ProgressBar.
        /// </summary>
        [Description("Gets or sets the first background color of the ProgressBar")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "DarkGray")]
        public Color BarBackgroundColorStart
        {
            get { return colorArray[6]; }
            set
            {
                if (!value.Equals(colorArray[6]))
                    colorArray[6] = value;
            }
        }

        /// <summary>
        /// Gets or sets the second background color of the ProgressBar.
        /// </summary>
        [Description("Gets or sets the second background color of the ProgressBar")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "Gray")]
        public Color BarBackgroundColorEnd
        {
            get { return colorArray[7]; }
            set
            {
                if (!value.Equals(colorArray[7]))
                    colorArray[7] = value;
            }
        }

        /// <summary>
        /// Gets or sets the first progress color.
        /// </summary>
        [Description("Gets or sets the first progress color")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "White")]
        public Color BarProgressColorStart
        {
            get { return colorArray[8]; }
            set
            {
                if (!value.Equals(colorArray[8]))
                    colorArray[8] = value;
            }
        }

        /// <summary>
        /// Gets or sets the second progress color.
        /// </summary>
        [Description("Gets or sets the second progress color")]
        [NotifyParentProperty(true)]
        [DefaultValue(typeof(Color), "WhiteSmoke")]
        public Color BarProgressColorEnd
        {
            get { return colorArray[9]; }
            set
            {
                if (!value.Equals(colorArray[9]))
                    colorArray[9] = value;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    class TooltipRendererConverter : ExpandableObjectConverter
    {
        #region Destructor

        ~TooltipRendererConverter()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Override Methods

        //All the CanConvertTo() method needs to is check that the target type is a string.
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        }

        //ConvertTo() simply checks that it can indeed convert to the desired type.
        public override object ConvertTo(ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return ToString(value);
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }

        /* The exact same process occurs in reverse when converting a TooltipRenderer object to a string.
        First the Properties window calls CanConvertFrom(). If it returns true, the next step is to call
        the ConvertFrom() method. */
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
                return FromString(value);
            else
                return base.ConvertFrom(context, culture, value);
        }

        #endregion

        #region Helper Methods

        private string ToString(object value)
        {
            TooltipRenderer tRenderer = value as TooltipRenderer;
            TypeConverter converter = new ColorConverter();
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                converter.ConvertToString(tRenderer.BorderColor),
                converter.ConvertToString(tRenderer.TitleForeColor),
                converter.ConvertToString(tRenderer.MessageForeColor),
                converter.ConvertToString(tRenderer.LightBackgroundColor),
                converter.ConvertToString(tRenderer.DarkBackgroundColor),
                tRenderer.Radius,
                converter.ConvertToString(tRenderer.BarBorderColor),
                converter.ConvertToString(tRenderer.BarBackgroundColorStart),
                converter.ConvertToString(tRenderer.BarBackgroundColorEnd),
                converter.ConvertToString(tRenderer.BarProgressColorStart),
                converter.ConvertToString(tRenderer.BarProgressColorEnd));
        }

        private TooltipRenderer FromString(object value)
        {
            string[] result = ((string)value).Split(',');
            if (result.Length != 11)
                throw new ArgumentException("Could not convert to value");
            try
            {
                TooltipRenderer tRenderer = new TooltipRenderer();
                // Retrieve the colors
                TypeConverter converter = new ColorConverter();
                tRenderer.BorderColor = (Color)converter.ConvertFromString(result[0]);
                tRenderer.TitleForeColor = (Color)converter.ConvertFromString(result[1]);
                tRenderer.MessageForeColor = (Color)converter.ConvertFromString(result[2]);
                tRenderer.LightBackgroundColor = (Color)converter.ConvertFromString(result[3]);
                tRenderer.DarkBackgroundColor = (Color)converter.ConvertFromString(result[4]);
                tRenderer.BarBorderColor = (Color)converter.ConvertFromString(result[6]);
                tRenderer.BarBackgroundColorStart = (Color)converter.ConvertFromString(result[7]);
                tRenderer.BarBackgroundColorEnd = (Color)converter.ConvertFromString(result[8]);
                tRenderer.BarProgressColorStart = (Color)converter.ConvertFromString(result[9]);
                tRenderer.BarProgressColorEnd = (Color)converter.ConvertFromString(result[10]);
                converter = new ByteConverter();
                tRenderer.Radius = (byte)converter.ConvertFromString(result[5]);
                return tRenderer;
            }
            catch (Exception)
            {
                throw new ArgumentException("Could not convert to value");
            }
        }

        #endregion
    }

    class TooltipRendererEditor : UITypeEditor
    {
        #region Destructor

        ~TooltipRendererEditor()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Override Methods

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            TooltipRenderer gradient = e.Value as TooltipRenderer;
            using (Brush brush = new LinearGradientBrush(Point.Empty,
                new Point(0, e.Bounds.Height),
                gradient.DarkBackgroundColor,
                gradient.LightBackgroundColor))
                e.Graphics.FillRectangle(brush, e.Bounds);
        }

        #endregion
    }
}