namespace ElevationGraph {
	partial class GraphForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
				map.VisibleChanged -= updateGraph;
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphForm));
			this.elevationGraph = new ZedGraph.ZedGraphControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.YcomboBox = new System.Windows.Forms.ComboBox();
			this.XcomboBox = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// elevationGraph
			// 
			this.elevationGraph.AutoSize = true;
			this.elevationGraph.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.elevationGraph.Dock = System.Windows.Forms.DockStyle.Top;
			this.elevationGraph.Location = new System.Drawing.Point(0, 0);
			this.elevationGraph.Name = "elevationGraph";
			this.elevationGraph.ScrollGrace = 0D;
			this.elevationGraph.ScrollMaxX = 0D;
			this.elevationGraph.ScrollMaxY = 0D;
			this.elevationGraph.ScrollMaxY2 = 0D;
			this.elevationGraph.ScrollMinX = 0D;
			this.elevationGraph.ScrollMinY = 0D;
			this.elevationGraph.ScrollMinY2 = 0D;
			this.elevationGraph.Size = new System.Drawing.Size(503, 0);
			this.elevationGraph.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.YcomboBox);
			this.panel1.Controls.Add(this.XcomboBox);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 378);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(503, 32);
			this.panel1.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Y Axis:";
			// 
			// YcomboBox
			// 
			this.YcomboBox.FormattingEnabled = true;
			this.YcomboBox.Items.AddRange(new object[] {
            "Meters",
            "Kilometers",
            "Feet",
            "Miles"});
			this.YcomboBox.Location = new System.Drawing.Point(48, 7);
			this.YcomboBox.Name = "YcomboBox";
			this.YcomboBox.Size = new System.Drawing.Size(88, 21);
			this.YcomboBox.TabIndex = 4;
			this.YcomboBox.Text = "Meters";
			this.YcomboBox.SelectedIndexChanged += new System.EventHandler(this.YcomboBox_SelectedIndexChanged);
			// 
			// XcomboBox
			// 
			this.XcomboBox.FormattingEnabled = true;
			this.XcomboBox.Items.AddRange(new object[] {
            "Meters",
            "Kilometers",
            "Feet",
            "Miles"});
			this.XcomboBox.Location = new System.Drawing.Point(196, 7);
			this.XcomboBox.Name = "XcomboBox";
			this.XcomboBox.Size = new System.Drawing.Size(88, 21);
			this.XcomboBox.TabIndex = 3;
			this.XcomboBox.Text = "Meters";
			this.XcomboBox.SelectedIndexChanged += new System.EventHandler(this.XcomboBox_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(151, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "X Axis:";
			// 
			// button2
			// 
			this.button2.Dock = System.Windows.Forms.DockStyle.Right;
			this.button2.Location = new System.Drawing.Point(353, 0);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 32);
			this.button2.TabIndex = 1;
			this.button2.Text = "Save";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			this.button1.Dock = System.Windows.Forms.DockStyle.Right;
			this.button1.Location = new System.Drawing.Point(428, 0);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 32);
			this.button1.TabIndex = 0;
			this.button1.Text = "Close";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// zedGraphControl1
			// 
			this.zedGraphControl1.AutoSize = true;
			this.zedGraphControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.zedGraphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.zedGraphControl1.Location = new System.Drawing.Point(0, 0);
			this.zedGraphControl1.Name = "zedGraphControl1";
			this.zedGraphControl1.ScrollGrace = 0D;
			this.zedGraphControl1.ScrollMaxX = 0D;
			this.zedGraphControl1.ScrollMaxY = 0D;
			this.zedGraphControl1.ScrollMaxY2 = 0D;
			this.zedGraphControl1.ScrollMinX = 0D;
			this.zedGraphControl1.ScrollMinY = 0D;
			this.zedGraphControl1.ScrollMinY2 = 0D;
			this.zedGraphControl1.Size = new System.Drawing.Size(503, 378);
			this.zedGraphControl1.TabIndex = 2;
			// 
			// GraphForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(503, 410);
			this.Controls.Add(this.zedGraphControl1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.elevationGraph);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "GraphForm";
			this.Text = "Elevation Graph";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ZedGraph.ZedGraphControl elevationGraph;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private ZedGraph.ZedGraphControl zedGraphControl1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox YcomboBox;
		private System.Windows.Forms.ComboBox XcomboBox;
	}
}