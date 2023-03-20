using System;
using System.Drawing.Design;

namespace NeoTabControlLibrary.Design
{
    public class RendererNameEditor : UITypeEditor
    {
        #region Destructor

        ~RendererNameEditor()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Override Methods

        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(
            System.ComponentModel.ITypeDescriptorContext context)
        {
            // We will use a window for property editing.
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(
            System.ComponentModel.ITypeDescriptorContext context,
            System.IServiceProvider provider, object value)
        {
            NeoTabWindow tc = context.Instance as NeoTabWindow;
            using (System.Windows.Forms.FolderBrowserDialog fdialog =
                new System.Windows.Forms.FolderBrowserDialog())
            {
                fdialog.RootFolder = Environment.SpecialFolder.MyComputer;
                fdialog.Description = "Select application directory for control rendering.";
                fdialog.ShowNewFolderButton = false;
                if (fdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string rootFolder = System.IO.Directory.GetCurrentDirectory();
                    System.IO.Directory.SetCurrentDirectory(fdialog.SelectedPath);
                    // Shows a add-in model dialog box.
                    tc.ShowAddInRendererManager();
                    System.IO.Directory.SetCurrentDirectory(rootFolder);
                }
            }
            return tc.RendererName;
        }

        public override bool GetPaintValueSupported(
            System.ComponentModel.ITypeDescriptorContext context)
        {
            // No special thumbnail will be shown for the grid.
            return false;
        }

        #endregion
    }
}