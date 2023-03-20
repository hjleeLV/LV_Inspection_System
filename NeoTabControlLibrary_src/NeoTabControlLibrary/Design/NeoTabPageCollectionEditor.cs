using System;
using System.ComponentModel.Design;

namespace NeoTabControlLibrary.Design
{
    public class NeoTabPageCollectionEditor : CollectionEditor
    {
        #region Constructor

        public NeoTabPageCollectionEditor(Type type)
            : base(type) { }

        #endregion

        #region Destructor

        ~NeoTabPageCollectionEditor()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Override Methods

        protected override Type CreateCollectionItemType()
        {
            return typeof(NeoTabPage);
        }

        #endregion
    }
}