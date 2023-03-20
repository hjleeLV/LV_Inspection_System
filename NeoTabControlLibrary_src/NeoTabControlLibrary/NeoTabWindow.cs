using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.Design;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary
{
    [Designer(typeof(NeoTabControlLibrary.Design.NeoTabWindowDesigner))]
    [DefaultEvent("SelectedIndexChanged"), DefaultProperty("TabPages")]
    [ToolboxItem(typeof(NeoTabControlLibrary.Design.NeoTabWindowToolboxItem))]
    public class NeoTabWindow : Control, ISupportInitialize, ICloneable
    {
        #region Events

        /// <summary>
        /// Event raised when the current renderer is updated/edited.
        /// </summary>
        [Description("Event raised when the current renderer is updated/edited")]
        public event EventHandler RendererUpdated;

        /// <summary>
        /// Occurs when the value of the Renderer property changes.
        /// </summary>
        [Description("Occurs when the value of the Renderer property changes")]
        public event EventHandler RendererChanged;

        /// <summary>
        /// Occurs when the value of the SelectedIndex property changes.
        /// </summary>
        [Description("Occurs when the value of the SelectedIndex property changes")]
        public event EventHandler<SelectedIndexChangedEventArgs> SelectedIndexChanged;

        /// <summary>
        /// Occurs when a NeoTabPage control is being selected.
        /// </summary>
        [Description("Occurs when a NeoTabPage control is being selected")]
        public event EventHandler<SelectedIndexChangingEventArgs> SelectedIndexChanging;

        /// <summary>
        /// Event raised when a NeoTabPage control is removed from this control.
        /// </summary>
        [Description("Event raised when a NeoTabPage control is removed from this control")]
        public event EventHandler<TabPageRemovedEventArgs> TabPageRemoved;

        /// <summary>
        /// Occurs when a NeoTabPage control is being removed.
        /// </summary>
        [Description("Occurs when a NeoTabPage control is being removed")]
        public event EventHandler<TabPageRemovingEventArgs> TabPageRemoving;

        /// <summary>
        /// Event raised when the smart drop down button is clicked on the control.
        /// </summary>
        [Description("Event raised when the smart drop down button is clicked on the control")]
        public event EventHandler<DropDownButtonClickedEventArgs> DropDownButtonClicked;

        #endregion

        #region Enums

        /// <summary>
        /// Drag and drop style for this control.
        /// </summary>
        public enum DragDropStyle
        {
            PageEffect,
            TabPageItemEffect
        };

        #endregion

        #region Symbolic Constants

        protected static readonly Size DEFAULT_SIZE = new Size(300, 200);

        #endregion

        #region Static Members Of The Class

        private static int barMaxValue = 1;
        private static ToolTips myTooltipForm = null;
        private static DDPaintCursor myDdCursor = null;

        #endregion

        #region Instance Members

        private int selectedIndex = -1;
        private RendererBase renderer = null;
        private TooltipRenderer tooltipRenderer = null;
        private Point queueTooltipPoint = Point.Empty;
        private DragDropStyle draggingStyle = DragDropStyle.TabPageItemEffect;
        private NeoTabPageControlCollection tabPages = null;
        private Dictionary<Rectangle, CommonObjects.ButtonState> tpItemRectangles =
            new Dictionary<Rectangle, CommonObjects.ButtonState>();
        private Dictionary<Rectangle, CommonObjects.ButtonState> smartButtonRectangles =
            new Dictionary<Rectangle, CommonObjects.ButtonState>(2);
        private NeoTabPageHidedMembersCollection hidedMembers =
            new NeoTabPageHidedMembersCollection();

        #endregion

        #region Constructor

        public NeoTabWindow()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw,
                 true);
            renderer = new DefaultRenderer();
            tooltipRenderer = new TooltipRenderer();
        }

        #endregion

        #region Destructor

        ~NeoTabWindow()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets the currently selected tab page.
        /// </summary>
        [Browsable(false)]
        public NeoTabPage SelectedTab
        {
            get
            {
                if (selectedIndex != -1)
                    return TabPages[selectedIndex] as NeoTabPage;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the index of the currently selected tab page.
        /// </summary>
        [Description("Gets or sets the index of the currently selected tab page")]
        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (!value.Equals(selectedIndex))
                {
                    // Check to see if there is an item at the supplied index.
                    if ((value >= TabPages.Count) || (value < 0))
                    {
                        throw new IndexOutOfRangeException();
                    }
                    else
                    {
                        selectedIndex = value;
                        if (!DesignMode)
                            CustomControlsLogic.SuspendLogic(Parent);
                        for (int i = 0; i < this.Controls.Count; i++)
                        {
                            NeoTabPage tp = TabPages[i] as NeoTabPage;
                            if (i == selectedIndex)
                            {
                                tp.ProgressUsedValue++;
                                tp.Visible = true;
                            }
                            else
                                tp.Visible = false;
                            barMaxValue = Math.Max(barMaxValue, tp.ProgressUsedValue);
                        }
                        if (initializing)
                        {
                            if (DesignMode)
                            {
                                ISelectionService selectionService =
                                    (ISelectionService)GetService(typeof(ISelectionService));
                                if (selectionService != null)
                                    selectionService.SetSelectedComponents(new IComponent[] { SelectedTab });
                            }
                            using (SelectedIndexChangedEventArgs e =
                                new SelectedIndexChangedEventArgs(TabPages[selectedIndex] as NeoTabPage, selectedIndex))
                            {
                                OnSelectedIndexChanged(e);
                            }
                        }
                        if (!DesignMode)
                            CustomControlsLogic.ResumeLogic(Parent);
                        else
                        {
                            Invalidate();
                            Update();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the tooltip effect is enable or not for this tab control.
        /// </summary>
        [Description("Determines whether the tooltip effect is enable or not for this tab control")]
        [DefaultValue(false)]
        [IsCloneable(true)]
        public bool IsTooltipEnabled { get; set; }

        /// <summary>
        /// Gets or sets the drag and drop style for this control.
        /// </summary>
        [Description("Gets or sets the drag and drop style for this control")]
        [DefaultValue(typeof(DragDropStyle), "TabPageItemEffect")]
        [IsCloneable(true)]
        public DragDropStyle DraggingStyle
        {
            get { return draggingStyle; }
            set
            {
                if (!value.Equals(draggingStyle))
                    draggingStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the type name of the renderer class for starting control with a custom renderer.
        /// </summary>
        [Description("Gets or sets the type name of the renderer class for starting control with a custom renderer")]
        [DefaultValue("DefaultRenderer")]
        [Editor(typeof(NeoTabControlLibrary.Design.RendererNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        [IsCloneable(true)]
        public string RendererName { get; set; }

        /// <summary>
        /// Gets or sets the renderer class of the NeoTabWindow control.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [IsCloneable(true)]
        public RendererBase Renderer
        {
            get { return renderer; }
            set
            {
                try
                {
                    if (!value.Equals(renderer))
                    {
                        renderer.RendererUpdated -= OnRendererUpdated;
                        if (!DesignMode)
                        {
                            CustomControlsLogic.SuspendLogic(Parent);
                            renderer = value;
                            RebuildControl();
                            RebuildSmartButtons();
                            UpdateStyles();
                            OnRendererChanged();
                            CustomControlsLogic.ResumeLogic(Parent);
                        }
                        else
                        {
                            renderer = value;
                            RebuildControl();
                            RebuildSmartButtons();
                            UpdateStyles();
                        }
                        renderer.RendererUpdated += new EventHandler(OnRendererUpdated);
                    }
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Value cannot be null!, please enter a valid value.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the tooltip renderer of the control.
        /// </summary>
        [Description("Gets or sets the tooltip renderer of the control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [IsCloneable(true)]
        public TooltipRenderer TooltipRenderer
        {
            get { return tooltipRenderer; }
            set
            {
                try
                {
                    if (!value.Equals(tooltipRenderer))
                        tooltipRenderer = value;
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Value cannot be null!, please enter a valid value.");
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NeoTabPageControlCollection TabPages
        {
            get { return tabPages; }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                return new Rectangle(
                    base.DisplayRectangle.Left + Padding.Left, // x
                    base.DisplayRectangle.Top + Padding.Top,   // y
                    base.DisplayRectangle.Width - (Padding.Left + Padding.Right),   // width
                    base.DisplayRectangle.Height - (Padding.Top + Padding.Bottom)); // height
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return DEFAULT_SIZE;
            }
        }

        #endregion

        #region Override Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Visible)
            {
                // Draw control background.
                renderer.OnRendererBackground(e.Graphics, ClientRectangle);
                Rectangle tabPageAreaRct = DisplayRectangle;
                tabPageAreaRct.X -= renderer.TabPageAreaCornerOffset.Left;
                tabPageAreaRct.Y -= renderer.TabPageAreaCornerOffset.Top;
                tabPageAreaRct.Width += (renderer.TabPageAreaCornerOffset.Left + renderer.TabPageAreaCornerOffset.Right);
                tabPageAreaRct.Height += (renderer.TabPageAreaCornerOffset.Top + renderer.TabPageAreaCornerOffset.Bottom);
                // Draw panel area.
                renderer.OnRendererTabPageArea(e.Graphics, tabPageAreaRct);
                if (tpItemRectangles.Count == 0)
                    return;
                // Draw smart buttons.
                int i = -1;
                Rectangle checkingSmartBtnRct = Rectangle.Empty;
                foreach (KeyValuePair<Rectangle, CommonObjects.ButtonState> current in smartButtonRectangles)
                {
                    i++;
                    switch (i)
                    {
                        /*** SmartCloseButton. ***/
                        case 0: 
                            if (!current.Key.IsEmpty)
                                renderer.OnDrawSmartCloseButton(e.Graphics, current.Key,
                                    SelectedTab.IsCloseable ? current.Value : CommonObjects.ButtonState.Disabled);
                            break;
                        /*** SmartDropDownButton. ***/
                        default:
                            renderer.OnDrawSmartDropDownButton(e.Graphics, current.Key, current.Value);
                            break;
                    }
                    checkingSmartBtnRct = current.Key;
                }
                // Draw tabPage items.
                i = -1;
                foreach (KeyValuePair<Rectangle, CommonObjects.ButtonState> current in tpItemRectangles)
                {
                    i++;
                    bool isWrapped = false;
                    // Always draw first tab page.
                    if (i > 0)
                    {
                        switch (renderer.NeoTabPageItemsSide)
                        {
                            case TabPageLayout.Top:
                            case TabPageLayout.Bottom:
                                if (current.Key.Right >=
                                    (checkingSmartBtnRct.IsEmpty ? DisplayRectangle.Right : checkingSmartBtnRct.Left))
                                    isWrapped = true;
                                break;
                            default:
                                if (current.Key.Bottom >=
                                    (checkingSmartBtnRct.IsEmpty ? DisplayRectangle.Bottom : checkingSmartBtnRct.Top))
                                    isWrapped = true;
                                break;
                        }
                    }
                    if (!isWrapped)
                    {
                        NeoTabPage tp = TabPages[i] as NeoTabPage;
                        CommonObjects.ButtonState btnState;
                        if (selectedIndex == i)
                            btnState = CommonObjects.ButtonState.Pressed;
                        else
                        {
                            if (!tp.IsSelectable)
                                btnState = CommonObjects.ButtonState.Disabled;
                            else
                                btnState = current.Value;
                        }

                        renderer.OnRendererTabPageItem(e.Graphics, current.Key,
                            String.IsNullOrEmpty(tp.Text) ? tp.Name : tp.Text,
                            i, btnState);
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (tpItemRectangles.Count == 0)
                return;
            if (renderer.IsSupportSmartCloseButton || renderer.IsSupportSmartDropDownButton)
                RebuildSmartButtons();
            if ((renderer.NeoTabPageItemsSide == TabPageLayout.Bottom) ||
                (renderer.NeoTabPageItemsSide == TabPageLayout.Right))
            {
                RebuildControl();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            foreach (NeoTabPage tabPage in this.Controls)
            {
                int x, y, w, h;

                x = DisplayRectangle.Location.X;
                y = DisplayRectangle.Location.Y;

                w = DisplayRectangle.Size.Width;
                h = DisplayRectangle.Size.Height;

                tabPage.SetBounds(x, y, w, h, BoundsSpecified.All);
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (e.Control is NeoTabPage)
            {
                NeoTabPage tp = e.Control as NeoTabPage;
                if (String.IsNullOrEmpty(tp.Name))
                {
                    int value = 0;
                    foreach (NeoTabPage tabPage in this.Controls)
                    {
                        try
                        {
                            if (tabPage.Name.Contains("neoTabPage"))
                            {
                                int current;
                                if (Int32.TryParse(tabPage.Name.Substring(10, tabPage.Name.Length - 10),
                                    out current))
                                {
                                    value = Math.Max(value, current);
                                }
                            }
                        }
                        catch { ;}
                    }
                    tp.Name = String.Format("neoTabPage{0}", ++value);
                }
                if (String.IsNullOrEmpty(tp.Text))
                {
                    tp.Text = tp.Name;
                }
                if (String.IsNullOrEmpty(tp.ToolTipText))
                {
                    tp.ToolTipText = tp.Text;
                }
                tp.Dock = DockStyle.Fill;
                tp.TextChanged += (sender, ea) =>
                    {
                        if (initializing)
                        {
                            RebuildControl();
                            UpdateStyles();
                        }
                    };
                base.OnControlAdded(e);
                if (Controls.Count == 1)
                    selectedIndex = 0;
                if (initializing)
                {
                    RebuildControl();
                    UpdateStyles();
                }
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            RebuildControl();
            SelectNextAvailableTabPage();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button != System.Windows.Forms.MouseButtons.None)
                return;
            Rectangle checkingSmartBtnRct = Rectangle.Empty;
            if (this.Controls.Count > 0)
            {
                int itemIndex = -1;
                List<Rectangle> list = new List<Rectangle>(smartButtonRectangles.Keys);
                foreach (Rectangle smartButtonRectangle in list)
                {
                    itemIndex++;
                    if (!smartButtonRectangle.Contains(e.Location))
                    {
                        if (smartButtonRectangles[smartButtonRectangle]
                            == CommonObjects.ButtonState.Hover)
                        {
                            smartButtonRectangles[smartButtonRectangle] = CommonObjects.ButtonState.Normal;
                            Invalidate(smartButtonRectangle);
                        }
                    }
                    else
                    {
                        CommonObjects.ButtonState smartButtonState = smartButtonRectangles[smartButtonRectangle];
                        if (itemIndex == 0 ? (SelectedTab.IsCloseable && smartButtonState
                            != CommonObjects.ButtonState.Hover) : smartButtonState
                            != CommonObjects.ButtonState.Hover)
                        {
                            smartButtonRectangles[smartButtonRectangle] = CommonObjects.ButtonState.Hover;
                            Invalidate(smartButtonRectangle);
                        }
                    }
                    checkingSmartBtnRct = smartButtonRectangle;
                }
            }
            if (this.Controls.Count > 1)
            {
                int itemIndex = -1;
                List<Rectangle> list = new List<Rectangle>(tpItemRectangles.Keys);
                foreach (Rectangle itemRectangle in list)
                {
                    itemIndex++;
                    if (!itemRectangle.Contains(e.Location))
                    {
                        if (tpItemRectangles[itemRectangle]
                            == CommonObjects.ButtonState.Hover)
                        {
                            tpItemRectangles[itemRectangle] = CommonObjects.ButtonState.Normal;
                            Invalidate(itemRectangle);
                        }
                    }
                    else
                    {
                        NeoTabPage tp = TabPages[itemIndex] as NeoTabPage;
                        CommonObjects.ButtonState itemState = tpItemRectangles[itemRectangle];
                        if (itemState != CommonObjects.ButtonState.Hover && tp != null &&
                            selectedIndex != itemIndex && tp.IsSelectable)
                        {
                            bool isAvailable = true;
                            switch (renderer.NeoTabPageItemsSide)
                            {
                                case TabPageLayout.Top:
                                case TabPageLayout.Bottom:
                                    if (itemRectangle.Right >=
                                        (checkingSmartBtnRct.IsEmpty ? DisplayRectangle.Right : checkingSmartBtnRct.Left))
                                        isAvailable = false;
                                    break;
                                default:
                                    if (itemRectangle.Bottom >=
                                        (checkingSmartBtnRct.IsEmpty ? DisplayRectangle.Bottom : checkingSmartBtnRct.Top))
                                        isAvailable = false;
                                    break;
                            }
                            if (isAvailable)
                            {
                                tpItemRectangles[itemRectangle] = CommonObjects.ButtonState.Hover;
                                Invalidate(itemRectangle);
                                if (!DesignMode && IsTooltipEnabled)
                                {
                                    queueTooltipPoint = e.Location;
                                    // Shows a tooltip to the user for currently active tab page.
                                    ShowTooltip(tp, itemRectangle);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.Controls.Count > 0)
                {
                    int result = -1;
                    Rectangle smartButtonRectangle;
                    CommonObjects.ButtonState smartButtonState;
                    GetSmartButtonHitTest(e.Location, out smartButtonRectangle,
                        out smartButtonState, out result);
                    switch (result)
                    {
                        /*** Is on the SmartCloseButton. ***/
                        case 0:
                            NeoTabPage stp = this.SelectedTab;
                            if (stp.IsCloseable)
                                Remove(stp);
                            break;
                        /*** Is on the SmartDropDownButton. ***/
                        case 1:
                            ContextMenuStrip dropDownMenu = new ContextMenuStrip();
                            Point menuLocation = PointToScreen(new Point(smartButtonRectangle.Left, smartButtonRectangle.Bottom));
                            using (DropDownButtonClickedEventArgs ea = 
                                new DropDownButtonClickedEventArgs(dropDownMenu, menuLocation))
                            {
                                // Fire a Notification Event.
                                OnDropDownButtonClicked(ea);
                            }
                            break;
                        /*** Is not found! ***/
                        default:
                            if (this.Controls.Count > 1)
                            {
                                int itemIndex = -1;
                                Rectangle itemRectangle;
                                CommonObjects.ButtonState itemState;
                                NeoTabPage tabItem = GetHitTest(e.Location,
                                    out itemRectangle, out itemState, out itemIndex);
                                if (tabItem != null && tabItem.IsSelectable)
                                {
                                    bool isAvailable = true;
                                    switch (renderer.NeoTabPageItemsSide)
                                    {
                                        case TabPageLayout.Top:
                                        case TabPageLayout.Bottom:
                                            if (itemRectangle.Right >= 
                                                (smartButtonRectangle.IsEmpty ? DisplayRectangle.Right : smartButtonRectangle.Left))
                                                isAvailable = false;
                                            break;
                                        default:
                                            if (itemRectangle.Bottom >= 
                                                (smartButtonRectangle.IsEmpty ? DisplayRectangle.Bottom : smartButtonRectangle.Top))
                                                isAvailable = false;
                                            break;
                                    }
                                    if (isAvailable)
                                    {
                                        if (selectedIndex != itemIndex)
                                        {
                                            using (SelectedIndexChangingEventArgs ea =
                                                new SelectedIndexChangingEventArgs(tabItem, itemIndex))
                                            {
                                                // Fire a Notification Event.
                                                OnSelectedIndexChanging(ea);

                                                if (!ea.Cancel)
                                                    this.SelectedIndex = ea.TabPageIndex;
                                            }
                                        }
                                        else
                                        {
                                            if (!DesignMode && AllowDrop)
                                            {
                                                // Starts a drag & drop operation for currently selected tab page.
                                                BeginDragDrop(tabItem, itemRectangle,
                                                    CommonObjects.ButtonState.Pressed, itemIndex);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.Controls.Count > 0)
            {
                List<Rectangle> list = new List<Rectangle>(smartButtonRectangles.Keys);
                foreach (Rectangle smartButtonRectangle in list)
                {
                    if (smartButtonRectangles[smartButtonRectangle]
                        == CommonObjects.ButtonState.Hover)
                    {
                        smartButtonRectangles[smartButtonRectangle] = CommonObjects.ButtonState.Normal;
                        Invalidate(smartButtonRectangle);
                    }
                }
            }
            if (this.Controls.Count > 1)
            {
                queueTooltipPoint = new Point(-1, -1);
                List<Rectangle> list = new List<Rectangle>(tpItemRectangles.Keys);
                foreach (Rectangle itemRectangle in list)
                {
                    if (tpItemRectangles[itemRectangle]
                        == CommonObjects.ButtonState.Hover)
                    {
                        tpItemRectangles[itemRectangle] = CommonObjects.ButtonState.Normal;
                        Invalidate(itemRectangle);
                    }
                }
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            if (!DesignMode && drgevent.Data.GetDataPresent(typeof(NeoTabPage)))
            {
                Point pt = PointToClient(new Point(drgevent.X, drgevent.Y));
                int result = -1;
                Rectangle smartButtonRectangle;
                CommonObjects.ButtonState smartButtonState;
                GetSmartButtonHitTest(pt, out smartButtonRectangle,
                    out smartButtonState, out result);
                if (result >= 0)
                    return;
                int itemIndex = -1;
                Rectangle itemRectangle;
                CommonObjects.ButtonState itemState;
                NeoTabPage overTab = GetHitTest(pt,
                    out itemRectangle, out itemState, out itemIndex);
                if (overTab != null)
                {
                    bool isAvailable = true;
                    switch (renderer.NeoTabPageItemsSide)
                    {
                        case TabPageLayout.Top:
                        case TabPageLayout.Bottom:
                            if (itemRectangle.Right >=
                                (smartButtonRectangle.IsEmpty ? DisplayRectangle.Right : smartButtonRectangle.Left))
                                isAvailable = false;
                            break;
                        default:
                            if (itemRectangle.Bottom >= 
                                (smartButtonRectangle.IsEmpty ? DisplayRectangle.Bottom : smartButtonRectangle.Top))
                                isAvailable = false;
                            break;
                    }
                    if (isAvailable)
                    {
                        NeoTabPage draggingTab =
                            drgevent.Data.GetData(typeof(NeoTabPage)) as NeoTabPage;
                        if (!draggingTab.Equals(overTab))// If itself.
                        {
                            if (!draggingTab.Parent.Equals(overTab.Parent))
                            {
                                ((NeoTabWindow)draggingTab.Parent).Controls.Remove(draggingTab);
                                Controls.Add(draggingTab);
                                Controls.SetChildIndex(draggingTab, itemIndex);
                            }
                            else
                            {
                                // Switching tab indexes, we don't remove the dragging tab because, dragging tab already within the current collection.
                                Controls.SetChildIndex(overTab, selectedIndex);
                                Controls.SetChildIndex(draggingTab, itemIndex);
                            }
                            RebuildControl();
                            this.SelectedIndex = itemIndex;
                        }
                    }
                }
            }
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);
            if (!DesignMode)
            {
                //if (drgevent.KeyState == 1 && drgevent.Data.GetDataPresent(typeof(NeoTabPage)))
                //    drgevent.Effect = DragDropEffects.Move;
                //else
                //    drgevent.Effect = DragDropEffects.None;
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs gfbevent)
        {
            base.OnGiveFeedback(gfbevent);
            if (!DesignMode)
            {
                if ((gfbevent.Effect & DragDropEffects.Move) == DragDropEffects.Move)
                {
                    gfbevent.UseDefaultCursors = false;
                    Cursor.Current = myDdCursor.DDCursor;
                }
            }
        }

        protected override bool ProcessMnemonic(char charCode)
        {
            for (int i = 0; i < this.Controls.Count; i++)
            {
                NeoTabPage tp = TabPages[i] as NeoTabPage;
                if (IsMnemonic(charCode, tp.Text))
                {
                    if (tp.IsSelectable && selectedIndex != i)
                    {
                        using (SelectedIndexChangingEventArgs e =
                            new SelectedIndexChangingEventArgs(tp, i))
                        {
                            // Fire a Notification Event.
                            OnSelectedIndexChanging(e);

                            if (!e.Cancel)
                                this.SelectedIndex = e.TabPageIndex;
                        }
                    }
                    break;
                }
            }

            return base.ProcessMnemonic(charCode);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                // Selects last tab.
                case Keys.End:
                    OnNavigateTabPage(this.Controls.Count - 1);
                    break;
                // Selects first tab.
                case Keys.Home:
                    OnNavigateTabPage(0);
                    break;
                // Selects the tab on the left side of the currently selected tab.
                //case Keys.Left:
                case Keys.Tab | Keys.Control | Keys.Shift:
                    OnNavigateTabPage(selectedIndex - 1);
                    break;
                // Selects the tab on the right side of the currently selected tab.
                //case Keys.Right:
                case Keys.Tab | Keys.Control:
                    OnNavigateTabPage(selectedIndex + 1);
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override ControlCollection CreateControlsInstance()
        {
            if (tabPages == null)
                tabPages = new NeoTabPageControlCollection(this);

            return tabPages;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            /* Managed Resources */
            if (disposing)
            {
                // Dispose renderer class.
                renderer.Dispose();
                // Dispose tooltip renderer class.
                tooltipRenderer.Dispose();
                // Dispose the NeoTabPageHidedMembersCollection class.
                hidedMembers.Dispose();
                // Dispose all child controls.
                foreach (Control control in this.Controls)
                    control.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Virtual Methods

        protected virtual void OnRendererChanged()
        {
            if (RendererChanged != null)
                RendererChanged(this, EventArgs.Empty);
        }

        protected virtual void OnRendererUpdated(object sender, EventArgs e)
        {
            CustomControlsLogic.SuspendLogic(Parent);
            RebuildControl();
            UpdateStyles();
            if (RendererUpdated != null)
                RendererUpdated(sender, e);
            CustomControlsLogic.ResumeLogic(Parent);
        }

        protected virtual void OnSelectedIndexChanged(SelectedIndexChangedEventArgs e)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, e);
        }

        protected virtual void OnSelectedIndexChanging(SelectedIndexChangingEventArgs e)
        {
            if (SelectedIndexChanging != null)
                SelectedIndexChanging(this, e);
        }

        protected virtual void OnTabPageRemoved(TabPageRemovedEventArgs e)
        {
            if (TabPageRemoved != null)
                TabPageRemoved(this, e);
        }

        protected virtual void OnTabPageRemoving(TabPageRemovingEventArgs e)
        {
            if (TabPageRemoving != null)
                TabPageRemoving(this, e);
        }

        protected virtual void OnDropDownButtonClicked(DropDownButtonClickedEventArgs e)
        {
            if (this.Controls.Count > 0)
            {
                ToolStripMenuItem menuItem = null;
                menuItem = new ToolStripMenuItem("Close", null, null,
                    Keys.Control | Keys.C);
                menuItem.ImageScaling = ToolStripItemImageScaling.None;
                menuItem.Enabled = SelectedTab.IsCloseable ? true : false;
                if (menuItem.Enabled)
                {
                    menuItem.Click += (sender, ea) =>
                    {
                        Remove(SelectedTab);
                    };
                }
                e.ContextMenu.Items.Add(menuItem);
                menuItem = new ToolStripMenuItem("Show/Hide Tab Items", null, null,
                    Keys.Control | Keys.M);
                menuItem.ImageScaling = ToolStripItemImageScaling.None;
                menuItem.Click += (sender, ea) =>
                {
                    ShowTabManager();
                };
                e.ContextMenu.Items.Add(menuItem);
                if (this.Controls.Count > 1)
                {
                    e.ContextMenu.Items.Add(new ToolStripSeparator());
                    int n = -1;
                    foreach (NeoTabPage tp in this.TabPages)
                    {
                        n++;
                        if (!tp.Equals(SelectedTab))
                        {
                            menuItem = new ToolStripMenuItem(tp.Text, null, null, n.ToString());
                            menuItem.ImageScaling = ToolStripItemImageScaling.None;
                            menuItem.Enabled = tp.IsSelectable ? true : false;
                            if (menuItem.Enabled)
                            {
                                menuItem.Click += (sender, ea) =>
                                {
                                    OnNavigateTabPage(Int32.Parse(((ToolStripItem)sender).Name));
                                };
                            }
                            e.ContextMenu.Items.Add(menuItem);
                        }
                    }
                }
                if (DropDownButtonClicked != null)
                    DropDownButtonClicked(this, e);
                e.ContextMenu.Show(e.MenuLocation);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Do its base processes for control creating.
        /// </summary>
        private void RebuildControl()
        {
            Rectangle rct = this.ClientRectangle;

            // Removes all items in the collection.
            tpItemRectangles.Clear();
            this.BackColor = renderer.BackColor;
            /* Top, Left, MaxWidth, MaxHeight, Margin Spacings and display rectangles(left, top, right, bottom), IMGSIZE. */
            int[] tpItemsPosAndSize = new int[10];
            tpItemsPosAndSize[4] = renderer.ItemObjectsDrawingMargin;
            tpItemsPosAndSize[9] = 48;// IMGSIZE.
            using (Graphics g = this.CreateGraphics())
            {
                switch (renderer.NeoTabPageItemsSide)
                {
                    case TabPageLayout.Top:
                        tpItemsPosAndSize[1] = renderer.TabPageItemsAreaOffset.Left;
                        tpItemsPosAndSize[0] = renderer.TabPageItemsAreaOffset.Bottom;
                        switch (renderer.NeoTabPageItemsStyle)
                        {
                            case TabPageItemStyle.OnlyText:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = tpItemsPosAndSize[1];

                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    tpItemsPosAndSize[2] = txtSize.Width;
                                    tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                    tpItemsPosAndSize[3] = txtSize.Height;
                                    tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);
                                    tpItemRectangles.Add(new Rectangle(xLoc, 0, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[1] += (tpItemsPosAndSize[2] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.Only16x16_Image:
                                tpItemsPosAndSize[9] = 16;
                                goto case TabPageItemStyle.Only48x48_Image;
                            case TabPageItemStyle.Only32x32_Image:
                                tpItemsPosAndSize[9] = 32;
                                goto case TabPageItemStyle.Only48x48_Image;
                            case TabPageItemStyle.Only48x48_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = tpItemsPosAndSize[1];

                                    tpItemsPosAndSize[2] = tpItemsPosAndSize[9];
                                    tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                    tpItemsPosAndSize[3] = tpItemsPosAndSize[9];
                                    tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);
                                    tpItemRectangles.Add(new Rectangle(xLoc, 0, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[1] += (tpItemsPosAndSize[2] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.TextAnd16x16_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = tpItemsPosAndSize[1];

                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    tpItemsPosAndSize[2] = txtSize.Width + 16;
                                    tpItemsPosAndSize[2] += (3 * tpItemsPosAndSize[4]);
                                    tpItemsPosAndSize[3] = Math.Max(txtSize.Height, 16);
                                    tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);
                                    tpItemRectangles.Add(new Rectangle(xLoc, 0, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[1] += (tpItemsPosAndSize[2] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.TextAnd32x32_Image:
                                tpItemsPosAndSize[9] = 32;
                                goto case TabPageItemStyle.TextAnd48x48_Image;
                            case TabPageItemStyle.TextAnd48x48_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = tpItemsPosAndSize[1];

                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    tpItemsPosAndSize[2] = Math.Max(txtSize.Width, tpItemsPosAndSize[9]);
                                    tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                    tpItemsPosAndSize[3] = txtSize.Height + tpItemsPosAndSize[9];
                                    tpItemsPosAndSize[3] += (3 * tpItemsPosAndSize[4]);
                                    tpItemRectangles.Add(new Rectangle(xLoc, 0, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[1] += (tpItemsPosAndSize[2] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                        }
                        // left padding.
                        tpItemsPosAndSize[5] = renderer.TabPageAreaCornerOffset.Left;
                        // top padding.
                        tpItemsPosAndSize[6] = renderer.TabPageAreaCornerOffset.Top + tpItemsPosAndSize[3] + tpItemsPosAndSize[0];
                        // right padding.
                        tpItemsPosAndSize[7] = renderer.TabPageAreaCornerOffset.Right;
                        // bottom padding.
                        tpItemsPosAndSize[8] = renderer.TabPageAreaCornerOffset.Bottom;
                        break;
                    case TabPageLayout.Bottom:
                        tpItemsPosAndSize[1] = renderer.TabPageItemsAreaOffset.Left;
                        tpItemsPosAndSize[0] = renderer.TabPageItemsAreaOffset.Top;
                        switch (renderer.NeoTabPageItemsStyle)
                        {
                            case TabPageItemStyle.OnlyText:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = tpItemsPosAndSize[1];

                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    tpItemsPosAndSize[2] = txtSize.Width;
                                    tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                    tpItemsPosAndSize[3] = txtSize.Height;
                                    tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);

                                    int yLoc = rct.Bottom - tpItemsPosAndSize[3];
                                    tpItemRectangles.Add(new Rectangle(xLoc, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[1] += (tpItemsPosAndSize[2] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.Only16x16_Image:
                                tpItemsPosAndSize[9] = 16;
                                goto case TabPageItemStyle.Only48x48_Image;
                            case TabPageItemStyle.Only32x32_Image:
                                tpItemsPosAndSize[9] = 32;
                                goto case TabPageItemStyle.Only48x48_Image;
                            case TabPageItemStyle.Only48x48_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = tpItemsPosAndSize[1];

                                    tpItemsPosAndSize[2] = tpItemsPosAndSize[9];
                                    tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                    tpItemsPosAndSize[3] = tpItemsPosAndSize[9];
                                    tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);

                                    int yLoc = rct.Bottom - tpItemsPosAndSize[3];
                                    tpItemRectangles.Add(new Rectangle(xLoc, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[1] += (tpItemsPosAndSize[2] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.TextAnd16x16_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = tpItemsPosAndSize[1];

                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    tpItemsPosAndSize[2] = txtSize.Width + 16;
                                    tpItemsPosAndSize[2] += (3 * tpItemsPosAndSize[4]);
                                    tpItemsPosAndSize[3] = Math.Max(txtSize.Height, 16);
                                    tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);

                                    int yLoc = rct.Bottom - tpItemsPosAndSize[3];
                                    tpItemRectangles.Add(new Rectangle(xLoc, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[1] += (tpItemsPosAndSize[2] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.TextAnd32x32_Image:
                                tpItemsPosAndSize[9] = 32;
                                goto case TabPageItemStyle.TextAnd48x48_Image;
                            case TabPageItemStyle.TextAnd48x48_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = tpItemsPosAndSize[1];

                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    tpItemsPosAndSize[2] = Math.Max(txtSize.Width, tpItemsPosAndSize[9]);
                                    tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                    tpItemsPosAndSize[3] = txtSize.Height + tpItemsPosAndSize[9];
                                    tpItemsPosAndSize[3] += (3 * tpItemsPosAndSize[4]);

                                    int yLoc = rct.Bottom - tpItemsPosAndSize[3];
                                    tpItemRectangles.Add(new Rectangle(xLoc, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[1] += (tpItemsPosAndSize[2] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                        }
                        // left padding.
                        tpItemsPosAndSize[5] = renderer.TabPageAreaCornerOffset.Left;
                        // top padding.
                        tpItemsPosAndSize[6] = renderer.TabPageAreaCornerOffset.Top;
                        // right padding.
                        tpItemsPosAndSize[7] = renderer.TabPageAreaCornerOffset.Right;
                        // bottom padding.
                        tpItemsPosAndSize[8] = renderer.TabPageAreaCornerOffset.Bottom + tpItemsPosAndSize[3] + tpItemsPosAndSize[0];
                        break;
                    case TabPageLayout.Left:
                        tpItemsPosAndSize[1] = renderer.TabPageItemsAreaOffset.Right;
                        tpItemsPosAndSize[0] = renderer.TabPageItemsAreaOffset.Top;
                        switch (renderer.NeoTabPageItemsStyle)
                        {
                            case TabPageItemStyle.OnlyText:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    // Maximum Width.
                                    tpItemsPosAndSize[2] = Math.Max(txtSize.Width, tpItemsPosAndSize[2]);
                                    // Maximum Height.
                                    tpItemsPosAndSize[3] = Math.Max(txtSize.Height, tpItemsPosAndSize[3]);
                                }
                                tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int yLoc = tpItemsPosAndSize[0];
                                    tpItemRectangles.Add(new Rectangle(0, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[0] += (tpItemsPosAndSize[3] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.Only16x16_Image:
                                tpItemsPosAndSize[9] = 16;
                                goto case TabPageItemStyle.Only48x48_Image;
                            case TabPageItemStyle.Only32x32_Image:
                                tpItemsPosAndSize[9] = 32;
                                goto case TabPageItemStyle.Only48x48_Image;
                            case TabPageItemStyle.Only48x48_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int yLoc = tpItemsPosAndSize[0];

                                    tpItemsPosAndSize[2] = tpItemsPosAndSize[9];
                                    tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                    tpItemsPosAndSize[3] = tpItemsPosAndSize[9];
                                    tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);
                                    tpItemRectangles.Add(new Rectangle(0, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[0] += (tpItemsPosAndSize[3] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.TextAnd16x16_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    // Maximum Width.
                                    tpItemsPosAndSize[2] = Math.Max(txtSize.Width + 16, tpItemsPosAndSize[2]);
                                    // Maximum Height.
                                    tpItemsPosAndSize[3] = Math.Max(txtSize.Height, 16);
                                }
                                tpItemsPosAndSize[2] += (3 * tpItemsPosAndSize[4]);
                                tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int yLoc = tpItemsPosAndSize[0];
                                    tpItemRectangles.Add(new Rectangle(0, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[0] += (tpItemsPosAndSize[3] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.TextAnd32x32_Image:
                                tpItemsPosAndSize[9] = 32;
                                goto case TabPageItemStyle.TextAnd48x48_Image;
                            case TabPageItemStyle.TextAnd48x48_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    // Maximum Width.
                                    tpItemsPosAndSize[2] = Math.Max(txtSize.Width, Math.Max(tpItemsPosAndSize[9], tpItemsPosAndSize[2]));//burası
                                    // Maximum Height.
                                    tpItemsPosAndSize[3] = Math.Max(txtSize.Height + tpItemsPosAndSize[9], tpItemsPosAndSize[3]);
                                }
                                tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                tpItemsPosAndSize[3] += (3 * tpItemsPosAndSize[4]);
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int yLoc = tpItemsPosAndSize[0];
                                    tpItemRectangles.Add(new Rectangle(0, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[0] += (tpItemsPosAndSize[3] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                        }
                        // left padding.
                        tpItemsPosAndSize[5] = renderer.TabPageAreaCornerOffset.Left + tpItemsPosAndSize[2] + tpItemsPosAndSize[1];
                        // top padding.
                        tpItemsPosAndSize[6] = renderer.TabPageAreaCornerOffset.Top;
                        // right padding.
                        tpItemsPosAndSize[7] = renderer.TabPageAreaCornerOffset.Right;
                        // bottom padding.
                        tpItemsPosAndSize[8] = renderer.TabPageAreaCornerOffset.Bottom;
                        break;
                    case TabPageLayout.Right:
                        tpItemsPosAndSize[1] = renderer.TabPageItemsAreaOffset.Left;
                        tpItemsPosAndSize[0] = renderer.TabPageItemsAreaOffset.Top;
                        switch (renderer.NeoTabPageItemsStyle)
                        {
                            case TabPageItemStyle.OnlyText:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    // Maximum Width.
                                    tpItemsPosAndSize[2] = Math.Max(txtSize.Width, tpItemsPosAndSize[2]);
                                    // Maximum Height.
                                    tpItemsPosAndSize[3] = Math.Max(txtSize.Height, tpItemsPosAndSize[3]);
                                }
                                tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = rct.Right - tpItemsPosAndSize[2], yLoc = tpItemsPosAndSize[0];
                                    tpItemRectangles.Add(new Rectangle(xLoc, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[0] += (tpItemsPosAndSize[3] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.Only16x16_Image:
                                tpItemsPosAndSize[9] = 16;
                                goto case TabPageItemStyle.Only48x48_Image;
                            case TabPageItemStyle.Only32x32_Image:
                                tpItemsPosAndSize[9] = 32;
                                goto case TabPageItemStyle.Only48x48_Image;
                            case TabPageItemStyle.Only48x48_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    tpItemsPosAndSize[2] = tpItemsPosAndSize[9];
                                    tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                    tpItemsPosAndSize[3] = tpItemsPosAndSize[9];
                                    tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);

                                    int xLoc = rct.Right - tpItemsPosAndSize[2], yLoc = tpItemsPosAndSize[0];
                                    tpItemRectangles.Add(new Rectangle(xLoc, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[0] += (tpItemsPosAndSize[3] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.TextAnd16x16_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    // Maximum Width.
                                    tpItemsPosAndSize[2] = Math.Max(txtSize.Width + 16, tpItemsPosAndSize[2]);
                                    // Maximum Height.
                                    tpItemsPosAndSize[3] = Math.Max(txtSize.Height, 16);
                                }
                                tpItemsPosAndSize[2] += (3 * tpItemsPosAndSize[4]);
                                tpItemsPosAndSize[3] += (2 * tpItemsPosAndSize[4]);
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = rct.Right - tpItemsPosAndSize[2], yLoc = tpItemsPosAndSize[0];
                                    tpItemRectangles.Add(new Rectangle(xLoc, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[0] += (tpItemsPosAndSize[3] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                            case TabPageItemStyle.TextAnd32x32_Image:
                                tpItemsPosAndSize[9] = 32;
                                goto case TabPageItemStyle.TextAnd48x48_Image;
                            case TabPageItemStyle.TextAnd48x48_Image:
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    // Text size of the current item.
                                    Size txtSize = g.MeasureString(tabPage.Text, renderer.NeoTabPageItemsFont, Int32.MaxValue).ToSize();
                                    // Maximum Width.
                                    tpItemsPosAndSize[2] = Math.Max(txtSize.Width, Math.Max(tpItemsPosAndSize[9], tpItemsPosAndSize[2]));
                                    // Maximum Height.
                                    tpItemsPosAndSize[3] = Math.Max(txtSize.Height + tpItemsPosAndSize[9], tpItemsPosAndSize[3]);
                                }
                                tpItemsPosAndSize[2] += (2 * tpItemsPosAndSize[4]);
                                tpItemsPosAndSize[3] += (3 * tpItemsPosAndSize[4]);
                                foreach (NeoTabPage tabPage in this.Controls)
                                {
                                    int xLoc = rct.Right - tpItemsPosAndSize[2], yLoc = tpItemsPosAndSize[0];
                                    tpItemRectangles.Add(new Rectangle(xLoc, yLoc, tpItemsPosAndSize[2], tpItemsPosAndSize[3]),
                                        CommonObjects.ButtonState.Normal);

                                    tpItemsPosAndSize[0] += (tpItemsPosAndSize[3] + renderer.TabPageItemsBetweenSpacing);
                                }
                                break;
                        }
                        // left padding.
                        tpItemsPosAndSize[5] = renderer.TabPageAreaCornerOffset.Left;
                        // top padding.
                        tpItemsPosAndSize[6] = renderer.TabPageAreaCornerOffset.Top;
                        // right padding.
                        tpItemsPosAndSize[7] = renderer.TabPageAreaCornerOffset.Right + tpItemsPosAndSize[2] + tpItemsPosAndSize[1];
                        // bottom padding.
                        tpItemsPosAndSize[8] = renderer.TabPageAreaCornerOffset.Bottom;
                        break;
                }
            }

            this.Padding = new Padding(tpItemsPosAndSize[5], tpItemsPosAndSize[6], tpItemsPosAndSize[7], tpItemsPosAndSize[8]);
        }

        /// <summary>
        /// Re-build the smart button rectangles.
        /// </summary>
        private void RebuildSmartButtons()
        {
            Rectangle tabPageAreaRct = DisplayRectangle;
            tabPageAreaRct.X -= renderer.TabPageAreaCornerOffset.Left;
            tabPageAreaRct.Y -= renderer.TabPageAreaCornerOffset.Top;
            tabPageAreaRct.Width += (renderer.TabPageAreaCornerOffset.Left + renderer.TabPageAreaCornerOffset.Right);
            tabPageAreaRct.Height += (renderer.TabPageAreaCornerOffset.Top + renderer.TabPageAreaCornerOffset.Bottom);
            // Removes all items in the collection.
            smartButtonRectangles.Clear();
            Point loc = Point.Empty;
            switch (renderer.NeoTabPageItemsSide)
            {
                case TabPageLayout.Top:
                    loc.X = tabPageAreaRct.Right - renderer.SmartButtonsAreaOffset.Right - renderer.SmartButtonsSize.Width;
                    loc.Y = tabPageAreaRct.Top - renderer.SmartButtonsAreaOffset.Bottom - renderer.SmartButtonsSize.Height;
                    if (renderer.IsSupportSmartCloseButton)
                    {
                        smartButtonRectangles.Add(new Rectangle(loc, renderer.SmartButtonsSize), CommonObjects.ButtonState.Normal);
                        loc.X -= (renderer.SmartButtonsBetweenSpacing + renderer.SmartButtonsSize.Width);
                    }
                    else
                    {
                        smartButtonRectangles.Add(Rectangle.Empty, CommonObjects.ButtonState.Disabled);
                    }
                    if (renderer.IsSupportSmartDropDownButton)
                    {
                        smartButtonRectangles.Add(new Rectangle(loc, renderer.SmartButtonsSize), CommonObjects.ButtonState.Normal);
                    }
                    break;
                case TabPageLayout.Bottom:
                    loc.X = tabPageAreaRct.Right - renderer.SmartButtonsAreaOffset.Right - renderer.SmartButtonsSize.Width;
                    loc.Y = tabPageAreaRct.Bottom + renderer.SmartButtonsAreaOffset.Top;
                    if (renderer.IsSupportSmartCloseButton)
                    {
                        smartButtonRectangles.Add(new Rectangle(loc, renderer.SmartButtonsSize), CommonObjects.ButtonState.Normal);
                        loc.X -= (renderer.SmartButtonsBetweenSpacing + renderer.SmartButtonsSize.Width);
                    }
                    else
                    {
                        smartButtonRectangles.Add(Rectangle.Empty, CommonObjects.ButtonState.Disabled);
                    }
                    if (renderer.IsSupportSmartDropDownButton)
                    {
                        smartButtonRectangles.Add(new Rectangle(loc, renderer.SmartButtonsSize), CommonObjects.ButtonState.Normal);
                    }
                    break;
                case TabPageLayout.Left:
                    loc.X = tabPageAreaRct.Left - renderer.SmartButtonsAreaOffset.Right - renderer.SmartButtonsSize.Width;
                    loc.Y = tabPageAreaRct.Bottom - renderer.SmartButtonsAreaOffset.Bottom - renderer.SmartButtonsSize.Height;
                    if (renderer.IsSupportSmartCloseButton)
                    {
                        smartButtonRectangles.Add(new Rectangle(loc, renderer.SmartButtonsSize), CommonObjects.ButtonState.Normal);
                        loc.Y -= (renderer.SmartButtonsBetweenSpacing + renderer.SmartButtonsSize.Height);
                    }
                    else
                    {
                        smartButtonRectangles.Add(Rectangle.Empty, CommonObjects.ButtonState.Disabled);
                    }
                    if (renderer.IsSupportSmartDropDownButton)
                    {
                        smartButtonRectangles.Add(new Rectangle(loc, renderer.SmartButtonsSize), CommonObjects.ButtonState.Normal);
                    }
                    break;
                case TabPageLayout.Right:
                    loc.X = tabPageAreaRct.Right + renderer.SmartButtonsAreaOffset.Left;
                    loc.Y = tabPageAreaRct.Bottom - renderer.SmartButtonsAreaOffset.Bottom - renderer.SmartButtonsSize.Height;
                    if (renderer.IsSupportSmartCloseButton)
                    {
                        smartButtonRectangles.Add(new Rectangle(loc, renderer.SmartButtonsSize), CommonObjects.ButtonState.Normal);
                        loc.Y -= (renderer.SmartButtonsBetweenSpacing + renderer.SmartButtonsSize.Height);
                    }
                    else
                    {
                        smartButtonRectangles.Add(Rectangle.Empty, CommonObjects.ButtonState.Disabled);
                    }
                    if (renderer.IsSupportSmartDropDownButton)
                    {
                        smartButtonRectangles.Add(new Rectangle(loc, renderer.SmartButtonsSize), CommonObjects.ButtonState.Normal);
                    }
                    break;
            }
        }

        /// <summary>
        /// Selects a tab page with a specified index of the tab.
        /// </summary>
        /// <param name="tabPageIndex">Index of the tab to select</param>
        private void OnNavigateTabPage(int tabPageIndex)
        {
            if (tabPageIndex == selectedIndex || this.Controls.Count <= 1)
                return;
            if (tabPageIndex > this.Controls.Count - 1)
                tabPageIndex = 0;
            else if (tabPageIndex < 0)
                tabPageIndex = this.Controls.Count - 1;
            try
            {
                NeoTabPage selectingTabPage = TabPages[tabPageIndex] as NeoTabPage;
                if (selectingTabPage == null || !selectingTabPage.IsSelectable)
                    return;
                using (SelectedIndexChangingEventArgs e =
                    new SelectedIndexChangingEventArgs(selectingTabPage, tabPageIndex))
                {
                    // Fire a Notification Event.
                    OnSelectedIndexChanging(e);

                    if (!e.Cancel)
                        this.SelectedIndex = e.TabPageIndex;
                }
            }
            catch (ArgumentOutOfRangeException)
            { ;}
        }

        /// <summary>
        /// Selects next available tab page from the collection.
        /// </summary>
        private void SelectNextAvailableTabPage()
        {
            switch (this.Controls.Count)
            {
                case 0:
                    selectedIndex = -1;
                    break;
                default:
                    bool success = false;
                    for (int i = 0; i < this.Controls.Count; i++)
                    {
                        NeoTabPage tp = TabPages[i] as NeoTabPage;
                        if (!tp.IsSelectable)
                            continue;
                        if (i != selectedIndex)
                            OnNavigateTabPage(i);
                        else
                        {
                            tp.Visible = true;
                            Invalidate();
                            Update();
                        }
                        // If selection is succeed, exit from the loop.
                        if (selectedIndex == i)
                        {
                            success = true;
                            break;
                        }
                    }
                    if (!success)
                    {
                        TabPages[0].Visible = true;
                        selectedIndex = 0;
                        Invalidate();
                        Update();
                    }
                    break;
            }
        }

        private void ShowTooltip(NeoTabPage tabItem,
            Rectangle itemRectangle)
        {
            try
            {
                if (myTooltipForm == null)
                {
                    myTooltipForm = new ToolTips(tabItem.Text, tabItem.ToolTipText, tabItem.ProgressUsedValue, barMaxValue);
                    myTooltipForm.TaskCompleted += (sender, e) =>
                        {
                            int tpIndex = -1;
                            Rectangle tpRectangle;
                            CommonObjects.ButtonState tpState;
                            NeoTabPage tp = GetHitTest(queueTooltipPoint,
                                out tpRectangle, out tpState, out tpIndex);
                            if (tp != null && tp.Text != myTooltipForm.TITLE)
                            {
                                myTooltipForm.TITLE = tp.Text;
                                myTooltipForm.DESCRIPTION = tp.ToolTipText;
                                myTooltipForm.VALUE = tp.ProgressUsedValue;
                                myTooltipForm.MAXVALUE = barMaxValue;
                                switch (renderer.NeoTabPageItemsSide)
                                {
                                    default:
                                        myTooltipForm.Location = PointToScreen(new Point(tpRectangle.Right + 2,
                                            tpRectangle.Bottom + 2));
                                        break;
                                    case TabPageLayout.Bottom:
                                        myTooltipForm.Location = PointToScreen(new Point(tpRectangle.Right + 2,
                                            tpRectangle.Top - 2 - myTooltipForm.Height));
                                        break;
                                    case TabPageLayout.Right:
                                        myTooltipForm.Location = PointToScreen(new Point(tpRectangle.Left - 2 - myTooltipForm.Width,
                                            tpRectangle.Bottom + 2));
                                        break;
                                }
                                myTooltipForm.StartAsyncTooltip();
                            }
                            else
                            {
                                myTooltipForm.Close();
                                myTooltipForm = null;
                            }
                        };
                }
                if (myTooltipForm.Status != ToolTips.StatusState.InProgress)
                {
                    myTooltipForm.TooltipRenderer = tooltipRenderer;
                    switch (renderer.NeoTabPageItemsSide)
                    {
                        default:
                            myTooltipForm.Location = PointToScreen(new Point(itemRectangle.Right + 2,
                                itemRectangle.Bottom + 2));
                            break;
                        case TabPageLayout.Bottom:
                            myTooltipForm.Location = PointToScreen(new Point(itemRectangle.Right + 2,
                                itemRectangle.Top - 2 - myTooltipForm.Height));
                            break;
                        case TabPageLayout.Right:
                            myTooltipForm.Location = PointToScreen(new Point(itemRectangle.Left - 2 - myTooltipForm.Width,
                                itemRectangle.Bottom + 2));
                            break;
                    }
                    myTooltipForm.StartAsyncTooltip();
                    myTooltipForm.Show(this);
                }
            }
            catch { ; }
        }

        private void BeginDragDrop(NeoTabPage tabItem,
            Rectangle itemRectangle,
            CommonObjects.ButtonState itemState,
            int itemIndex)
        {
            if (draggingStyle == DragDropStyle.PageEffect)
            {
                Size ps = DisplayRectangle.Size;
                if (ps.Width > 180)
                    ps.Width = 180;
                if (ps.Height > 180)
                    ps.Height = 180;
                using (Bitmap bm = new Bitmap(ps.Width, ps.Height))
                {
                    myDdCursor = new DDPaintCursor();
                    using (Graphics gr = Graphics.FromImage(bm))
                    {
                        Point pt = PointToScreen(DisplayRectangle.Location);
                        gr.CopyFromScreen(pt.X, pt.Y,
                            0, 0, ps);
                    }
                    myDdCursor.DrawDDCursor(bm);
                }
            }
            else
            {
                myDdCursor = new DDPaintCursor(renderer);
                myDdCursor.DrawDDCursor(itemRectangle,
                    String.IsNullOrEmpty(tabItem.Text) ? tabItem.Name : tabItem.Text,
                    itemIndex, itemState);
            }
            DoDragDrop(tabItem, DragDropEffects.Move);
        }

        #endregion

        #region General Methods

        /// <summary>
        /// Removes the specified NeoTabPage control from the control collection.
        /// </summary>
        /// <param name="toBeRemoved">to be removed tab page control</param>
        public void Remove(NeoTabPage toBeRemoved)
        {
            if (toBeRemoved == null)
                return;
            int toBeRemovedIndex = TabPages.IndexOf(toBeRemoved);
            if (toBeRemovedIndex != -1)
                RemoveAt(toBeRemovedIndex);
        }

        /// <summary>
        /// Removes a NeoTabPage control from the control collection at the specified indexed location if it supports removing.
        /// </summary>
        /// <param name="toBeRemovedIndex">to be removed tab page index</param>
        public void RemoveAt(int toBeRemovedIndex)
        {
            // Check to see if there is an item at the supplied index.
            if ((toBeRemovedIndex >= TabPages.Count) || (toBeRemovedIndex < 0))
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                NeoTabPage tp = TabPages[toBeRemovedIndex] as NeoTabPage;
                if (tp.IsCloseable)
                {
                    using (TabPageRemovingEventArgs e =
                        new TabPageRemovingEventArgs(tp, toBeRemovedIndex))
                    {
                        OnTabPageRemoving(e);
                        if (!e.Cancel)
                        {
                            TabPages.RemoveAt(toBeRemovedIndex);
                            using (TabPageRemovedEventArgs ea =
                                new TabPageRemovedEventArgs(tp, toBeRemovedIndex))
                            {
                                OnTabPageRemoved(ea);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The result value is: 0 ( SmartCloseButton ), 1 ( SmartDropDownButton ), -1 ( Not Found ).
        /// </summary>
        /// <param name="pt">Mouse point coordinate</param>
        /// <param name="rectangle">Smart button rectangle</param>
        /// <param name="state">Smart button state</param>
        /// <param name="result">-1, 0 or 1.</param>
        public void GetSmartButtonHitTest(Point pt, out Rectangle rectangle,
            out CommonObjects.ButtonState state, out int result)
        {
            int i = -1;
            rectangle = Rectangle.Empty;
            foreach (KeyValuePair<Rectangle, CommonObjects.ButtonState> current in smartButtonRectangles)
            {
                i++;
                rectangle = current.Key;
                if (!rectangle.Contains(pt))
                    continue;

                state = current.Value;
                result = i;
                return;
            }
            state = CommonObjects.ButtonState.Disabled;
            result = -1;
        }

        /// <summary>
        /// Gets the NeoTabPage control at the specified point if it exists in the collection.
        /// </summary>
        /// <param name="pt">Mouse point coordinate</param>
        /// <param name="rectangle">NeoTabPage item rectangle</param>
        /// <param name="state">NeoTabPage item button state</param>
        /// <param name="tabPageIndex">The index of the tab page</param>
        /// <returns>It returns an existing NeoTabPage control if the collection contains the tab page at the specified point; otherwise, null.</returns>
        public NeoTabPage GetHitTest(Point pt, out Rectangle rectangle,
            out CommonObjects.ButtonState state, out int tabPageIndex)
        {
            int i = -1;
            foreach (KeyValuePair<Rectangle, CommonObjects.ButtonState> current in tpItemRectangles)
            {
                i++;

                if (!current.Key.Contains(pt))
                    continue;

                tabPageIndex = i;
                state = current.Value;
                rectangle = current.Key;
                NeoTabPage RetVal = TabPages[i] as NeoTabPage;
                return RetVal;
            }

            tabPageIndex = -1;
            state = CommonObjects.ButtonState.Normal;
            rectangle = Rectangle.Empty;
            return null;
        }

        /// <summary>
        /// Allows you to show or hide an existing tab page items.
        /// </summary>
        public void ShowTabManager()
        {
            using (TabShowHideManager manager =
                new TabShowHideManager())
            {
                /*** Add enabled items to the list. ***/
                foreach (NeoTabPage tp in this.Controls)
                {
                    if (tp.IsCloseable)
                        manager.AddNewItem(tp, true);
                }
                /*** Add hided items to the list. ***/
                foreach (NeoTabPage tp in this.hidedMembers)
                    manager.AddNewItem(tp, false);
                /*** If clicked on the apply button. ***/
                if (manager.ShowDialog()
                    == DialogResult.OK)
                {
                    foreach (NeoTabPage tp in manager.SelectedItems)
                    {
                        if (!this.Controls.Contains(tp))
                            this.Controls.Add(tp);
                    }
                    hidedMembers.Clear();
                    foreach (NeoTabPage tp in manager.UnSelectedItems)
                    {
                        if (this.Controls.Contains(tp))
                            this.Controls.Remove(tp);
                        hidedMembers.Add(tp);
                    }
                }
            }
        }

        /// <summary>
        /// Shows Add-in Manager Form.
        /// </summary>
        public void ShowAddInRendererManager()
        {
            using (AddInRendererManager manager = new AddInRendererManager(typeof(DefaultRenderer)))
            {
                manager.Renderer = this.Renderer;
                if (manager.ShowDialog() == DialogResult.OK)
                {
                    if (manager.Renderer != null)
                    {
                        Type type = manager.Renderer.GetType();
                        if (!type.Equals(this.Renderer.GetType()))
                        {
                            this.Renderer = manager.Renderer;
                            this.RendererName = type.Name;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shows the editor form of the current renderer, if it supports.
        /// </summary>
        public void ShowAddInRendererEditor()
        {
            try
            {
                this.Renderer.InvokeEditor();
            }
            catch (NotImplementedException)
            {
                MessageBox.Show("Editor support is not implemented for this renderer.", "NeoTabControl Library");
            }
        }

        #endregion

        #region ISupportInitialize Members

        private bool initializing = false;
        void ISupportInitialize.BeginInit()
        {
            // Do nothing.
        }
        void ISupportInitialize.EndInit()
        {
            initializing = true;
            RebuildControl();
            if (tpItemRectangles.Count > 0 &&
                (renderer.IsSupportSmartCloseButton || renderer.IsSupportSmartDropDownButton))
                RebuildSmartButtons();
            UpdateStyles();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            NeoTabWindow toBeCloned = CustomControlsLogic.GetMyClone(this) as NeoTabWindow;
            toBeCloned.AllowDrop = this.AllowDrop;
            ((ISupportInitialize)toBeCloned).EndInit();
            return toBeCloned;
        }

        #endregion
    }
}