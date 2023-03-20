using System;

namespace NeoTabControlLibrary
{
    public sealed class TabPageRemovingEventArgs : EventArgs, IDisposable
    {
        #region Instance Member

        private NeoTabPage tabPage = null;
        private int tabPageIndex = -1;
        private bool cancel = false;

        #endregion

        #region Constructor

        public TabPageRemovingEventArgs(NeoTabPage tabPage, int tabPageIndex)
        {
            this.tabPage = tabPage;
            this.tabPageIndex = tabPageIndex;
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
            set
            {
                if (!value.Equals(cancel))
                    cancel = value;
            }
        }

        /// <summary>
        /// Gets the zero-based index of the NeoTabPage control in the NeoTabWindow.TabPages control collection.
        /// </summary>
        public int TabPageIndex
        {
            get { return tabPageIndex; }
        }

        /// <summary>
        /// Gets the NeoTabPage control the event is occurring for.
        /// </summary>
        public NeoTabPage TabPage
        {
            get { return tabPage; }
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