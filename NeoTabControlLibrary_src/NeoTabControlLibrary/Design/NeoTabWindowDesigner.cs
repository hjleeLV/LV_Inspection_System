using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Security.Permissions;
using System.ComponentModel.Design;

namespace NeoTabControlLibrary.Design
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class NeoTabWindowDesigner : ParentControlDesigner
    {
        #region Instance Members

        private DesignerVerbCollection dsVerbs;
        private IComponentChangeService changeService;

        #endregion

        #region Constructor

        public NeoTabWindowDesigner()
            : base() { }

        #endregion

        #region Destructor

        ~NeoTabWindowDesigner()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Property

        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (dsVerbs == null)
                {
                    DesignerVerb[] dsverbItems = new DesignerVerb[]
                    {
                        new DesignerVerb("Add Tab", new EventHandler(AddTab)),
                        new DesignerVerb("Remove Tab", new EventHandler(RemoveTab))
                    };
                    dsVerbs = new DesignerVerbCollection();
                    dsVerbs.AddRange(dsverbItems);
                }
                return dsVerbs;
            }
        }

        #endregion

        #region Override Methods

        /*  As a general rule, always call the base method first in the PreFilterXxx() methods and last in the 
            PostFilterXxx() methods. This way, all designer classes are given the proper opportunity to apply their 
            changes. The ControlDesigner and ComponentDesigner use these methods to add properties like Visible, 
            Enabled, Name, and Locked. */

        /// <summary>
        /// Override this method to remove unused or inappropriate events.
        /// </summary>
        /// <param name="events">Events collection of the control.</param>
        protected override void PostFilterEvents(System.Collections.IDictionary events)
        {
            events.Remove("TextChanged");
            events.Remove("FontChanged");
            events.Remove("PaddingChanged");
            events.Remove("ForeColorChanged");
            events.Remove("BackgroundImageChanged");
            events.Remove("BackgroundImageLayoutChanged");

            base.PostFilterEvents(events);
        }

        /// <summary>
        /// Override this method to remove unused or inappropriate properties.
        /// </summary>
        /// <param name="properties">Properties collection of the control.</param>
        protected override void PostFilterProperties(System.Collections.IDictionary properties)
        {
            properties.Remove("Text");
            properties.Remove("Font");
            properties.Remove("Padding");
            properties.Remove("ForeColor");
            properties.Remove("BackColor");
            properties.Remove("BackgroundImage");
            properties.Remove("BackgroundImageLayout");

            base.PostFilterProperties(properties);
        }

        public override bool CanParent(System.Windows.Forms.Control control)
        {
            // Children can only be of type NeoTabPage. 
            return (control is NeoTabPage);
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            // Update your designer verb whenever ComponentChanged event occurs.
            if (changeService != null)
                changeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (changeService != null)
                    changeService.ComponentChanged -= OnComponentChanged;
            }

            base.Dispose(disposing);
        }

        protected override bool GetHitTest(System.Drawing.Point point)
        {
            NeoTabWindow tw = (NeoTabWindow)Component;
            if (tw != null)
            {
                point = Control.PointToClient(point);
                int itemIndex = -1;
                CommonObjects.ButtonState itemState;
                System.Drawing.Rectangle itemRectangle;
                NeoTabPage tp = tw.GetHitTest(point, out itemRectangle, out itemState, out itemIndex);
                // If the mouse is positioned over a TabPage item, allow the mouse events to occur.
                return (tp != null && tp.IsSelectable && itemIndex != tw.SelectedIndex);
            }
            return base.GetHitTest(point);
        }
        
        #endregion

        #region Helper Methods

        private void AddTab(object sender, EventArgs e)
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host == null)
                return;
            DesignerTransactionUtility.DoInTransaction
            (
                host,
                "AddTab",
                new TransactionMethod(Transaction_AddTab),
                Component
            );
        }

        private void RemoveTab(object sender, EventArgs e)
        {
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            if (host == null)
                return;
            DesignerTransactionUtility.DoInTransaction
            (
                host,
                "RemoveTab",
                new TransactionMethod(Transaction_RemoveTab),
                Component
            );
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (dsVerbs == null)
                return;
            if ((e.Component is NeoTabWindow == false) ||
                e.Member.Name != "Controls")
                return;

            NeoTabWindow tw = e.Component as NeoTabWindow;
            switch (tw.Controls.Count)
            {
                case 0:
                    dsVerbs[0].Enabled = true;
                    dsVerbs[1].Enabled = false;
                    break;
                default:
                    dsVerbs[0].Enabled = true;
                    dsVerbs[1].Enabled = true;
                    break;
            }
        }

        #endregion

        #region Transaction Methods

        private object Transaction_AddTab(IDesignerHost host, object parameter)
        {
            // Tab Page
            NeoTabPage tabPage = (NeoTabPage)host.CreateComponent(typeof(NeoTabPage));
            tabPage.BackColor = System.Drawing.Color.Transparent;
            // Tab Control
            NeoTabWindow tabControl = (NeoTabWindow)parameter;
            MemberDescriptor md = TypeDescriptor.GetProperties(tabControl)["Controls"];
            RaiseComponentChanging(md);
            tabControl.Controls.Add(tabPage);
            RaiseComponentChanged(md, null, null);
            return parameter;
        }

        private object Transaction_RemoveTab(IDesignerHost host, object parameter)
        {
            // Tab Control
            NeoTabWindow tabControl = (NeoTabWindow)parameter;
            MemberDescriptor md = TypeDescriptor.GetProperties(tabControl)["Controls"];
            RaiseComponentChanging(md);
            host.DestroyComponent(tabControl.SelectedTab);
            RaiseComponentChanged(md, null, null);
            return parameter;
        }

        #endregion
    }
}