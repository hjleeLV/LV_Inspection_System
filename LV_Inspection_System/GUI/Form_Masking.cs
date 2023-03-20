using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LV_Inspection_System.GUI
{
    public partial class Form_Masking : Form
    {
        public int Cam_num = 0;
        public int Roi_Idx = 0;
        public Rectangle t_ROI;

        public string Save_Path = string.Empty;
        public Bitmap CAM_Image = null;
        public Bitmap Ori_Image = null;
        public Bitmap Mask_Image = null;
        public Bitmap Disp_Image = null;
        private Mat Mat_Ori_Image = null;
        private Mat Mat_Mask_Image = null;
        private Mat Mat_Disp_Image = null;

        private MODE _MODE = MODE.UNDO;
        private SHAPE _SHAPE = SHAPE.RECT;
        private VIEW _VIEW = VIEW.MERGE;
        private int m_Size = 10;

        enum MODE { MASK, UNDO };
        enum SHAPE { RECT, FREE };
        enum VIEW { ORI, MASK, MERGE };


        System.Drawing.Point lastPoint = System.Drawing.Point.Empty;
        bool isMouseDown = false;
        bool isMouseUp = false;

        public Form_Masking()
        {
            InitializeComponent();

        }

        public Bitmap _CAM_Image
        {
            get
            {
                return CAM_Image;
            }
            set
            {
                CAM_Image = value;
                Ori_Image = cropAtRect(CAM_Image, t_ROI);
                Mask_Image = Ori_Image.Clone() as Bitmap;
                using (Graphics g = Graphics.FromImage(Mask_Image)) { g.Clear(Color.FromArgb(0, 255, 0)); }
                //pictureBox1.Image = Ori_Image.Clone() as Bitmap;
                imageBox1.Image = Ori_Image.Clone() as Bitmap;
                Load_Mask_Image();
                imageBox1.SizeMode = Cyotek.Windows.Forms.ImageBoxSizeMode.Fit;
                imageBox1.SizeMode = Cyotek.Windows.Forms.ImageBoxSizeMode.Normal;
                imageBox1.Cursor = CreateCursor();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                //pictureBox1.Refresh();
            }
        }

        private Bitmap cropAtRect(Bitmap source, Rectangle section)
        {
            try
            {
                Bitmap bmp = new Bitmap(section.Width, section.Height, PixelFormat.Format24bppRgb);
                bmp.SetResolution(source.HorizontalResolution, source.VerticalResolution);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    // Draw the given area (section) of the source image
                    // at location 0,0 on the empty bitmap (bmp)
                    g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                    return bmp;
                }
            }
            catch
            {
               
            }
            return null;
        }

        private static Cursor CreateCursor()
        {
            Bitmap bm = new Bitmap(16, 16);
            bm.MakeTransparent();
            return new Cursor(bm.GetHicon());
        }

        private void Save_Mask_Image()
        {
            if (Mat_Mask_Image.Empty())
            {
                return;
            }
            Mat[] planes;
            Cv2.Split(Mat_Mask_Image, out planes);
            Cv2.InRange(planes[2], 1, 255, planes[2]);

            Mat C_Mat_Mask_Image = new Mat(new OpenCvSharp.Size(CAM_Image.Width, CAM_Image.Height), MatType.CV_8UC1, Scalar.All(0));
            Rect tt_ROI = new Rect(t_ROI.X, t_ROI.Y, t_ROI.Width, t_ROI.Height);
            C_Mat_Mask_Image[tt_ROI] = planes[2].Clone();

            if (Save_Path.Length > 0 && planes.Length > 1)
            {
                C_Mat_Mask_Image.SaveImage(Save_Path);
                Add_Message("Mask saved.");
            }
        }


        private void Load_Mask_Image()
        {
            if (File.Exists(Save_Path))
            {
                using (Mat t_load = Cv2.ImRead(Save_Path, ImreadModes.Grayscale))
                {
                    Rect tt_ROI = new Rect(t_ROI.X, t_ROI.Y, t_ROI.Width, t_ROI.Height);
                    Mat_Mask_Image = t_load[tt_ROI].Clone();
                    if (Ori_Image.Width != Mat_Mask_Image.Width || Ori_Image.Height != Mat_Mask_Image.Height)
                    {
                        Mat_Mask_Image = new Mat(new OpenCvSharp.Size(Ori_Image.Width, Ori_Image.Height), MatType.CV_8UC1, Scalar.All(0));
                    }
                    Mat C_Mat_Mask_Image = new Mat(Mat_Mask_Image.Size(), MatType.CV_8UC3, Scalar.All(0));
                    Mat[] planes;
                    Cv2.Split(C_Mat_Mask_Image, out planes);
                    Cv2.Threshold(Mat_Mask_Image, planes[1], 1, 255, ThresholdTypes.BinaryInv);
                    planes[2] = Mat_Mask_Image.Clone();
                    Mat merged = new Mat();
                    Cv2.Merge(planes, merged);
                    Mask_Image = null;
                    Mask_Image = BitmapConverter.ToBitmap(merged.Clone());
                    Add_Message("Mask Loaded.");
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                _MODE = MODE.MASK;
                imageBox1.Invalidate();
            }
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                _MODE = MODE.UNDO;
                imageBox1.Invalidate();
            }
        }

        private void RadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                _SHAPE = SHAPE.RECT;
                imageBox1.Invalidate();
            }
        }

        private void RadioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                _SHAPE = SHAPE.FREE;
                int.TryParse(textBox_Size.Text, out m_Size);
                imageBox1.Invalidate();
            }
        }

        private void RadioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                _VIEW = VIEW.ORI;
                imageBox1.Invalidate();
            }
        }

        private void RadioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                _VIEW = VIEW.MASK;
                imageBox1.Invalidate();
            }
        }

        private void RadioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                _VIEW = VIEW.MERGE;
                imageBox1.Invalidate();
            }
        }

        private void TextBox_Size_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBox_Size.Text, out m_Size);
            imageBox1.Invalidate();
        }

        protected System.Drawing.Point TranslateZoomMousePosition(System.Drawing.Point coordinates)
        {
            // test to make sure our image is not null
            if (imageBox1.Image == null) return coordinates;
            // Make sure our control width and height are not 0 and our 
            // image width and height are not 0
            int t_Width = imageBox1.Width;
            int t_Height = imageBox1.Height;
            if (t_Width == 0 || t_Height == 0 || imageBox1.Image.Width == 0 || imageBox1.Image.Height == 0) return coordinates;
            // This is the one that gets a little tricky. Essentially, need to check 
            // the aspect ratio of the image to the aspect ratio of the control
            // to determine how it is being rendered
            float imageAspect = (float)imageBox1.Image.Width / imageBox1.Image.Height;
            float controlAspect = (float)t_Width / t_Height;
            float newX = coordinates.X;
            float newY = coordinates.Y;
            if (imageAspect > controlAspect)
            {
                // This means that we are limited by width, 
                // meaning the image fills up the entire control from left to right
                float ratioWidth = (float)imageBox1.Image.Width / t_Width;
                newX *= ratioWidth;
                float scale = (float)t_Width / imageBox1.Image.Width;
                float displayHeight = scale * imageBox1.Image.Height;
                float diffHeight = t_Height - displayHeight;
                diffHeight /= 2;
                newY -= diffHeight;
                newY /= scale;
            }
            else
            {
                // This means that we are limited by height, 
                // meaning the image fills up the entire control from top to bottom
                float ratioHeight = (float)imageBox1.Image.Height / t_Height;
                newY *= ratioHeight;
                float scale = (float)t_Height / imageBox1.Image.Height;
                float displayWidth = scale * imageBox1.Image.Width;
                float diffWidth = t_Width - displayWidth;
                diffWidth /= 2;
                newX -= diffWidth;
                newX /= scale;
            }
            return new System.Drawing.Point((int)newX, (int)newY);
        }

        private void Merge_Processing()
        {
            try
            {
                Mat_Ori_Image = BitmapConverter.ToMat(Ori_Image.Clone() as Bitmap);
                Mat_Mask_Image = BitmapConverter.ToMat(Mask_Image.Clone() as Bitmap);
                //Mat_Ori_Image.CopyTo(Mat_Disp_Image);
                Mat_Disp_Image = null;
                Mat_Disp_Image = new Mat(Mat_Ori_Image.Size(), MatType.CV_8UC3);
                Cv2.BitwiseAnd(Mat_Ori_Image, Mat_Mask_Image, Mat_Disp_Image);
                Disp_Image = null;
                Disp_Image = BitmapConverter.ToBitmap(Mat_Disp_Image.Clone());
            }
            catch
            { }
        }

        bool t_running_flag = false;
        private void Button_SAVE_Click(object sender, EventArgs e)
        {
            try
            {
                if (t_running_flag)
                {
                    return;
                }
                t_running_flag = true;
                Merge_Processing();

                Save_Mask_Image();

                richTextBox1.ResetText();
                Add_Message("[Save] " + Save_Path);
                t_running_flag = false;
            }
            catch
            {
                t_running_flag = false;
            }
        }

        public void Add_Message(String msg)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke((MethodInvoker)delegate
                {
                    if (richTextBox1.Lines.Length > 11)
                    {
                        richTextBox1.ResetText();
                    }
                    string display_str = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + msg + "\r\n" + richTextBox1.Text;
                    richTextBox1.Text = display_str;
                });
            }
            else
            {
                if (richTextBox1.Lines.Length > 11)
                {
                    richTextBox1.ResetText();
                }
                string display_str = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + msg + "\r\n" + richTextBox1.Text;
                richTextBox1.Text = display_str;
            }
        }

        private void ImageBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
                isMouseUp = true;
                return;
            }
            radioButton7.Checked = true;
            lastPoint = e.Location;
            isMouseDown = true;
            isMouseUp = false;
            Draw_Shape2(e.Location);
        }

        System.Drawing.Point _PT;
        private void ImageBox1_MouseMove(object sender, MouseEventArgs e)
        {
            _PT = e.Location;
            if (isMouseDown == true)
            {
                Draw_Shape2(e.Location);
            }
            else
            {
                imageBox1.Invalidate();
            }
        }

        private void ImageBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseUp = true;
            Draw_Shape2(e.Location);
            isMouseDown = false;
            lastPoint = System.Drawing.Point.Empty;
            PT_List.Clear();
        }

        void DrawCross(Graphics g, System.Drawing.Point point)
        {
            Pen crossPen = new Pen(Color.White, 1);
            g.DrawLine(crossPen, new System.Drawing.Point(0, point.Y), new System.Drawing.Point(imageBox1.Width, point.Y));
            g.DrawLine(crossPen, new System.Drawing.Point(point.X, 0), new System.Drawing.Point(point.X, imageBox1.Height));
            crossPen.Dispose();

            if (_SHAPE == SHAPE.FREE)
            {
                int t_Size = (int)((float)m_Size * imageBox1.ZoomFactor);
                if (_MODE == MODE.MASK)
                {
                    Pen FreePen = new Pen(Color.SkyBlue, 1);
                    g.DrawEllipse(FreePen, point.X - t_Size / 2, point.Y - t_Size / 2, t_Size, t_Size);
                    FreePen.Dispose();
                }
                else if (_MODE == MODE.UNDO)
                {
                    Pen FreePen = new Pen(Color.OrangeRed, 1);
                    g.DrawEllipse(FreePen, point.X - t_Size / 2, point.Y - t_Size / 2, t_Size, t_Size);
                    FreePen.Dispose();
                }
            }
            else if (_SHAPE == SHAPE.RECT)
            {
                if (isMouseDown == true)//check to see if the mouse button is down
                {
                    Rectangle t_R = new Rectangle(Math.Min(lastPoint.X, point.X), Math.Min(lastPoint.Y, point.Y), Math.Abs(lastPoint.X - point.X), Math.Abs(lastPoint.Y - point.Y));

                    if (_MODE == MODE.MASK)
                    {
                        g.DrawRectangle(new Pen(Color.SkyBlue, 1), t_R);
                    }
                    else if (_MODE == MODE.UNDO)
                    {
                        g.DrawRectangle(new Pen(Color.OrangeRed, 1), t_R);
                    }
                }
            }
        }

        private List<System.Drawing.Point> PT_List = new List<System.Drawing.Point>();
        Graphics g;
        private void Draw_Shape2(System.Drawing.Point PT)
        {
            if (g == null)
            {
                g = Graphics.FromImage(Mask_Image);
            }
            //if (isMouseDown == true)//check to see if the mouse button is down
            {
                if (_SHAPE == SHAPE.FREE && isMouseDown == true)
                {
                    var location = imageBox1.PointToImage(PT);
                    //var location = TranslateZoomMousePosition(PT);
                    int t_Size = m_Size;//(int)((float)m_Size * imageBox1.ZoomFactor);
                    //using (Graphics g = Graphics.FromImage(Mask_Image))
                    {//we need to create a Graphics object to draw on the picture box, its our main tool
                        //when making a Pen object, you can just give it color only or give it color and pen size
                        //SolidBrush redBrush = new SolidBrush(Color.Red);
                        if (PT == lastPoint)
                        {
                            if (_MODE == MODE.MASK)
                            {
                                g.FillEllipse(new SolidBrush(Color.FromArgb(0, 255, 0)), location.X - t_Size / 2, location.Y - t_Size / 2, t_Size, t_Size);
                            }
                            else if (_MODE == MODE.UNDO)
                            {
                                g.FillEllipse(new SolidBrush(Color.Red), location.X - t_Size / 2, location.Y - t_Size / 2, t_Size, t_Size);
                            }
                            PT_List.Add(imageBox1.PointToImage(PT));
                        }
                        else
                        {
                            var t_lastPoint = imageBox1.PointToImage(lastPoint);
                            var t_CurrPoint = imageBox1.PointToImage(PT);

                            PT_List.Add(t_CurrPoint);

                            if (_MODE == MODE.MASK)
                            {
                                //g.DrawLine(new Pen(Color.FromArgb(0, 255, 0), m_Size), t_lastPoint, t_CurrPoint);
                                var PTs = PT_List.ToArray();
                                if (PTs.Length < 4)
                                {
                                    g.DrawCurve(new Pen(Color.FromArgb(0, 255, 0), t_Size), PTs, 0.1f);
                                }
                                else
                                {
                                    g.DrawBezier(new Pen(Color.FromArgb(0, 255, 0), t_Size), PTs[PTs.Length - 4], PTs[PTs.Length - 3], PTs[PTs.Length - 2], PTs[PTs.Length - 1]);
                                }
                                g.FillEllipse(new SolidBrush(Color.FromArgb(0, 255, 0)), PTs[PTs.Length - 1].X - t_Size / 2, PTs[PTs.Length - 1].Y - t_Size / 2, t_Size, t_Size);
                            }
                            else if (_MODE == MODE.UNDO)
                            {
                                //g.DrawLine(new Pen(Color.Red, m_Size), t_lastPoint, t_CurrPoint);
                                var PTs = PT_List.ToArray();
                                if (PTs.Length < 4)
                                {
                                    g.DrawCurve(new Pen(Color.Red, t_Size), PTs, 0.1f);
                                }
                                else
                                {
                                    g.DrawBezier(new Pen(Color.Red, t_Size), PTs[PTs.Length - 4], PTs[PTs.Length - 3], PTs[PTs.Length - 2], PTs[PTs.Length - 1]);
                                }
                                g.FillEllipse(new SolidBrush(Color.Red), PTs[PTs.Length - 1].X - t_Size / 2, PTs[PTs.Length - 1].Y - t_Size / 2, t_Size, t_Size);
                            }
                            lastPoint = PT;
                        }
                        //g.DrawLine(new Pen(Color.Black, 2), lastPoint, e.Location);
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        //this is to give the drawing a more smoother, less sharper look
                    }

                    imageBox1.Invalidate();//refreshes the picturebox
                }
                else if (_SHAPE == SHAPE.RECT && isMouseUp == true && isMouseDown == true)
                {
                    if (lastPoint != null)//if our last point is not null, which in this case we have assigned above
                    {
                        var t_lastPoint = imageBox1.PointToImage(lastPoint);
                        var t_CurrPoint = imageBox1.PointToImage(PT);
                        Rectangle t_R = new Rectangle(Math.Min(t_lastPoint.X, t_CurrPoint.X), Math.Min(t_lastPoint.Y, t_CurrPoint.Y), Math.Abs(t_lastPoint.X - t_CurrPoint.X), Math.Abs(t_lastPoint.Y - t_CurrPoint.Y));
                        //using (Graphics g = Graphics.FromImage(Mask_Image))
                        {//we need to create a Graphics object to draw on the picture box, its our main tool
                         //when making a Pen object, you can just give it color only or give it color and pen size
                         //SolidBrush redBrush = new SolidBrush(Color.Red);
                            if (_MODE == MODE.MASK)
                            {
                                g.FillRectangle(new SolidBrush(Color.FromArgb(0, 255, 0)), t_R);
                            }
                            else if (_MODE == MODE.UNDO)
                            {
                                g.FillRectangle(new SolidBrush(Color.Red), t_R);
                            }

                            //g.DrawLine(new Pen(Color.Black, 2), lastPoint, e.Location);
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            //this is to give the drawing a more smoother, less sharper look
                        }

                        //}
                        imageBox1.Invalidate();//refreshes the picturebox
                        //lastPoint = e.Location;//keep assigning the lastPoint to the current mouse position
                    }
                }
            }
        }

        //bool Painting_check = false;
        Stopwatch t_SW = new Stopwatch();
        private void ImageBox1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                DrawCross(e.Graphics, _PT);

                //if (Painting_check)
                //{
                //    return;
                //}
                //Painting_check = true;
                //Disp_Image = null;
                if (_VIEW == VIEW.MASK)
                {
                    Disp_Image = Mask_Image.Clone() as Bitmap;
                }
                else if (_VIEW == VIEW.ORI)
                {
                    Disp_Image = Ori_Image.Clone() as Bitmap;
                }
                else if (_VIEW == VIEW.MERGE)
                {
                    Merge_Processing();
                }
                if (Disp_Image == null)
                {
                    return;
                }
                //if (imageBox1.Image == null)
                //{
                //    imageBox1.Image = Disp_Image;
                //}
                imageBox1.Image = null;
                imageBox1.Image = Disp_Image.Clone() as Bitmap;

                if (t_SW.ElapsedMilliseconds == 0)
                {
                    t_SW.Start();
                }
                else if (t_SW.ElapsedMilliseconds >= 500)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    t_SW.Stop();
                    t_SW.Reset();
                    t_SW.Start();
                }
                //Painting_check = false;
            }
            catch
            {
                //Painting_check = false;
            }
        }

        private void ImageBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            imageBox1.SizeMode = Cyotek.Windows.Forms.ImageBoxSizeMode.Fit;
            imageBox1.SizeMode = Cyotek.Windows.Forms.ImageBoxSizeMode.Normal;
        }

        private void Button_CLOSE_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form_Masking_FormClosed(object sender, FormClosedEventArgs e)
        {
            //OSVApp.Instance().MC_PRO.t_Model_Change_Stop_Flag = false;
        }

        private void button_DELETE_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Save_Path))
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    FileInfo f = new FileInfo(Save_Path);
                    f.Delete();

                    richTextBox1.ResetText();
                    Add_Message("[Delete] " + Save_Path);
                }
                else
                {
                    richTextBox1.ResetText();
                    Add_Message("[Delete] Not exist the mask file!");
                }
            }
            catch
            { }
        }
    }
}
