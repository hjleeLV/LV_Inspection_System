using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NeoTabControlLibrary
{
    /// <summary>
    /// Please be carefull to use this tool for your controls. Call the ResumeLogic() method after the SuspendLogic() method at the same control.
    /// </summary>
    public static class CustomControlsLogic
    {
        #region Symbolic Constants

        private const int WM_USER = 0x400;
        private const int WM_SETREDRAW = 0x000B;
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int EM_SETEVENTMASK = (WM_USER + 69);

        #endregion

        #region Interop

        [DllImport("user32")]
        private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        
        #endregion

        private static IntPtr EVENTMASK = IntPtr.Zero;

        #region General Methods

        /// <summary>
        /// Stops painting messages and events.
        /// </summary>
        /// <param name="parent">Parent control handle</param>
        public static void SuspendLogic(Control parent)
        {
            if (parent != null)
            {
                // Stop redrawing:
                SendMessage(parent.Handle, WM_SETREDRAW, 0, IntPtr.Zero);

                // Stop sending of events:
                EVENTMASK = SendMessage(parent.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Starts painting messages and events.
        /// </summary>
        /// <param name="parent">Parent control handle</param>
        public static void ResumeLogic(Control parent)
        {
            if (parent != null)
            {
                // Turn-On events:
                SendMessage(parent.Handle, EM_SETEVENTMASK, 0, EVENTMASK);

                // Turn-On redrawing:
                SendMessage(parent.Handle, WM_SETREDRAW, 1, IntPtr.Zero);

                parent.Refresh();
            }
        }

        /// <summary>
        /// Creates a new instance of the specified type using that type's allowed properties.
        /// </summary>
        /// <param name="source">Source Object (To be cloned.)</param>
        /// <returns>Returns created new object</returns>
        public static Object GetMyClone(Object source)
        {
            // Grab the type of the source instance.
            Type sourceType = source.GetType();

            // Firstly, we create a new instance using that type's default constructor.
            object newObject = Activator.CreateInstance(sourceType, false);

            foreach (System.Reflection.PropertyInfo pi in sourceType.GetProperties())
            {
                if (pi.CanWrite)
                {
                    // Gets custom attribute for the current property item.
                    IsCloneableAttribute attribute = GetAttributeForSpecifiedProperty(pi);
                    if (attribute == null)
                        continue;
                    
                    if (attribute.IsCloneable)
                    {
                        // We query if the property support the ICloneable interface.
                        Type ICloneType = pi.PropertyType.GetInterface("ICloneable", true);
                        if (ICloneType != null)
                        {
                            // Getting the ICloneable interface from the object.
                            ICloneable IClone = (ICloneable)pi.GetValue(source, null);
                            if (IClone != null)
                            {
                                // We use the Clone() method to set the new value to the property.
                                pi.SetValue(newObject, IClone.Clone(), null);
                            }
                            else
                            {
                                // If return value is null, just set it null.
                                pi.SetValue(newObject, null, null);
                            }
                        }
                        else
                        {
                            // If the property doesn't support the ICloneable interface then just set it.
                            pi.SetValue(newObject, pi.GetValue(source, null), null);
                        }
                    }
                }
            }

            return newObject;
        }

        #endregion
        
        #region Helper Methods

        private static IsCloneableAttribute GetAttributeForSpecifiedProperty(System.Reflection.MemberInfo element)
        {
            IsCloneableAttribute attribute = (IsCloneableAttribute)Attribute.GetCustomAttribute(element, typeof(IsCloneableAttribute));
            return attribute;
        }

        #endregion
    }

    /// <summary>
    /// To apply clone support a specific property, please use this tool.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IsCloneableAttribute : Attribute
    {
        #region Destructor

        ~IsCloneableAttribute()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        // Attribute constructor for positional parameters.
        public IsCloneableAttribute(bool isCloneable)
        {
            this.IsCloneable = isCloneable;
        }

        /// <summary>
        /// Determines whether the current property is cloneable or not.
        /// </summary>
        public bool IsCloneable { get; private set; }
    }
}