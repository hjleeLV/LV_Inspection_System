using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.ComponentModel.Design;

namespace NeoTabControlLibrary.Design
{
    [Serializable]
    public class NeoTabWindowToolboxItem : ToolboxItem
    {
        #region Constructor

        public NeoTabWindowToolboxItem(Type toolType)
            : base(toolType)
        { }

        /* And you must provide this special constructor for serialization.
           If you add additional data to NeoTabWindowToolboxItem that you
           want to serialize, you may override Deserialize and Serialize methods to add that data. */
        public NeoTabWindowToolboxItem(SerializationInfo info, StreamingContext context)
        {
            Deserialize(info, context);
        }

        #endregion

        #region Destructor

        ~NeoTabWindowToolboxItem()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Override Methods

        protected override IComponent[] CreateComponentsCore(IDesignerHost host, 
            System.Collections.IDictionary defaultValues)
        {
            IComponent[] comps = base.CreateComponentsCore(host, defaultValues);
            // comps will have a single component: our data type(NeoTabWindow).
            return DesignerTransactionUtility.DoInTransaction
            (
                host,
                "NeoTabWindowToolboxItem_Create_New_NeoTabPage",
                new TransactionMethod(CreateControlWithOneTabPage),
                comps[0]
            ) as IComponent[];
        }

        #endregion

        #region Transaction Methods

        public object CreateControlWithOneTabPage(IDesignerHost host, object parameter)
        {
            // Tab Page
            NeoTabPage tabPage = (NeoTabPage)host.CreateComponent(typeof(NeoTabPage));
            tabPage.BackColor = System.Drawing.Color.Transparent;
            // Tab Control
            NeoTabWindow tabControl = (NeoTabWindow)parameter;
            tabControl.Controls.Add(tabPage);
            ((ISupportInitialize)tabControl).EndInit();
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
                    tabControl.ShowAddInRendererManager();
                    System.IO.Directory.SetCurrentDirectory(rootFolder);
                }
            }
            return parameter;
        }

        #endregion
    }

    public delegate object TransactionMethod(IDesignerHost host, object parameter);
    
    public abstract class DesignerTransactionUtility
    {
        public static object DoInTransaction(
            IDesignerHost host,
            string transactionName,
            TransactionMethod transactionMethod,
            object parameter)
        {
            DesignerTransaction transaction = null;
            object RetVal = null;

            try
            {
                // Create a new designer transaction.
                transaction = host.CreateTransaction(transactionName);

                // Do the task polymorphically.
                RetVal = transactionMethod(host, parameter);
            }
            catch (CheckoutException ex)	// something has gone wrong with transaction creation.
            {
                if (ex != CheckoutException.Canceled)
                    throw ex;
            }
            catch
            {
                if (transaction != null)
                {
                    transaction.Cancel();
                    transaction = null;	// the transaction won't commit in the finally block.
                }

                throw;
            }
            finally
            {
                if (transaction != null)
                    transaction.Commit();
            }

            return RetVal;
        }
    }
}