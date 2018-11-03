namespace HydroDesktop.Plugins.MetadataFetcher.Forms
{
	partial class AddServicesForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose ( bool disposing )
		{
			if ( disposing && (components != null) )
			{
				components.Dispose ();
			}
			base.Dispose ( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddServicesForm));
            this.gbxProgress = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.prgMain = new System.Windows.Forms.ProgressBar();
            this.gbxServices = new System.Windows.Forms.GroupBox();
            this.dgvAddServices = new System.Windows.Forms.DataGridView();
            this.dgcServiceTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcServiceUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcServiceCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcCitation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcAbstract = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcDescriptionUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcContactName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgcContactEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.bgwMain = new System.ComponentModel.BackgroundWorker();
            this.gbxUpdate = new System.Windows.Forms.GroupBox();
            this.btnCheckExisting = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCheckForValidService = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblWebsite = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblContact = new System.Windows.Forms.Label();
            this.txtContact = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtWebsite = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblCode = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lblCitation = new System.Windows.Forms.Label();
            this.txtCitation = new System.Windows.Forms.TextBox();
            this.lblAbstract = new System.Windows.Forms.Label();
            this.txtAbstract = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblUrl = new System.Windows.Forms.Label();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.tcAddService = new System.Windows.Forms.TabControl();
            this.tpAddSingleSvc = new System.Windows.Forms.TabPage();
            this.tpAddMultiSvcs = new System.Windows.Forms.TabPage();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbxProgress.SuspendLayout();
            this.gbxServices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAddServices)).BeginInit();
            this.gbxUpdate.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tcAddService.SuspendLayout();
            this.tpAddSingleSvc.SuspendLayout();
            this.tpAddMultiSvcs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxProgress
            // 
            this.gbxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxProgress.Controls.Add(this.btnCancel);
            this.gbxProgress.Controls.Add(this.prgMain);
            this.gbxProgress.Location = new System.Drawing.Point(12, 305);
            this.gbxProgress.Name = "gbxProgress";
            this.gbxProgress.Size = new System.Drawing.Size(588, 46);
            this.gbxProgress.TabIndex = 7;
            this.gbxProgress.TabStop = false;
            this.gbxProgress.Text = "Ready";
            this.gbxProgress.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(497, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 23);
            this.btnCancel.TabIndex = 28;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // prgMain
            // 
            this.prgMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgMain.Location = new System.Drawing.Point(9, 16);
            this.prgMain.Name = "prgMain";
            this.prgMain.Size = new System.Drawing.Size(482, 22);
            this.prgMain.TabIndex = 0;
            // 
            // gbxServices
            // 
            this.gbxServices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxServices.Controls.Add(this.dgvAddServices);
            this.gbxServices.Location = new System.Drawing.Point(5, 6);
            this.gbxServices.Name = "gbxServices";
            this.gbxServices.Size = new System.Drawing.Size(573, 234);
            this.gbxServices.TabIndex = 2;
            this.gbxServices.TabStop = false;
            this.gbxServices.Text = "Enter or import service information to add to the metadata cache database";
            // 
            // dgvAddServices
            // 
            this.dgvAddServices.AllowUserToResizeRows = false;
            this.dgvAddServices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAddServices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvAddServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAddServices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcServiceTitle,
            this.dgcServiceUrl,
            this.dgcServiceCode,
            this.dgcCitation,
            this.dgcAbstract,
            this.dgcDescriptionUrl,
            this.dgcContactName,
            this.dgcContactEmail});
            this.dgvAddServices.Location = new System.Drawing.Point(6, 19);
            this.dgvAddServices.Name = "dgvAddServices";
            this.dgvAddServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAddServices.Size = new System.Drawing.Size(561, 209);
            this.dgvAddServices.TabIndex = 5;
            // 
            // dgcServiceTitle
            // 
            this.dgcServiceTitle.HeaderText = "Title";
            this.dgcServiceTitle.Name = "dgcServiceTitle";
            this.dgcServiceTitle.ToolTipText = "(Required) A brief title for the web service";
            this.dgcServiceTitle.Width = 52;
            // 
            // dgcServiceUrl
            // 
            this.dgcServiceUrl.HeaderText = "URL";
            this.dgcServiceUrl.Name = "dgcServiceUrl";
            this.dgcServiceUrl.ToolTipText = "(Required) The base URL of the web service. The URL does not need the ?wsdl param" +
    "eter.";
            this.dgcServiceUrl.Width = 54;
            // 
            // dgcServiceCode
            // 
            this.dgcServiceCode.HeaderText = "Code";
            this.dgcServiceCode.Name = "dgcServiceCode";
            this.dgcServiceCode.ToolTipText = "(Optional) A unique text identifier for the service, typically used by services r" +
    "egistered by HIS Central";
            this.dgcServiceCode.Width = 57;
            // 
            // dgcCitation
            // 
            this.dgcCitation.HeaderText = "Citation";
            this.dgcCitation.Name = "dgcCitation";
            this.dgcCitation.ToolTipText = "(Optional) The text that should be used when citing the web service as a referenc" +
    "e";
            this.dgcCitation.Width = 67;
            // 
            // dgcAbstract
            // 
            this.dgcAbstract.HeaderText = "Abstract";
            this.dgcAbstract.Name = "dgcAbstract";
            this.dgcAbstract.ToolTipText = "(Optional) Describes the purpose, data, source, or other details about the web se" +
    "rvice";
            this.dgcAbstract.Width = 71;
            // 
            // dgcDescriptionUrl
            // 
            this.dgcDescriptionUrl.HeaderText = "Website";
            this.dgcDescriptionUrl.Name = "dgcDescriptionUrl";
            this.dgcDescriptionUrl.ToolTipText = "(Optional) URL of the website where the web service is documented";
            this.dgcDescriptionUrl.Width = 71;
            // 
            // dgcContactName
            // 
            this.dgcContactName.HeaderText = "Contact";
            this.dgcContactName.Name = "dgcContactName";
            this.dgcContactName.ToolTipText = "(Optional) Name of the person to contact when making inquiries about the web serv" +
    "ice";
            this.dgcContactName.Width = 69;
            // 
            // dgcContactEmail
            // 
            this.dgcContactEmail.HeaderText = "Email";
            this.dgcContactEmail.Name = "dgcContactEmail";
            this.dgcContactEmail.ToolTipText = "(Optional) Email address of the person to contact when making inquiries about the" +
    " web service";
            this.dgcContactEmail.Width = 57;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.Location = new System.Drawing.Point(469, 15);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(113, 23);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "&Update Database";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // bgwMain
            // 
            this.bgwMain.WorkerReportsProgress = true;
            this.bgwMain.WorkerSupportsCancellation = true;
            this.bgwMain.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwMain_DoWork);
            this.bgwMain.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwMain_ProgressChanged);
            this.bgwMain.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwMain_RunWorkerCompleted);
            // 
            // gbxUpdate
            // 
            this.gbxUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxUpdate.Controls.Add(this.btnCheckExisting);
            this.gbxUpdate.Controls.Add(this.btnUpdate);
            this.gbxUpdate.Location = new System.Drawing.Point(12, 305);
            this.gbxUpdate.Name = "gbxUpdate";
            this.gbxUpdate.Size = new System.Drawing.Size(592, 46);
            this.gbxUpdate.TabIndex = 1;
            this.gbxUpdate.TabStop = false;
            // 
            // btnCheckExisting
            // 
            this.btnCheckExisting.Enabled = false;
            this.btnCheckExisting.Location = new System.Drawing.Point(350, 15);
            this.btnCheckExisting.Name = "btnCheckExisting";
            this.btnCheckExisting.Size = new System.Drawing.Size(113, 23);
            this.btnCheckExisting.TabIndex = 3;
            this.btnCheckExisting.Text = "&Check Existing";
            this.btnCheckExisting.UseVisualStyleBackColor = true;
            this.btnCheckExisting.Click += new System.EventHandler(this.btnCheckExisting_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(613, 24);
            this.menuStrip1.TabIndex = 30;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromFileToolStripMenuItem});
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.importToolStripMenuItem.Text = "&Import";
            // 
            // fromFileToolStripMenuItem
            // 
            this.fromFileToolStripMenuItem.Name = "fromFileToolStripMenuItem";
            this.fromFileToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.fromFileToolStripMenuItem.Text = "From &File...";
            this.fromFileToolStripMenuItem.Click += new System.EventHandler(this.fromFileToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCheckForValidService});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // mnuCheckForValidService
            // 
            this.mnuCheckForValidService.CheckOnClick = true;
            this.mnuCheckForValidService.Name = "mnuCheckForValidService";
            this.mnuCheckForValidService.Size = new System.Drawing.Size(440, 22);
            this.mnuCheckForValidService.Text = "&Check for valid WaterOneFlow service before committing to database";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "CSV (Comma delimited) (*.csv)|*.csv";
            this.openFileDialog1.Title = "Open";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel3);
            this.groupBox1.Controls.Add(this.flowLayoutPanel2);
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(572, 238);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Enter service information to add to the metadata cache database";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.lblWebsite);
            this.flowLayoutPanel3.Controls.Add(this.txtEmail);
            this.flowLayoutPanel3.Controls.Add(this.lblContact);
            this.flowLayoutPanel3.Controls.Add(this.txtContact);
            this.flowLayoutPanel3.Controls.Add(this.lblEmail);
            this.flowLayoutPanel3.Controls.Add(this.txtWebsite);
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(285, 104);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(281, 126);
            this.flowLayoutPanel3.TabIndex = 19;
            // 
            // lblWebsite
            // 
            this.lblWebsite.AutoSize = true;
            this.lblWebsite.Location = new System.Drawing.Point(3, 0);
            this.lblWebsite.Name = "lblWebsite";
            this.lblWebsite.Size = new System.Drawing.Size(46, 13);
            this.lblWebsite.TabIndex = 14;
            this.lblWebsite.Text = "Website";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(3, 16);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(216, 20);
            this.txtEmail.TabIndex = 5;
            // 
            // lblContact
            // 
            this.lblContact.AutoSize = true;
            this.lblContact.Location = new System.Drawing.Point(3, 39);
            this.lblContact.Name = "lblContact";
            this.lblContact.Size = new System.Drawing.Size(75, 13);
            this.lblContact.TabIndex = 15;
            this.lblContact.Text = "Contact Name";
            // 
            // txtContact
            // 
            this.txtContact.Location = new System.Drawing.Point(3, 55);
            this.txtContact.Name = "txtContact";
            this.txtContact.Size = new System.Drawing.Size(216, 20);
            this.txtContact.TabIndex = 6;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(3, 78);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(72, 13);
            this.lblEmail.TabIndex = 16;
            this.lblEmail.Text = "Contact Email";
            // 
            // txtWebsite
            // 
            this.txtWebsite.Location = new System.Drawing.Point(3, 94);
            this.txtWebsite.Name = "txtWebsite";
            this.txtWebsite.Size = new System.Drawing.Size(216, 20);
            this.txtWebsite.TabIndex = 7;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.lblCode);
            this.flowLayoutPanel2.Controls.Add(this.txtCode);
            this.flowLayoutPanel2.Controls.Add(this.lblCitation);
            this.flowLayoutPanel2.Controls.Add(this.txtCitation);
            this.flowLayoutPanel2.Controls.Add(this.lblAbstract);
            this.flowLayoutPanel2.Controls.Add(this.txtAbstract);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(19, 104);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(253, 126);
            this.flowLayoutPanel2.TabIndex = 18;
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Location = new System.Drawing.Point(3, 0);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(32, 13);
            this.lblCode.TabIndex = 10;
            this.lblCode.Text = "Code";
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(3, 16);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(216, 20);
            this.txtCode.TabIndex = 2;
            // 
            // lblCitation
            // 
            this.lblCitation.AutoSize = true;
            this.lblCitation.Location = new System.Drawing.Point(3, 39);
            this.lblCitation.Name = "lblCitation";
            this.lblCitation.Size = new System.Drawing.Size(42, 13);
            this.lblCitation.TabIndex = 11;
            this.lblCitation.Text = "Citation";
            // 
            // txtCitation
            // 
            this.txtCitation.Location = new System.Drawing.Point(3, 55);
            this.txtCitation.Name = "txtCitation";
            this.txtCitation.Size = new System.Drawing.Size(216, 20);
            this.txtCitation.TabIndex = 3;
            // 
            // lblAbstract
            // 
            this.lblAbstract.AutoSize = true;
            this.lblAbstract.Location = new System.Drawing.Point(3, 78);
            this.lblAbstract.Name = "lblAbstract";
            this.lblAbstract.Size = new System.Drawing.Size(46, 13);
            this.lblAbstract.TabIndex = 12;
            this.lblAbstract.Text = "Abstract";
            // 
            // txtAbstract
            // 
            this.txtAbstract.Location = new System.Drawing.Point(3, 94);
            this.txtAbstract.Name = "txtAbstract";
            this.txtAbstract.Size = new System.Drawing.Size(216, 20);
            this.txtAbstract.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.lblTitle);
            this.flowLayoutPanel1.Controls.Add(this.txtTitle);
            this.flowLayoutPanel1.Controls.Add(this.lblUrl);
            this.flowLayoutPanel1.Controls.Add(this.txtURL);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(19, 19);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(547, 79);
            this.flowLayoutPanel1.TabIndex = 17;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(95, 13);
            this.lblTitle.TabIndex = 8;
            this.lblTitle.Text = "Title (Required)";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(3, 16);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(482, 20);
            this.txtTitle.TabIndex = 0;
            this.txtTitle.Validating += new System.ComponentModel.CancelEventHandler(this.txtTitle_Validating);
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUrl.Location = new System.Drawing.Point(3, 39);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(95, 13);
            this.lblUrl.TabIndex = 9;
            this.lblUrl.Text = "URL (Required)";
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(3, 55);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(482, 20);
            this.txtURL.TabIndex = 1;
            this.txtURL.Validating += new System.ComponentModel.CancelEventHandler(this.txtURL_Validating);
            // 
            // tcAddService
            // 
            this.tcAddService.Controls.Add(this.tpAddSingleSvc);
            this.tcAddService.Controls.Add(this.tpAddMultiSvcs);
            this.tcAddService.Location = new System.Drawing.Point(12, 27);
            this.tcAddService.Name = "tcAddService";
            this.tcAddService.SelectedIndex = 0;
            this.tcAddService.Size = new System.Drawing.Size(592, 272);
            this.tcAddService.TabIndex = 32;
            this.tcAddService.SelectedIndexChanged += new System.EventHandler(this.tcAddService_SelectedIndexChanged);
            // 
            // tpAddSingleSvc
            // 
            this.tpAddSingleSvc.Controls.Add(this.groupBox1);
            this.tpAddSingleSvc.Location = new System.Drawing.Point(4, 22);
            this.tpAddSingleSvc.Name = "tpAddSingleSvc";
            this.tpAddSingleSvc.Padding = new System.Windows.Forms.Padding(3);
            this.tpAddSingleSvc.Size = new System.Drawing.Size(584, 246);
            this.tpAddSingleSvc.TabIndex = 0;
            this.tpAddSingleSvc.Text = "Add Single Service";
            this.tpAddSingleSvc.UseVisualStyleBackColor = true;
            // 
            // tpAddMultiSvcs
            // 
            this.tpAddMultiSvcs.Controls.Add(this.gbxServices);
            this.tpAddMultiSvcs.Location = new System.Drawing.Point(4, 22);
            this.tpAddMultiSvcs.Name = "tpAddMultiSvcs";
            this.tpAddMultiSvcs.Padding = new System.Windows.Forms.Padding(3);
            this.tpAddMultiSvcs.Size = new System.Drawing.Size(584, 246);
            this.tpAddMultiSvcs.TabIndex = 1;
            this.tpAddMultiSvcs.Text = "Add Multiple Services";
            this.tpAddMultiSvcs.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.ContainerControl = this;
            // 
            // AddServicesForm
            // 
            this.AcceptButton = this.btnUpdate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 355);
            this.Controls.Add(this.tcAddService);
            this.Controls.Add(this.gbxUpdate);
            this.Controls.Add(this.gbxProgress);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AddServicesForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Add WaterOneFlow Service Info ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddServicesForm_FormClosing);
            this.Load += new System.EventHandler(this.AddServicesForm_Load);
            this.gbxProgress.ResumeLayout(false);
            this.gbxServices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAddServices)).EndInit();
            this.gbxUpdate.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tcAddService.ResumeLayout(false);
            this.tpAddSingleSvc.ResumeLayout(false);
            this.tpAddMultiSvcs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox gbxProgress;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ProgressBar prgMain;
		private System.Windows.Forms.GroupBox gbxServices;
		private System.Windows.Forms.DataGridView dgvAddServices;
		private System.Windows.Forms.Button btnUpdate;
		private System.ComponentModel.BackgroundWorker bgwMain;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgcServiceTitle;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgcServiceUrl;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgcServiceCode;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgcCitation;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgcAbstract;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgcDescriptionUrl;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgcContactName;
		private System.Windows.Forms.DataGridViewTextBoxColumn dgcContactEmail;
		private System.Windows.Forms.GroupBox gbxUpdate;
		private System.Windows.Forms.Button btnCheckExisting;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnuCheckForValidService;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fromFileToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtContact;
        private System.Windows.Forms.TextBox txtWebsite;
        private System.Windows.Forms.TextBox txtAbstract;
        private System.Windows.Forms.TextBox txtCitation;
        private System.Windows.Forms.Label lblCitation;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblContact;
        private System.Windows.Forms.Label lblWebsite;
        private System.Windows.Forms.Label lblAbstract;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.TabControl tcAddService;
        private System.Windows.Forms.TabPage tpAddSingleSvc;
        private System.Windows.Forms.TabPage tpAddMultiSvcs;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.ErrorProvider errorProvider1;
	}
}