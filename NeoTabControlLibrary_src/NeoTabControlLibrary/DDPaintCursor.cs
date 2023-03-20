using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary
{
    public class DDPaintCursor
    {
        #region API

        [DllImport("gdi32")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        [DllImport("user32", EntryPoint = "CreateIconIndirect")]
        private static extern IntPtr CreateIconIndirect(IntPtr iconInfo);

        #endregion

        #region Struct

        [StructLayout(LayoutKind.Sequential)]
        struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        };

        #endregion
        
        #region Constructor

        public DDPaintCursor() { ;}

        public DDPaintCursor(RendererBase renderer)
		{
            Renderer = renderer;
        }

        #endregion

        #region Destructor

        ~DDPaintCursor()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Property

        public Cursor DDCursor { get; private set; }
        public RendererBase Renderer { get; private set; }

        #endregion

        #region Helper Methods

        private Cursor CreateCursor(Bitmap bmp)
        {
            ICONINFO iInfo = new ICONINFO();
            iInfo.fIcon = false;
            iInfo.xHotspot = 0;
            iInfo.yHotspot = 0;
            iInfo.hbmMask = bmp.GetHbitmap();
            iInfo.hbmColor = bmp.GetHbitmap();
            // Create a new pointer for the drag&drop cursor icon.
            IntPtr pointer = Marshal.AllocHGlobal(Marshal.SizeOf(iInfo));
            Marshal.StructureToPtr(iInfo, pointer, true);
            IntPtr ddpointer = CreateIconIndirect(pointer);
            // Clean Up!!!
            DestroyIcon(pointer);
            DeleteObject(iInfo.hbmMask);
            DeleteObject(iInfo.hbmColor);
            return new Cursor(ddpointer);
        }

        #endregion

        #region General Methods

        public void DrawDDCursor(Bitmap bmp)
        {
            using (Bitmap overlayTransparent = new Bitmap(bmp.Width, bmp.Height,
                PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(overlayTransparent))
                {
                    // Create an ImageAttributes object and set its color matrix
                    using (ImageAttributes attributes = new ImageAttributes())
                    {
                        ColorMatrix colorMatrix = new ColorMatrix();
                        colorMatrix.Matrix33 = 180 / 255f;
                        attributes.SetColorMatrix(colorMatrix);
                        g.DrawImage(bmp, new Rectangle(0, 0, overlayTransparent.Width, overlayTransparent.Height),
                            0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attributes);
                        g.DrawRectangle(Pens.LightGray, 0, 0, overlayTransparent.Width - 1, overlayTransparent.Height - 1);
                    }
                    Cursor overlayCursor = Cursors.Hand;
                    overlayCursor.Draw(g, new Rectangle(Point.Empty, overlayCursor.Size));
                }
                DDCursor = CreateCursor(overlayTransparent);
            }
        }

        public void DrawDDCursor(Rectangle tabPageItemRct, string tabPageText, 
            int index, CommonObjects.ButtonState btnState)
        {
            using (Bitmap overlay = new Bitmap(tabPageItemRct.Width, tabPageItemRct.Height,
                PixelFormat.Format32bppRgb),
                overlayTransparent = new Bitmap(tabPageItemRct.Width, tabPageItemRct.Height,
                PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(overlay))
                {
                    Renderer.OnRendererTabPageItem(g, new Rectangle(0, 0, overlay.Width, overlay.Height),
                        tabPageText, index, btnState);
                }
                using (Graphics g = Graphics.FromImage(overlayTransparent))
                {
                    // Create an ImageAttributes object and set its color matrix
                    using (ImageAttributes attributes = new ImageAttributes())
                    {
                        ColorMatrix colorMatrix = new ColorMatrix();
                        colorMatrix.Matrix33 = 210 / 255f;
                        attributes.SetColorMatrix(colorMatrix);
                        g.DrawImage(overlay, new Rectangle(0, 0, overlayTransparent.Width, overlayTransparent.Height),
                            0, 0, overlay.Width, overlay.Height, GraphicsUnit.Pixel, attributes);
                    }
                    Cursor overlayCursor = Cursors.Hand;
                    overlayCursor.Draw(g, new Rectangle(Point.Empty, overlayCursor.Size));
                }
                DDCursor = CreateCursor(overlayTransparent);
            }
        }

        #endregion
    }
}