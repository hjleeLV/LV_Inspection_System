using System;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace NeoTabControlLibrary
{
    public class ToolTips : Form
    {
        #region API

        [DllImport("gdi32")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32")]
        private static extern bool DeleteDC(IntPtr dc);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int ReleaseDC(
            IntPtr hwnd,
            IntPtr hdc);

        [DllImport("user32", ExactSpelling = true, SetLastError = true)]
        private static extern bool UpdateLayeredWindow(IntPtr hwnd,
            IntPtr hdcDst,
            ref Point pptDst,
            ref Size psize,
            IntPtr hdcSrc,
            ref Point pprSrc,
            Int32 crKey,
            ref BLENDFUNCTION pblend,
            Int32 dwFlags);

        #endregion

        #region Struct

        [StructLayout(LayoutKind.Sequential)]
        struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        };

        #endregion

        #region Event

        /// <summary>
        /// Event raised when the tooltip operation is succesfully completed.
        /// </summary>
        public event EventHandler TaskCompleted;

        #endregion

        #region Delegate

        public delegate void UIThreadPainterDelegate(Bitmap bmp, byte transparency);

        #endregion

        #region Enums

        public enum StatusState
        {
            Pending, InProgress, Completed
        };

        enum WindowExStyles
        {
            GWL_STYLE = -16,
            GWL_EXSTYLE = (-20),    //GetWindowLong
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_RIGHT = 0x00001000,   //Gives a window generic right-aligned properties.This depends on the window class.
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,  //Displays the window text using right-to-left reading order properties.
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,   //Places a vertical scroll bar to the left of the client area.
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_OVERLAPPEDWINDOW = 0x00000300,
            WS_EX_PALETTEWINDOW = 0x00000188,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_NOACTIVATE = 0x08000000
        };

        #endregion

        #region Symbolic Constants

        // STEP_COUNT, STEP_INTERVAL, CHECK_INTERVAL
        private static readonly int[] THREAD_ARRAY = { 9, 20, 1 };
        
        // SHADOW_DEPTH, BORDER_THICKNESS, TOTAL_BORDER_THICKNESS
        private static readonly int[] BORDERS = { 6, 1, 2 };

        private static readonly Size DEFAULT_SIZE = new Size(300 + BORDERS[2] + BORDERS[0],
            65 + BORDERS[2] + BORDERS[0]);
        
        // TITLE_FONT, DESCRIPTION_FONT
        private static readonly Font[] FONTS = { 
            new Font("Tahoma", 9.75f, FontStyle.Bold, GraphicsUnit.Point), 
            new Font("Tahoma", 8.75f, FontStyle.Regular, GraphicsUnit.Point) };

        private const int LWA_ALPHA = 0x00000002;
        private const byte AC_SRC_OVER = 0x00;
        private const byte AC_SRC_ALPHA = 0x01;

        #endregion

        #region Static Members Of The Class

        // TITLE_SIZE, MESSAGE_SIZE
        private static Size[] sizeOfMessages = new Size[2];

        #endregion

        #region Instance Members
        
        // This is the thread where the tooltip task is carried out.
        private Thread thread;
        private Guid id = Guid.NewGuid();

        #endregion

        #region Constructor

        public ToolTips()
        {
            this.SetStyle(ControlStyles.FixedWidth | ControlStyles.FixedHeight, true);
            this.SetStyle(ControlStyles.Selectable | ControlStyles.ResizeRedraw, false);
            this.DoubleBuffered = true;

            this.ShowIcon = false;
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        public ToolTips(string title, string description, int value, int maxvalue)
            : this()
        {
            TITLE = title;
            DESCRIPTION = description;
            VALUE = value;
            MAXVALUE = maxvalue;
        }

        #endregion

        #region Destructor

        ~ToolTips()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Property

        public int VALUE { get; set; }
        public int MAXVALUE { get; set; }
        public string TITLE { get; set; }
        public string DESCRIPTION { get; set; }

        /// <summary>
        /// Gets or sets the tooltip renderer of the tab control.
        /// </summary>
        public TooltipRenderer TooltipRenderer { get; set; }

        /// <summary>
        /// Use a unique ID to track the tooltip task later, if needed.
        /// </summary>
        public Guid ID
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Track the status of the tooltip task.
        /// </summary>
        public StatusState Status { get; private set; }

        /// <summary>
        /// Gets the status of the tooltip thread.
        /// </summary>
        public string TaskLog { get; private set; }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // This form has to have the WS_EX_LAYERED extended style.
                cp.ExStyle |= (int)WindowExStyles.WS_EX_LAYERED;
                // Hide from ALT+Tab menu, Turn on WS_EX_TOOLWINDOW style.
                cp.ExStyle |= (int)WindowExStyles.WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        protected override bool ShowWithoutActivation
        {
            // True if the window will not be activated when it's shown; otherwise, false.
            get { return true; }
        }

        #endregion

        #region Override Methods

        protected override Size DefaultSize
        {
            get
            {
                return DEFAULT_SIZE;
            }
        }

        #endregion

        #region Helper Methods

        private void CalculateSize()
        {
            using (Graphics g = CreateGraphics())
            {
                sizeOfMessages[0] = MeasureString(g, TITLE, FONTS[0], Int32.MaxValue);
                sizeOfMessages[1] = MeasureString(g, DESCRIPTION, FONTS[1], 576 - (2 * BORDERS[1]));
            }
        }

        private Size MeasureString(Graphics g, string val, Font font, int maxWidth)
        {
            SizeF sizeF = g.MeasureString(val, font, maxWidth);
            return new Size(
                (int)sizeF.Width < maxWidth ? (int)sizeF.Width + 1 : maxWidth,
                (int)sizeF.Height + 1);
        }

        private GraphicsPath CreateRoundRect(Rectangle rect, int radius)
        {
            GraphicsPath gp = new GraphicsPath();
            int x = rect.X;
            int y = rect.Y;
            int width = rect.Width;
            int height = rect.Height;
            if (radius > 0)
            {
                radius = Math.Min(radius, height / 2 - 1);
                radius = Math.Min(radius, width / 2 - 1);
                gp.AddLine(x + radius, y, x + width - (radius * 2), y);
                gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90);
                gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2));
                gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
                gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height);
                gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
                gp.AddLine(x, y + height - (radius * 2), x, y + radius);
                gp.AddArc(x, y, radius * 2, radius * 2, 180, 90);
            }
            else
            {
                gp.AddRectangle(rect);
            }
            gp.CloseFigure();
            return gp;
        }

        private void DrawContent(Graphics g)
        {
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            try
            {
                int y = BORDERS[1] + 8;
                int x = BORDERS[1] + 6;
                int x2 = Width - 12 - BORDERS[0] - BORDERS[2];
                using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit))
                {
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;

                    #region Title and ProgressBar
                    if (!String.IsNullOrEmpty(TITLE))
                    {
                        Rectangle titleRect = new Rectangle(
                            x,
                            y,
                            x2 - 55,
                            sizeOfMessages[0].Height);
                        using (Brush titleBrush = new SolidBrush(TooltipRenderer.TitleForeColor))
                            g.DrawString(TITLE, FONTS[0], titleBrush, titleRect, format);

                        #region ProgressBar
                        Rectangle progressRect = new Rectangle(
                            titleRect.Right + 5,
                            y,
                            50,
                            sizeOfMessages[0].Height / 3);
                        using (ProgressBarPainter pPainter =
                            new ProgressBarPainter(TooltipRenderer.BarBorderColor,
                            TooltipRenderer.BarBackgroundColorStart, TooltipRenderer.BarBackgroundColorEnd,
                            TooltipRenderer.BarProgressColorStart, TooltipRenderer.BarProgressColorEnd))
                        {
                            pPainter.VALUE = VALUE;
                            pPainter.MAXVALUE = MAXVALUE;
                            pPainter.PaintProgressBar(g, progressRect);
                        }
                        y = titleRect.Bottom + 1;
                        #endregion
                    }
                    #endregion

                    #region Message
                    if (!String.IsNullOrEmpty(DESCRIPTION))
                    {
                        Rectangle messageRect = new Rectangle(
                            x,
                            y + 4,
                            x2,
                            sizeOfMessages[1].Height);
                        using (Brush messageBrush = new SolidBrush(TooltipRenderer.MessageForeColor))
                            g.DrawString(DESCRIPTION, FONTS[1], messageBrush, messageRect, format);
                    }
                    #endregion
                }
            }
            catch { ;}
        }

        private void DrawShadow(Graphics g)
        {
            if (BORDERS[0] > 0)
            {
                Rectangle shadowRect = new Rectangle(
                    BORDERS[0],
                    BORDERS[0],
                    Width - BORDERS[0],
                    Height - BORDERS[0]);
                if (shadowRect.Width > 0 && shadowRect.Height > 0)
                {
                    using (GraphicsPath shadowPath = CreateRoundRect(shadowRect, TooltipRenderer.Radius))
                    {
                        using (PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath))
                        {
                            Color[] colors = new Color[4];
                            float[] positions = new float[4];
                            ColorBlend sBlend = new ColorBlend();
                            colors[0] = Color.FromArgb(0, 0, 0, 0);
                            colors[1] = Color.FromArgb(32, 0, 0, 0);
                            colors[2] = Color.FromArgb(64, 0, 0, 0);
                            colors[3] = Color.FromArgb(128, 0, 0, 0);
                            positions[0] = 0.0f;
                            positions[1] = 0.015f;
                            positions[2] = 0.030f;
                            positions[3] = 1.0f;
                            sBlend.Colors = colors;
                            sBlend.Positions = positions;
                            shadowBrush.InterpolationColors = sBlend;
                            shadowBrush.CenterPoint = new Point(
                                shadowRect.Left + (shadowRect.Width / 2),
                                shadowRect.Top + (shadowRect.Height / 2));
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }
                }
            }
        }

        private void DrawBackground(Graphics g)
        {
            Rectangle rct = new Rectangle(0, 0,
                Width - BORDERS[0] - BORDERS[1],
                Height - BORDERS[0] - BORDERS[1]);
            if (rct.Width > 0 && rct.Height > 0)
            {
                using (GraphicsPath path = CreateRoundRect(rct, TooltipRenderer.Radius))
                {
                    using (Brush brush = new LinearGradientBrush(
                        rct, TooltipRenderer.DarkBackgroundColor, TooltipRenderer.LightBackgroundColor,
                        LinearGradientMode.Vertical))
                        g.FillPath(brush, path);
                    using (Pen pen = new Pen(TooltipRenderer.BorderColor))
                        g.DrawPath(pen, path);
                }
            }
        }

        /// <summary>
        /// Creates a new transparent layered window.
        /// </summary>
        /// <param name="bmp">To be layered bitmap</param>
        /// <param name="transparency">Alpha value</param>
        private void GDIWindow(Bitmap bmp, byte transparency)
        {
            IntPtr hDC = GetDC(IntPtr.Zero);
            try
            {
                IntPtr hMemDC = CreateCompatibleDC(hDC);
                try
                {
                    IntPtr hBmp = bmp.GetHbitmap(Color.FromArgb(0));
                    try
                    {
                        IntPtr previousBmp = SelectObject(hMemDC, hBmp);
                        try
                        {
                            Point ptDst = new Point(Left, Top);
                            Size size = new Size(bmp.Width, bmp.Height);
                            Point ptSrc = new Point(0, 0);
                            BLENDFUNCTION blend = new BLENDFUNCTION();
                            blend.BlendOp = AC_SRC_OVER;
                            blend.BlendFlags = 0;
                            blend.SourceConstantAlpha = transparency;
                            blend.AlphaFormat = AC_SRC_ALPHA;
                            UpdateLayeredWindow(
                                Handle,
                                hDC,
                                ref ptDst,
                                ref size,
                                hMemDC,
                                ref ptSrc,
                                0,
                                ref blend,
                                LWA_ALPHA);
                        }
                        finally
                        {
                            SelectObject(hDC, previousBmp);
                        }
                    }
                    finally
                    {
                        DeleteObject(hBmp);
                    }
                }
                finally
                {
                    DeleteDC(hMemDC);
                }
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, hDC);
            }
        }

        /// <summary>
        /// Tooltip worker method.
        /// </summary>
        private void DoTask()
        {
            Size size = DefaultSize;
            using (Bitmap overlay = new Bitmap(size.Width, size.Height))
            {
                using (Graphics g = Graphics.FromImage(overlay))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    DrawShadow(g);
                    DrawBackground(g);
                    DrawContent(g);
                }
                /* Initial alpha value is 25. */
                byte transparency = 25;
                for (int i = 1; i <= THREAD_ARRAY[0]; i++)
                {
                    Invoke(new UIThreadPainterDelegate(GDIWindow), 
                        new object[] { overlay, transparency });
                    transparency += 25;
                    // Pause specified milliseconds before the next pass.
                    Thread.Sleep(TimeSpan.FromMilliseconds(THREAD_ARRAY[1]));
                }
                DateTime startTime = DateTime.Now;
                do
                {
                    /* Still waiting for 3 seconds time limit to pass. */
                    Thread.Sleep(TimeSpan.FromSeconds(THREAD_ARRAY[2]));
                }
                while (DateTime.Now.Subtract(startTime).TotalSeconds < 3);
                transparency = 200;
                for (int i = 1; i <= THREAD_ARRAY[0] - 1; i++)
                {
                    Invoke(new UIThreadPainterDelegate(GDIWindow), 
                        new object[] { overlay, transparency });
                    transparency -= 25;
                    // Pause specified milliseconds before the next pass.
                    Thread.Sleep(TimeSpan.FromMilliseconds(THREAD_ARRAY[1]));
                }
            }
        }

        /// <summary>
        /// Tooltip thread method.
        /// </summary>
        private void StartTaskAsync()
        {
            try
            {
                DoTask();
                Status = StatusState.Completed;
                Invoke(new EventHandler(OnTaskCompleted),
                    new object[] { this, EventArgs.Empty });
                // Write log message.
                TaskLog = String.Format("Tooltip task completed !!!, {0} is completed succesfully at {1:F}",
                    Thread.CurrentThread.Name, DateTime.Now);
            }
            catch (ThreadAbortException)
            {
                // Write log message.
                TaskLog = String.Format("Tooltip task aborted !!!, {0} is destroyed and stopped at {1:F}",
                    Thread.CurrentThread.Name, DateTime.Now);
            }
        }

        private void OnTaskCompleted(object sender, EventArgs e)
        {
            // Signal that the tooltip operation is completed.
            if (TaskCompleted != null)
                TaskCompleted(this, EventArgs.Empty);
        }

        #endregion

        #region General Methods

        /// <summary>
        /// Stops the currently running tooltip task.
        /// </summary>
        /// <param name="cancelWaitTime">
        /// How long the thread will wait before aborting a tooltip thread that hasn't responded to the cancellation message.
        /// TimeSpan.Zero means polite stops are not enabled.
        /// </param>
        public void StopAsyncTooltip(TimeSpan cancelWaitTime)
        {
            // Perform no operation if tooltip task isn't running.
            if (Status != StatusState.InProgress)
                return;
            // Try the polite approach.
            if (cancelWaitTime != TimeSpan.Zero)
            {
                DateTime startTime = DateTime.Now;
                // Write log message.
                TaskLog = String.Format("Tooltip task cancellation pending request !!!, {0} is aborting now {1:F}", 
                    thread.Name, startTime);
                do
                {
                    /* Still waiting for the time limit to pass.
                       Allow other threads to do some work. */
                    Thread.Sleep(TimeSpan.FromSeconds(THREAD_ARRAY[2]));
                }
                while (DateTime.Now.Subtract(startTime).TotalSeconds < cancelWaitTime.Seconds);
            }
            if (thread.IsAlive)
            {
                // Tell the task thread to abort itself immediately, raises a ThreadAbortException in the task thread after calling the Thread.Join() method.
                thread.Abort();
                // Wait for the tooltip thread to finish.
                thread.Join();
            }
        }

        /// <summary>
        /// Starts a new tooltip thread in background.
        /// </summary>
        public void StartAsyncTooltip()
        {
            if (Status == StatusState.InProgress)
                throw new InvalidOperationException("Already in progress...");
            else
            {
                // Calculate message sizes.
                CalculateSize();
                // Initialize the new task.
                Status = StatusState.InProgress;
                /* Create the thread and run it in the background so
                   it will terminate automatically if the application ends. */
                thread = new Thread(StartTaskAsync);
                thread.Name = String.Format("Tooltip thread({0})", ID);
                thread.IsBackground = true;
                // Start the thread.
                thread.Start();
                // Write log message.
                TaskLog = String.Format("Tooltip task started !!!, {0} is started at {1:F}", 
                    thread.Name, DateTime.Now);
            }
        }

        #endregion
    }
}