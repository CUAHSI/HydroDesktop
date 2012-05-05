namespace HydroDesktop.ObjectModel.Controls
{
    partial class SiteView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblElevation = new System.Windows.Forms.Label();
            this.tbSiteCode = new System.Windows.Forms.TextBox();
            this.lblSiteCode = new System.Windows.Forms.Label();
            this.lblLongitude = new System.Windows.Forms.Label();
            this.lblLatitude = new System.Windows.Forms.Label();
            this.tbSiteName = new System.Windows.Forms.TextBox();
            this.lblSiteName = new System.Windows.Forms.Label();
            this.tbVertDatum = new System.Windows.Forms.TextBox();
            this.lblVerticalDatum = new System.Windows.Forms.Label();
            this.lblLocalX = new System.Windows.Forms.Label();
            this.lblLocalY = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.lblPosAccuracy = new System.Windows.Forms.Label();
            this.nudPosAccuracy = new HydroDesktop.ObjectModel.Controls.NumericUpDownEx();
            this.nudLocalY = new HydroDesktop.ObjectModel.Controls.NumericUpDownEx();
            this.nudLocalX = new HydroDesktop.ObjectModel.Controls.NumericUpDownEx();
            this.nudElevation = new HydroDesktop.ObjectModel.Controls.NumericUpDownEx();
            this.nudLng = new HydroDesktop.ObjectModel.Controls.NumericUpDownEx();
            this.nudLat = new HydroDesktop.ObjectModel.Controls.NumericUpDownEx();
            this.tbState = new System.Windows.Forms.TextBox();
            this.lblState = new System.Windows.Forms.Label();
            this.tbCounty = new System.Windows.Forms.TextBox();
            this.lblCounty = new System.Windows.Forms.Label();
            this.tbComments = new System.Windows.Forms.TextBox();
            this.lblComments = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPosAccuracy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudElevation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLng)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLat)).BeginInit();
            this.SuspendLayout();
            // 
            // lblElevation
            // 
            this.lblElevation.AutoSize = true;
            this.lblElevation.Location = new System.Drawing.Point(14, 130);
            this.lblElevation.Name = "lblElevation";
            this.lblElevation.Size = new System.Drawing.Size(51, 13);
            this.lblElevation.TabIndex = 20;
            this.lblElevation.Text = "Elevation";
            // 
            // tbSiteCode
            // 
            this.tbSiteCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSiteCode.Location = new System.Drawing.Point(103, 44);
            this.tbSiteCode.Name = "tbSiteCode";
            this.tbSiteCode.Size = new System.Drawing.Size(162, 20);
            this.tbSiteCode.TabIndex = 1;
            // 
            // lblSiteCode
            // 
            this.lblSiteCode.AutoSize = true;
            this.lblSiteCode.Location = new System.Drawing.Point(14, 47);
            this.lblSiteCode.Name = "lblSiteCode";
            this.lblSiteCode.Size = new System.Drawing.Size(52, 13);
            this.lblSiteCode.TabIndex = 19;
            this.lblSiteCode.Text = "Site code";
            // 
            // lblLongitude
            // 
            this.lblLongitude.AutoSize = true;
            this.lblLongitude.Location = new System.Drawing.Point(14, 102);
            this.lblLongitude.Name = "lblLongitude";
            this.lblLongitude.Size = new System.Drawing.Size(54, 13);
            this.lblLongitude.TabIndex = 18;
            this.lblLongitude.Text = "Longitude";
            // 
            // lblLatitude
            // 
            this.lblLatitude.AutoSize = true;
            this.lblLatitude.Location = new System.Drawing.Point(14, 74);
            this.lblLatitude.Name = "lblLatitude";
            this.lblLatitude.Size = new System.Drawing.Size(45, 13);
            this.lblLatitude.TabIndex = 17;
            this.lblLatitude.Text = "Latitude";
            // 
            // tbSiteName
            // 
            this.tbSiteName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSiteName.Location = new System.Drawing.Point(103, 16);
            this.tbSiteName.Name = "tbSiteName";
            this.tbSiteName.Size = new System.Drawing.Size(162, 20);
            this.tbSiteName.TabIndex = 0;
            // 
            // lblSiteName
            // 
            this.lblSiteName.AutoSize = true;
            this.lblSiteName.Location = new System.Drawing.Point(14, 19);
            this.lblSiteName.Name = "lblSiteName";
            this.lblSiteName.Size = new System.Drawing.Size(54, 13);
            this.lblSiteName.TabIndex = 14;
            this.lblSiteName.Text = "Site name";
            // 
            // tbVertDatum
            // 
            this.tbVertDatum.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbVertDatum.Location = new System.Drawing.Point(103, 156);
            this.tbVertDatum.Name = "tbVertDatum";
            this.tbVertDatum.Size = new System.Drawing.Size(162, 20);
            this.tbVertDatum.TabIndex = 21;
            // 
            // lblVerticalDatum
            // 
            this.lblVerticalDatum.AutoSize = true;
            this.lblVerticalDatum.Location = new System.Drawing.Point(14, 159);
            this.lblVerticalDatum.Name = "lblVerticalDatum";
            this.lblVerticalDatum.Size = new System.Drawing.Size(76, 13);
            this.lblVerticalDatum.TabIndex = 22;
            this.lblVerticalDatum.Text = "Vertical Datum";
            // 
            // lblLocalX
            // 
            this.lblLocalX.AutoSize = true;
            this.lblLocalX.Location = new System.Drawing.Point(14, 186);
            this.lblLocalX.Name = "lblLocalX";
            this.lblLocalX.Size = new System.Drawing.Size(40, 13);
            this.lblLocalX.TabIndex = 24;
            this.lblLocalX.Text = "LocalX";
            // 
            // lblLocalY
            // 
            this.lblLocalY.AutoSize = true;
            this.lblLocalY.Location = new System.Drawing.Point(14, 214);
            this.lblLocalY.Name = "lblLocalY";
            this.lblLocalY.Size = new System.Drawing.Size(40, 13);
            this.lblLocalY.TabIndex = 26;
            this.lblLocalY.Text = "LocalY";
            // 
            // lblPosAccuracy
            // 
            this.lblPosAccuracy.AutoSize = true;
            this.lblPosAccuracy.Location = new System.Drawing.Point(13, 242);
            this.lblPosAccuracy.Name = "lblPosAccuracy";
            this.lblPosAccuracy.Size = new System.Drawing.Size(84, 13);
            this.lblPosAccuracy.TabIndex = 28;
            this.lblPosAccuracy.Text = "PosAccuracy_m";
            // 
            // nudPosAccuracy
            // 
            this.nudPosAccuracy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPosAccuracy.DecimalPlaces = 4;
            this.nudPosAccuracy.FullReadOnly = false;
            this.nudPosAccuracy.Location = new System.Drawing.Point(103, 240);
            this.nudPosAccuracy.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudPosAccuracy.Minimum = new decimal(new int[] {
            90,
            0,
            0,
            -2147483648});
            this.nudPosAccuracy.Name = "nudPosAccuracy";
            this.nudPosAccuracy.Size = new System.Drawing.Size(162, 20);
            this.nudPosAccuracy.TabIndex = 27;
            // 
            // nudLocalY
            // 
            this.nudLocalY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLocalY.DecimalPlaces = 4;
            this.nudLocalY.FullReadOnly = false;
            this.nudLocalY.Location = new System.Drawing.Point(103, 212);
            this.nudLocalY.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudLocalY.Minimum = new decimal(new int[] {
            90,
            0,
            0,
            -2147483648});
            this.nudLocalY.Name = "nudLocalY";
            this.nudLocalY.Size = new System.Drawing.Size(162, 20);
            this.nudLocalY.TabIndex = 25;
            // 
            // nudLocalX
            // 
            this.nudLocalX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLocalX.DecimalPlaces = 4;
            this.nudLocalX.FullReadOnly = false;
            this.nudLocalX.Location = new System.Drawing.Point(103, 184);
            this.nudLocalX.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudLocalX.Minimum = new decimal(new int[] {
            90,
            0,
            0,
            -2147483648});
            this.nudLocalX.Name = "nudLocalX";
            this.nudLocalX.Size = new System.Drawing.Size(162, 20);
            this.nudLocalX.TabIndex = 23;
            // 
            // nudElevation
            // 
            this.nudElevation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudElevation.DecimalPlaces = 4;
            this.nudElevation.FullReadOnly = false;
            this.nudElevation.Location = new System.Drawing.Point(103, 128);
            this.nudElevation.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.nudElevation.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.nudElevation.Name = "nudElevation";
            this.nudElevation.Size = new System.Drawing.Size(162, 20);
            this.nudElevation.TabIndex = 4;
            // 
            // nudLng
            // 
            this.nudLng.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLng.DecimalPlaces = 4;
            this.nudLng.FullReadOnly = false;
            this.nudLng.Location = new System.Drawing.Point(103, 100);
            this.nudLng.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.nudLng.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.nudLng.Name = "nudLng";
            this.nudLng.Size = new System.Drawing.Size(162, 20);
            this.nudLng.TabIndex = 3;
            // 
            // nudLat
            // 
            this.nudLat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLat.DecimalPlaces = 4;
            this.nudLat.FullReadOnly = false;
            this.nudLat.Location = new System.Drawing.Point(103, 72);
            this.nudLat.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudLat.Minimum = new decimal(new int[] {
            90,
            0,
            0,
            -2147483648});
            this.nudLat.Name = "nudLat";
            this.nudLat.Size = new System.Drawing.Size(162, 20);
            this.nudLat.TabIndex = 2;
            // 
            // tbState
            // 
            this.tbState.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbState.Location = new System.Drawing.Point(103, 266);
            this.tbState.Name = "tbState";
            this.tbState.Size = new System.Drawing.Size(162, 20);
            this.tbState.TabIndex = 29;
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(14, 269);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(32, 13);
            this.lblState.TabIndex = 30;
            this.lblState.Text = "State";
            // 
            // tbCounty
            // 
            this.tbCounty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCounty.Location = new System.Drawing.Point(103, 292);
            this.tbCounty.Name = "tbCounty";
            this.tbCounty.Size = new System.Drawing.Size(162, 20);
            this.tbCounty.TabIndex = 31;
            // 
            // lblCounty
            // 
            this.lblCounty.AutoSize = true;
            this.lblCounty.Location = new System.Drawing.Point(14, 295);
            this.lblCounty.Name = "lblCounty";
            this.lblCounty.Size = new System.Drawing.Size(40, 13);
            this.lblCounty.TabIndex = 32;
            this.lblCounty.Text = "County";
            // 
            // tbComments
            // 
            this.tbComments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbComments.Location = new System.Drawing.Point(103, 318);
            this.tbComments.Name = "tbComments";
            this.tbComments.Size = new System.Drawing.Size(162, 20);
            this.tbComments.TabIndex = 33;
            // 
            // lblComments
            // 
            this.lblComments.AutoSize = true;
            this.lblComments.Location = new System.Drawing.Point(14, 321);
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new System.Drawing.Size(56, 13);
            this.lblComments.TabIndex = 34;
            this.lblComments.Text = "Comments";
            // 
            // SiteView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbComments);
            this.Controls.Add(this.lblComments);
            this.Controls.Add(this.tbCounty);
            this.Controls.Add(this.lblCounty);
            this.Controls.Add(this.tbState);
            this.Controls.Add(this.lblState);
            this.Controls.Add(this.nudPosAccuracy);
            this.Controls.Add(this.lblPosAccuracy);
            this.Controls.Add(this.nudLocalY);
            this.Controls.Add(this.lblLocalY);
            this.Controls.Add(this.nudLocalX);
            this.Controls.Add(this.lblLocalX);
            this.Controls.Add(this.tbVertDatum);
            this.Controls.Add(this.lblVerticalDatum);
            this.Controls.Add(this.nudElevation);
            this.Controls.Add(this.lblElevation);
            this.Controls.Add(this.tbSiteCode);
            this.Controls.Add(this.lblSiteCode);
            this.Controls.Add(this.nudLng);
            this.Controls.Add(this.nudLat);
            this.Controls.Add(this.lblLongitude);
            this.Controls.Add(this.lblLatitude);
            this.Controls.Add(this.tbSiteName);
            this.Controls.Add(this.lblSiteName);
            this.Name = "SiteView";
            this.Size = new System.Drawing.Size(279, 353);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPosAccuracy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLocalX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudElevation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLng)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLat)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NumericUpDownEx nudElevation;
        private System.Windows.Forms.Label lblElevation;
        private System.Windows.Forms.TextBox tbSiteCode;
        private System.Windows.Forms.Label lblSiteCode;
        private NumericUpDownEx nudLng;
        private NumericUpDownEx nudLat;
        private System.Windows.Forms.Label lblLongitude;
        private System.Windows.Forms.Label lblLatitude;
        private System.Windows.Forms.TextBox tbSiteName;
        private System.Windows.Forms.Label lblSiteName;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.TextBox tbVertDatum;
        private System.Windows.Forms.Label lblVerticalDatum;
        private NumericUpDownEx nudLocalX;
        private System.Windows.Forms.Label lblLocalX;
        private NumericUpDownEx nudLocalY;
        private System.Windows.Forms.Label lblLocalY;
        private NumericUpDownEx nudPosAccuracy;
        private System.Windows.Forms.Label lblPosAccuracy;
        private System.Windows.Forms.TextBox tbState;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.TextBox tbCounty;
        private System.Windows.Forms.Label lblCounty;
        private System.Windows.Forms.TextBox tbComments;
        private System.Windows.Forms.Label lblComments;
    }
}
