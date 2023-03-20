using System;
using System.Collections;

namespace NeoTabControlLibrary
{
    public class NeoTabPageHidedMembersCollection : CollectionBase, IDisposable
    {
        #region Destructor

        ~NeoTabPageHidedMembersCollection()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Indexers

        public NeoTabPage this[int index]
        {
            get
            {
                return (NeoTabPage)this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        public NeoTabPage this[string name]
        {
            get
            {
                foreach (NeoTabPage tp in this)
                {
                    if (tp.Name.Equals(name))
                        return tp;
                }
                return null;
            }
        }

        #endregion

        #region General Methods

        public void Add(NeoTabPage tabPage)
        {
            if (this.List.Contains(tabPage))
                throw new ArgumentException(
                    "This tab page control is already within the collection.");
            else
                this.List.Add(tabPage);
        }

        public void Remove(NeoTabPage tabPage)
        {
            if (this.List.Contains(tabPage))
                this.List.Remove(tabPage);
        }

        public bool Contains(NeoTabPage tabPage)
        {
            if (this.List.Contains(tabPage))
                return true;
            else
                return false;
        }

        public int IndexOf(NeoTabPage tabPage)
        {
            return this.List.IndexOf(tabPage);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            foreach (NeoTabPage tp in this)
                tp.Dispose();
        }

        #endregion
    }
}