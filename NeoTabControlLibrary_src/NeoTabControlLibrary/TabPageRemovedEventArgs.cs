using System;

namespace NeoTabControlLibrary
{
    public sealed class TabPageRemovedEventArgs : EventArgs, IDisposable
    {
        #region Instance Member

        private NeoTabPage tabPage = null;
        private int tabPageIndex = -1;

        #endregion

        #region Constructor

        public TabPageRemovedEventArgs(NeoTabPage tabPage, int tabPageIndex)
        {
            this.tabPage = tabPage;
            this.tabPageIndex = tabPageIndex;
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets the zero-based index of the last removed NeoTabPage control from the collection.
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