using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NeoTabControlLibrary.CommonObjects;

namespace NeoTabControlLibrary
{
    public class AddInRendererManager : Form
    {
        #region Symbolic Constants

        private const string ADD_INS_FOLDER = "Add-ins";

        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Managed Resources
                foreach (Control control in this.Controls)
                    control.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.AddInsTable = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dataGridViewEditorButtonColumn1 = new NeoTabControlLibrary.DataGridViewEditorButtonColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Editor = new NeoTabControlLibrary.DataGridViewEditorButtonColumn();
            this.AvailableAddIns = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Developer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Empty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.AddInsTable)).BeginInit();
            this.SuspendLayout();
            // 
            // AddInsTable
            // 
            this.AddInsTable.AllowUserToAddRows = false;
            this.AddInsTable.AllowUserToDeleteRows = false;
            this.AddInsTable.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.AddInsTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.AddInsTable.BackgroundColor = System.Drawing.Color.White;
            this.AddInsTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AddInsTable.ColumnHeadersHeight = 21;
            this.AddInsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.AddInsTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Type,
            this.Editor,
            this.AvailableAddIns,
            this.Version,
            this.Developer,
            this.Empty});
            this.AddInsTable.GridColor = System.Drawing.Color.LightGray;
            this.AddInsTable.Location = new System.Drawing.Point(12, 12);
            this.AddInsTable.MultiSelect = false;
            this.AddInsTable.Name = "AddInsTable";
            this.AddInsTable.ReadOnly = true;
            this.AddInsTable.RowHeadersVisible = false;
            this.AddInsTable.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.AddInsTable.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.AddInsTable.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Gainsboro;
            this.AddInsTable.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.AddInsTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.AddInsTable.Size = new System.Drawing.Size(547, 176);
            this.AddInsTable.TabIndex = 0;
            this.AddInsTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.AddInsTable_CellContentClick);
            this.AddInsTable.SelectionChanged += new System.EventHandler(this.AddInsTable_SelectionChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 198);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.BackColor = System.Drawing.SystemColors.Control;
            this.txtDescription.Location = new System.Drawing.Point(12, 216);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(547, 81);
            this.txtDescription.TabIndex = 2;
            this.txtDescription.Text = "some description text.";
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(355, 309);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(99, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(460, 309);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(99, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // dataGridViewEditorButtonColumn1
            // 
            this.dataGridViewEditorButtonColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridViewEditorButtonColumn1.HeaderText = "";
            this.dataGridViewEditorButtonColumn1.Name = "dataGridViewEditorButtonColumn1";
            this.dataGridViewEditorButtonColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewEditorButtonColumn1.UseColumnTextForButtonValue = true;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.Visible = false;
            // 
            // Editor
            // 
            this.Editor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.Editor.HeaderText = "!";
            this.Editor.Name = "Editor";
            this.Editor.ReadOnly = true;
            this.Editor.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Editor.Text = "";
            this.Editor.Width = 5;
            // 
            // AvailableAddIns
            // 
            this.AvailableAddIns.FillWeight = 284.7716F;
            this.AvailableAddIns.HeaderText = "Available Add-ins";
            this.AvailableAddIns.Name = "AvailableAddIns";
            this.AvailableAddIns.ReadOnly = true;
            this.AvailableAddIns.Width = 200;
            // 
            // Version
            // 
            this.Version.FillWeight = 7.614212F;
            this.Version.HeaderText = "Version";
            this.Version.Name = "Version";
            this.Version.ReadOnly = true;
            this.Version.Width = 120;
            // 
            // Developer
            // 
            this.Developer.FillWeight = 7.614212F;
            this.Developer.HeaderText = "Developer";
            this.Developer.Name = "Developer";
            this.Developer.ReadOnly = true;
            this.Developer.Width = 140;
            // 
            // Empty
            // 
            this.Empty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Empty.HeaderText = "";
            this.Empty.Name = "Empty";
            this.Empty.ReadOnly = true;
            // 
            // AddInRendererManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 344);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AddInsTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddInRendererManager";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add-in Manager";
            this.Load += new System.EventHandler(this.AddInRendererManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AddInsTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView AddInsTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnApply;
        private DataGridViewEditorButtonColumn dataGridViewEditorButtonColumn1;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewEditorButtonColumn Editor;
        private DataGridViewTextBoxColumn AvailableAddIns;
        private DataGridViewTextBoxColumn Version;
        private DataGridViewTextBoxColumn Developer;
        private DataGridViewTextBoxColumn Empty;
        private System.Windows.Forms.Button btnCancel;

        #region Constructor

        public AddInRendererManager()
        {
            InitializeComponent();
        }

        public AddInRendererManager(Type defaultRendererType)
            : this()
        {
            this.DefaultRendererType = defaultRendererType;
        }

        #endregion

        #region Destructor

        ~AddInRendererManager()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets the renderer class of the NeoTabWindow control.
        /// </summary>
        public RendererBase Renderer { get; set; }

        /// <summary>
        /// Gets the default renderer type of the NeoTabWindow control.
        /// </summary>
        public Type DefaultRendererType { get; private set; }

        #endregion

        #region Helper Methods - Event Handlers

        private void GetAvailableAddIns()
        {
            AddInsTable.Rows.Clear();
            try
            {
                // Add default tab control renderer class to the collection.
                AddInRendererAttribute att = GetRendererAttributeFromType(DefaultRendererType);
                if (att != null)
                {
                    txtDescription.Text = att.Description;
                    DataGridViewRow row = new DataGridViewRow();
                    row.DefaultCellStyle = AddInsTable.RowTemplate.DefaultCellStyle;
                    DataGridViewCell[] dataCells = new DataGridViewCell[]
                    {
                        new DataGridViewTextBoxCell(){Value = DefaultRendererType.Equals(Renderer.GetType()) ? Renderer : new DefaultRenderer()},
                        new DataGridViewEditorButtonCell(){Value = "...", 
                            ToolTipText = att.IsSupportEditor ? "Show Editor" : "Not Supported", Enabled = att.IsSupportEditor},
                        new DataGridViewTextBoxCell(){Value = att.Name},
                        new DataGridViewTextBoxCell(){Value = att.VersionNumber},
                        new DataGridViewTextBoxCell(){Value = att.DeveloperName}
                    };
                    row.Cells.AddRange(dataCells);
                    AddInsTable.Rows.Add(row);
                }
                // This constructor does not check if a directory exists.
                DirectoryInfo directory = new DirectoryInfo(ADD_INS_FOLDER);
                // Gets all the Add-in directories in the current directory(ADD_INS_FOLDER).
                DirectoryInfo[] directories = directory.GetDirectories();
                foreach (DirectoryInfo currentDirectory in directories)
                {
                    FileInfo[] files = currentDirectory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo addInFile in files)
                    {
                        if (addInFile.Extension != ".dll")
                            continue;
                        Assembly assembly = Assembly.LoadFrom(addInFile.FullName);
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            att = GetRendererAttributeFromType(type);
                            // If it's a valid type add it to the renderer collection table.
                            if (att != null && type.IsPublic && type.IsSealed && type.IsClass &&
                                type.BaseType.Equals(typeof(RendererBase)))
                            {
                                DataGridViewRow row = new DataGridViewRow();
                                row.DefaultCellStyle = AddInsTable.RowTemplate.DefaultCellStyle;
                                DataGridViewCell[] dataCells = new DataGridViewCell[]
                                {
                                    new DataGridViewTextBoxCell(){Value = type.Equals(Renderer.GetType()) ? Renderer : Activator.CreateInstance(type)},
                                    new DataGridViewEditorButtonCell(){Value = "...", 
                                        ToolTipText = att.IsSupportEditor ? "Show Editor" : "Not Supported", Enabled = att.IsSupportEditor},
                                    new DataGridViewTextBoxCell(){Value = att.Name},
                                    new DataGridViewTextBoxCell(){Value = att.VersionNumber},
                                    new DataGridViewTextBoxCell(){Value = att.DeveloperName}
                                };
                                row.Cells.AddRange(dataCells);
                                AddInsTable.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (ArgumentNullException)
            {
                this.txtDescription.Text = "ADD_INS_FOLDER cannot be null string, please enter a valid value.";
            }
            catch (System.Security.SecurityException)
            {
                this.txtDescription.Text = "Caller does not have the required permission.";
            }
            catch (ArgumentException)
            {
                this.txtDescription.Text = "ADD_INS_FOLDER is an empty string, " +
                    "contains only white spaces, " +
                    "or contains invalid characters.";
            }
            catch (DirectoryNotFoundException)
            {
                this.txtDescription.Text = String.Format("{0} directory not found.", ADD_INS_FOLDER);
            }
            finally
            {
                this.Text += String.Format(" - {0} renderer class found.", AddInsTable.Rows.Count);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = AddInsTable.SelectedRows;
            DataGridViewRow selectedItem = selectedRows[0];
            // Gets the control renderer class from the first column of the selected item.
            this.Renderer = (RendererBase)selectedItem.Cells[0].Value;
        }

        private void AddInRendererManager_Load(object sender, EventArgs e)
        {
            GetAvailableAddIns();
        }

        private void AddInsTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 || e.ColumnIndex != AddInsTable.Columns["Editor"].Index)
                return;
            // Retrieve the renderer object.
            RendererBase renderer = (RendererBase)AddInsTable[0, e.RowIndex].Value;
            AddInRendererAttribute att = GetRendererAttributeFromType(renderer.GetType());
            if (att != null && att.IsSupportEditor)
            {
                renderer.InvokeEditor();
            }
        }

        private void AddInsTable_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection selectedRows = AddInsTable.SelectedRows;
            DataGridViewRow selectedItem = selectedRows[0];
            // Gets the description information from the first column of the selected item.
            AddInRendererAttribute att = GetRendererAttributeFromType(selectedItem.Cells[0].Value.GetType());
            if (att != null)
                txtDescription.Text = att.Description;
        }

        private static AddInRendererAttribute GetRendererAttributeFromType(Type type)
        {
            AddInRendererAttribute attribute = (AddInRendererAttribute)Attribute.GetCustomAttribute(type, typeof(AddInRendererAttribute));
            return attribute;
        }

        #endregion
        
        #region General Methods

        /// <summary>
        /// Loads a specific control renderer from a given renderer name.
        /// </summary>
        /// <param name="typeName">type name of the renderer class</param>
        /// <returns>It returns a renderer class from a specified type name.</returns>
        public static RendererBase LoadRenderer(string typeName)
        {
            RendererBase renderer = null;
            try
            {
                // This constructor does not check if a directory exists.
                DirectoryInfo directory = new DirectoryInfo(ADD_INS_FOLDER);
                // Gets all the Add-in directories in the current directory(ADD_INS_FOLDER).
                DirectoryInfo[] directories = directory.GetDirectories();
                foreach (DirectoryInfo currentDirectory in directories)
                {
                    FileInfo[] files = currentDirectory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo addInFile in files)
                    {
                        if (addInFile.Extension != ".dll")
                            continue;
                        Assembly assembly = Assembly.LoadFrom(addInFile.FullName);
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (!type.Name.Equals(typeName))
                                continue;
                            AddInRendererAttribute att = GetRendererAttributeFromType(type);
                            // If it's a valid type add it to the renderer collection table.
                            if (att != null && type.IsPublic && type.IsSealed && type.IsClass &&
                                type.BaseType.Equals(typeof(RendererBase)))
                                return Activator.CreateInstance(type) as RendererBase;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return renderer;
        }

        #endregion
    }
}