using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NeoTabControlLibrary
{
    public class ProgressBarPainter : IDisposable
    {
        #region Struct

        struct ColorManipulation
        {
            #region Instance Members

            public Color Dark;
            public Color Light;
            public Color Darker;
            public Color Lighter;
            public Color BaseColor;

            #endregion

            #region Constructor

            public ColorManipulation(Color baseColor)
            {
                this.Dark = Shade(0.8f, baseColor);
                this.Light = Smooth(0.6f, baseColor);
                this.Darker = Shade(0.6f, baseColor);
                this.Lighter = Smooth(0.3f, baseColor);
                this.BaseColor = baseColor;
            }

            #endregion

            #region Helper Methods

            private static Color Shade(float ratio, Color baseColor)
            {
                return Manipule(ratio, baseColor, Color.Black);
            }

            private static Color Smooth(float ratio, Color baseColor)
            {
                return Manipule(ratio, baseColor, Color.White);
            }

            private static Color Manipule(float ratio, Color baseColor, Color manipuleColor)
            {
                int r = (int)(manipuleColor.R + ratio * (baseColor.R - manipuleColor.R));// Red
                int g = (int)(manipuleColor.G + ratio * (baseColor.G - manipuleColor.G));// Green
                int b = (int)(manipuleColor.B + ratio * (baseColor.B - manipuleColor.B));// Blue
                return Color.FromArgb(r, g, b);
            }

            #endregion
        };

        #endregion
        
        #region Instance Members

        private Color[] COLORS = new Color[3];
        private ColorManipulation[] progressColors = null;

        #endregion

        #region Constructor

        public ProgressBarPainter(Color borderColor, Color backColorA, Color backColorB,
            Color progColorA, Color progColorB)
        {
            COLORS[0] = borderColor;
            COLORS[1] = backColorA;
            COLORS[2] = backColorB;
            progressColors = new ColorManipulation[] {
                new ColorManipulation(progColorA), new ColorManipulation(progColorB) };
        }

        #endregion

        #region Property

        public int VALUE { get; set; }
        public int MAXVALUE { get; set; }
        public Rectangle BarRectangle { get; set; }

        #endregion

        #region Helper Methods

        private void DrawBorder(Graphics gr)
        {
            Pen pen = new Pen(COLORS[0]);
            gr.DrawRectangle(pen, BarRectangle);
            pen.Dispose();
        }

        private void DrawBackground(Graphics gr)
        {
            Rectangle backgroundRectangle = BarRectangle;
            backgroundRectangle.Inflate(-1, -1);
            if (backgroundRectangle.Width < 2)
                return;
            using (LinearGradientBrush brush = new LinearGradientBrush(Point.Empty, new Point(0, backgroundRectangle.Height),
                COLORS[1], COLORS[2]))
            {
                Blend bl = new Blend(2);
                bl.Factors = new float[] { 0.3F, 1.0F };
                bl.Positions = new float[] { 0.0F, 1.0F };
                brush.Blend = bl;
                gr.FillRectangle(brush, backgroundRectangle);
            }
        }

        private void DrawProgress(Graphics gr)
        {
            Rectangle progressRectangle = BarRectangle;
            progressRectangle.Inflate(-1, -1);
            progressRectangle.Width = VALUE >= MAXVALUE ? progressRectangle.Width : progressRectangle.Width * VALUE / MAXVALUE;
            if (progressRectangle.Width < 2)
                return;
            Point left = new Point(progressRectangle.X, progressRectangle.Y);
            Point right = new Point(progressRectangle.Right, progressRectangle.Y);
            using (LinearGradientBrush brush = new LinearGradientBrush(Point.Empty, new Point(0, progressRectangle.Height),
                progressColors[0].BaseColor, progressColors[1].BaseColor))
            {
                Blend bl = new Blend(2);
                bl.Factors = new float[] { 0.3F, 1.0F };
                bl.Positions = new float[] { 0.0F, 1.0F };
                brush.Blend = bl;
                gr.FillRectangle(brush, progressRectangle);
            }
            LinearGradientBrush topInner = new LinearGradientBrush(left, right,
                progressColors[0].Light, progressColors[1].Light);
            LinearGradientBrush topOuter = new LinearGradientBrush(left, right,
                progressColors[0].Lighter, progressColors[1].Lighter);
            LinearGradientBrush bottomInner = new LinearGradientBrush(left, right,
                progressColors[0].Dark, progressColors[1].Dark);
            LinearGradientBrush bottomOuter = new LinearGradientBrush(left, right,
                progressColors[0].Darker, progressColors[1].Darker);
            // Inner Top
            using (Pen pen = new Pen(topInner))
            {
                gr.DrawLine(pen, progressRectangle.X + 1, progressRectangle.Y + 1, 
                    progressRectangle.Right - 1, progressRectangle.Y + 1);
            }
            // Inner Left
            using (Pen pen = new Pen(progressColors[0].Light))
            {
                gr.DrawLine(pen, progressRectangle.X + 1, progressRectangle.Y + 1, 
                    progressRectangle.X + 1, progressRectangle.Bottom - 1);
            }
            // Outer Top
            using (Pen pen = new Pen(topOuter))
            {
                gr.DrawLine(pen, progressRectangle.X, progressRectangle.Y, 
                    progressRectangle.Right, progressRectangle.Y);
            }
            // Outer Left
            using (Pen pen = new Pen(progressColors[0].Lighter))
            {
                gr.DrawLine(pen, progressRectangle.X, progressRectangle.Y, 
                    progressRectangle.X, progressRectangle.Bottom);
            }
            // Inner Bottom
            using (Pen pen = new Pen(bottomInner))
            {
                gr.DrawLine(pen, progressRectangle.X + 1, progressRectangle.Bottom - 1, 
                    progressRectangle.Right - 1, progressRectangle.Bottom - 1);
            }
            // Inner Right
            using (Pen pen = new Pen(progressColors[1].Dark))
            {
                gr.DrawLine(pen, progressRectangle.Right - 1, progressRectangle.Y + 1, 
                    progressRectangle.Right - 1, progressRectangle.Bottom - 1);
            }
            // Outer Bottom
            using (Pen pen = new Pen(bottomOuter))
            {
                gr.DrawLine(pen, progressRectangle.X, progressRectangle.Bottom, 
                    progressRectangle.Right, progressRectangle.Bottom);
            }
            // Outer Right
            using (Pen pen = new Pen(progressColors[1].Darker))
            {
                gr.DrawLine(pen, progressRectangle.Right, progressRectangle.Y, 
                    progressRectangle.Right, progressRectangle.Bottom);
            }
            topInner.Dispose();
            topOuter.Dispose();
            bottomInner.Dispose();
            bottomOuter.Dispose();
        }

        #endregion

        #region General Methods

        public void PaintProgressBar(Graphics g, Rectangle rct)
        {
            BarRectangle = rct;
            DrawBorder(g);
            DrawBackground(g);
            DrawProgress(g);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}