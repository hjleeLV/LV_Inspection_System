using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Security.Permissions;
using System.ComponentModel.Design;

namespace NeoTabControlLibrary.Design
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class NeoTabPageDesigner : ScrollableControlDesigner
    {
        #region Instance Members

        #endregion

        #region Constructor

        public NeoTabPageDesigner()
            : base() { }

        #endregion

        #region Destructor

        ~NeoTabPageDesigner()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Property

        public override SelectionRules SelectionRules
        {
            get
            {
                /* The component is locked to its container. This prevents moving 
                   and sizing, even if you’ve specified those flags. */
                return SelectionRules.Locked;
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
            events.Remove("DockChanged");
            events.Remove("MarginChanged");
            events.Remove("VisibleChanged");
            events.Remove("EnabledChanged");
            events.Remove("TabStopChanged");
            events.Remove("TabIndexChanged");
            events.Remove("LocationChanged");

            base.PostFilterEvents(events);
        }

        /// <summary>
        /// Override this method to remove unused or inappropriate properties.
        /// </summary>
        /// <param name="properties">Properties collection of the control.</param>
        protected override void PostFilterProperties(System.Collections.IDictionary properties)
        {
            properties.Remove("Size");
            properties.Remove("Dock");
            properties.Remove("Margin");
            properties.Remove("Anchor");
            properties.Remove("Visible");
            properties.Remove("Enabled");
            properties.Remove("TabStop");
            properties.Remove("TabIndex");
            properties.Remove("Location");
            properties.Remove("BorderStyle");
            properties.Remove("MaximumSize");
            properties.Remove("MinimumSize");

            base.PostFilterProperties(properties);
        }
        
        public override bool CanBeParentedTo(IDesigner parentDesigner)
        {
            // Control can only be parented by NeoTabWindow. 
            return (parentDesigner is NeoTabWindowDesigner);
        }

        #endregion

        #region Helper Methods

        #endregion

        #region General Methods

        #endregion
    }
}