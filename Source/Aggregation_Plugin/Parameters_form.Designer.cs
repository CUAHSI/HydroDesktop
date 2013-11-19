namespace Aggregation_Plugin
{
    partial class Parameters_form
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OK_click = new System.Windows.Forms.Button();
            this.Cancel_click = new System.Windows.Forms.Button();
            this.PolygonLayerList = new System.Windows.Forms.ComboBox();
            this.SiteList = new System.Windows.Forms.ComboBox();
            this.Polygon_Label = new System.Windows.Forms.Label();
            this.Point_Label = new System.Windows.Forms.Label();
            this.Variable_Label = new System.Windows.Forms.Label();
            this.Output_txt = new System.Windows.Forms.TextBox();
            this.VariableList = new System.Windows.Forms.ComboBox();
            this.Output_Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OK_click
            // 
            this.OK_click.Location = new System.Drawing.Point(59, 116);
            this.OK_click.Name = "OK_click";
            this.OK_click.Size = new System.Drawing.Size(75, 23);
            this.OK_click.TabIndex = 0;
            this.OK_click.Text = "OK";
            this.OK_click.UseVisualStyleBackColor = true;
            // 
            // Cancel_click
            // 
            this.Cancel_click.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_click.Location = new System.Drawing.Point(140, 116);
            this.Cancel_click.Name = "Cancel_click";
            this.Cancel_click.Size = new System.Drawing.Size(75, 23);
            this.Cancel_click.TabIndex = 1;
            this.Cancel_click.Text = "Cancel";
            this.Cancel_click.UseVisualStyleBackColor = true;
            // 
            // PolygonLayerList
            // 
            this.PolygonLayerList.FormattingEnabled = true;
            this.PolygonLayerList.Location = new System.Drawing.Point(82, 9);
            this.PolygonLayerList.Name = "PolygonLayerList";
            this.PolygonLayerList.Size = new System.Drawing.Size(170, 21);
            this.PolygonLayerList.TabIndex = 2;
            // 
            // SiteList
            // 
            this.SiteList.FormattingEnabled = true;
            this.SiteList.Location = new System.Drawing.Point(82, 36);
            this.SiteList.Name = "SiteList";
            this.SiteList.Size = new System.Drawing.Size(170, 21);
            this.SiteList.TabIndex = 3;
            // 
            // Polygon_Label
            // 
            this.Polygon_Label.AutoSize = true;
            this.Polygon_Label.Location = new System.Drawing.Point(1, 12);
            this.Polygon_Label.Name = "Polygon_Label";
            this.Polygon_Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Polygon_Label.Size = new System.Drawing.Size(74, 13);
            this.Polygon_Label.TabIndex = 4;
            this.Polygon_Label.Text = "Polygon Layer";
            // 
            // Point_Label
            // 
            this.Point_Label.AutoSize = true;
            this.Point_Label.Location = new System.Drawing.Point(50, 39);
            this.Point_Label.Name = "Point_Label";
            this.Point_Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Point_Label.Size = new System.Drawing.Size(25, 13);
            this.Point_Label.TabIndex = 5;
            this.Point_Label.Text = "Site";
            // 
            // Variable_Label
            // 
            this.Variable_Label.AutoSize = true;
            this.Variable_Label.Location = new System.Drawing.Point(30, 66);
            this.Variable_Label.Name = "Variable_Label";
            this.Variable_Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Variable_Label.Size = new System.Drawing.Size(45, 13);
            this.Variable_Label.TabIndex = 6;
            this.Variable_Label.Text = "Variable";
            // 
            // Output_txt
            // 
            this.Output_txt.Location = new System.Drawing.Point(82, 89);
            this.Output_txt.Name = "Output_txt";
            this.Output_txt.Size = new System.Drawing.Size(170, 20);
            this.Output_txt.TabIndex = 7;
            // 
            // VariableList
            // 
            this.VariableList.FormattingEnabled = true;
            this.VariableList.Location = new System.Drawing.Point(82, 63);
            this.VariableList.Name = "VariableList";
            this.VariableList.Size = new System.Drawing.Size(170, 21);
            this.VariableList.TabIndex = 8;
            // 
            // Output_Label
            // 
            this.Output_Label.AutoSize = true;
            this.Output_Label.Location = new System.Drawing.Point(5, 89);
            this.Output_Label.Name = "Output_Label";
            this.Output_Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Output_Label.Size = new System.Drawing.Size(70, 13);
            this.Output_Label.TabIndex = 9;
            this.Output_Label.Text = "Output Name";
            // 
            // Parameters_form
            // 
            this.AcceptButton = this.OK_click;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.CancelButton = this.Cancel_click;
            this.ClientSize = new System.Drawing.Size(264, 147);
            this.Controls.Add(this.Output_Label);
            this.Controls.Add(this.VariableList);
            this.Controls.Add(this.Output_txt);
            this.Controls.Add(this.Variable_Label);
            this.Controls.Add(this.Point_Label);
            this.Controls.Add(this.Polygon_Label);
            this.Controls.Add(this.SiteList);
            this.Controls.Add(this.PolygonLayerList);
            this.Controls.Add(this.Cancel_click);
            this.Controls.Add(this.OK_click);
            this.Name = "Parameters_form";
            this.Text = "Aggregation tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OK_click;
        private System.Windows.Forms.Button Cancel_click;
        private System.Windows.Forms.ComboBox PolygonLayerList;
        private System.Windows.Forms.ComboBox SiteList;
        private System.Windows.Forms.Label Polygon_Label;
        private System.Windows.Forms.Label Point_Label;
        private System.Windows.Forms.Label Variable_Label;
        private System.Windows.Forms.TextBox Output_txt;
        private System.Windows.Forms.ComboBox VariableList;
        private System.Windows.Forms.Label Output_Label;
    }
}