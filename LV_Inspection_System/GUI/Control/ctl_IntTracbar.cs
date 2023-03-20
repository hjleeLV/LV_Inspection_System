using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSV.GUI.CONTROL.COMPONENT
{
    public class TrackBarEx : TrackBar
    {
        private const int WM_SETFOCUS = 0x0007;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SETFOCUS)
            {
                return;
            }

            base.WndProc(ref m);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private static int MakeParam(int loWord, int hiWord)
        {
            return (hiWord << 16) | (loWord & 0xffff);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            SendMessage(this.Handle, 0x0128, MakeParam(1, 0x1), 0);
        }
    }

    public partial class ctl_IntTracbar : TrackBarEx
    {
        private System.ComponentModel.IContainer components = null;
        private bool ShowValue = true;
        public ctl_IntTracbar() : base()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private int MarkWidth = 8;      // Width of mark knobs
        private Color colorShadowDark = Color.FromKnownColor(KnownColor.ControlDarkDark);
        private float precision = 1f;
        public float Precision
        {
            get { return precision; }
            set
            {
                precision = value;
                // todo: update the 5 properties below
            }
        }
        public new float LargeChange
        { get { return base.LargeChange * precision; } set { base.LargeChange = (int)(value / precision); } }
        public new float Maximum
        { get { return base.Maximum * precision; } set { base.Maximum = (int)(value / precision); } }
        public new float Minimum
        { get { return base.Minimum * precision; } set { base.Minimum = (int)(value / precision); } }
        public new float SmallChange
        { get { return base.SmallChange * precision; } set { base.SmallChange = (int)(value / precision); } }
        public new float Value
        {
            get { return (float)Math.Round(base.Value * precision, 2); }
            set
            {
                try
                { base.Value = (int)(value / precision); }
                catch
                { }
            }
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();

            this.Name = "IntBar";
            this.Size = new System.Drawing.Size(241, 45);
            this.ValueChanged += new System.EventHandler(this.OnValueChanged);
            this.GotFocus += new System.EventHandler(this.OnGotFocus);
            this.LostFocus += new System.EventHandler(this.OnLostFocus);
            this.MouseHover += new System.EventHandler(this.OnMouseHover);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            this.TabStop = false;
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);

                base.SetStyle(ControlStyles.UserPaint, false);

                base.Refresh();                      // Use default paint implementation.

                if (ShowValue)
                {
                    // Adding additional paint code.
                    Point pctTxtPoint;
                    pctTxtPoint = new Point((int)((float)(this.Width - 24) * (Value - Minimum) / (Maximum - Minimum)), 26);

                    Font fontMark = new Font("Arial", MarkWidth);
                    SolidBrush brushMark = new SolidBrush(colorShadowDark);
                    StringFormat strformat = new StringFormat();
                    strformat.Alignment = StringAlignment.Center;
                    strformat.LineAlignment = StringAlignment.Near;

                    e.Graphics.DrawString(Value.ToString(), fontMark, brushMark, pctTxtPoint.X + 10, pctTxtPoint.Y, strformat);
                }
                base.SetStyle(ControlStyles.UserPaint, true);
            }
            catch
            { }
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            ShowValue = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, false);
            OnValueChanged(sender, e);
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            ShowValue = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }
        private void OnMouseHover(object sender, System.EventArgs e)
        {
            ShowValue = false;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, false);
            OnValueChanged(sender, e);
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            this.Invalidate(); //컨트롤 초기화
            this.Update();
            using (var grp = this.CreateGraphics()) //Graphics
            {
                Point pctTxtPoint;
                pctTxtPoint = new Point((int)((float)(this.Width - 24) * (Value - Minimum) / (Maximum - Minimum)), 26);

                Font fontMark = new Font("Arial", MarkWidth);
                SolidBrush brushMark = new SolidBrush(colorShadowDark);
                StringFormat strformat = new StringFormat();
                strformat.Alignment = StringAlignment.Center;
                strformat.LineAlignment = StringAlignment.Near;

                grp.DrawString(Value.ToString(), fontMark, brushMark, pctTxtPoint.X + 10, pctTxtPoint.Y, strformat);
            }
            this.Update();
        }
    }
}
