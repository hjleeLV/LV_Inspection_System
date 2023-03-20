using System;
using System.Drawing;
using System.Windows.Forms;

namespace NeoTabControlLibrary
{
    public sealed class DropDownButtonClickedEventArgs : EventArgs, IDisposable
    {
        #region Instance Member

        private ContextMenuStrip contextMenu = null;
        private Point menuLocation = Point.Empty;

        #endregion

        #region Constructor

        public DropDownButtonClickedEventArgs(ContextMenuStrip contextMenu, Point menuLocation)
        {
            this.contextMenu = contextMenu;
            this.menuLocation = menuLocation;
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets the drop-down menu of the control.
        /// </summary>
        public ContextMenuStrip ContextMenu
        {
            get { return contextMenu; }
        }

        /// <summary>
        /// Gets or sets the location(in screen coordinates) of the drop-down menu.
        /// </summary>
        public Point MenuLocation
        {
            get { return menuLocation; }
            set
            {
                menuLocation = value;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}