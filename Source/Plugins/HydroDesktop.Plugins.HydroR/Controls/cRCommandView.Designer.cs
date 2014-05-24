using System.Windows.Forms;
namespace HydroDesktop.Plugins.HydroR
{
    partial class cRCommandView
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
            this.sfdSave = new System.Windows.Forms.SaveFileDialog();
            this.ofdOpen = new System.Windows.Forms.OpenFileDialog();
            this.tTipExecute = new System.Windows.Forms.ToolTip(this.components);
            this.spcEditor = new System.Windows.Forms.SplitContainer();
            this.rtCommands = new HydroR.Controls.REditor();
            this.spcEditor.Panel1.SuspendLayout();
            this.spcEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // sfdSave
            // 
            this.sfdSave.DefaultExt = "r";
            this.sfdSave.Filter = "R files (*.r)|*.r| RichText (*.rtf)|*.rtf|Text File (*.txt)|*.txt|All Files (*.*)" +
                "|*.*";
            // 
            // ofdOpen
            // 
            this.ofdOpen.DefaultExt = "r";
            this.ofdOpen.Filter = "R files (*.r)|*.r|RichText (*.rtf)|*.rtf|Text File (*.txt)|*.txt|All Files (*.*)|" +
                "*.*";
            // 
            // spcEditor
            // 
            this.spcEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcEditor.Location = new System.Drawing.Point(0, 0);
            this.spcEditor.Name = "spcEditor";
            this.spcEditor.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // spcEditor.Panel1
            // 
            this.spcEditor.Panel1.Controls.Add(this.rtCommands);
            this.spcEditor.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // spcEditor.Panel2
            // 
            this.spcEditor.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            this.spcEditor.Panel2Collapsed = true;
            this.spcEditor.Size = new System.Drawing.Size(811, 631);
            this.spcEditor.SplitterDistance = 25;
            this.spcEditor.TabIndex = 1;
            this.spcEditor.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cRCommandView_MouseMove);
            this.spcEditor.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.spcEditor_SplitterMoved);
            // 
            // rtCommands
            // 
            this.rtCommands.AcceptsTab = true;
            this.rtCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtCommands.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtCommands.GenerateCode = false;
            this.rtCommands.Highlightlength = 528;
            this.rtCommands.Line = 0;
            this.rtCommands.Location = new System.Drawing.Point(3, 0);
            this.rtCommands.Name = "rtCommands";
            this.rtCommands.Size = new System.Drawing.Size(808, 625);
            this.rtCommands.TabIndex = 2;
            this.rtCommands.Text = global::HydroDesktop.Plugins.HydroR.Properties.Resources.PathToR;
            this.rtCommands.WordWrap = false;
            // 
            // cRCommandView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.spcEditor);
            this.Name = "cRCommandView";
            this.Size = new System.Drawing.Size(811, 631);
            this.Load += new System.EventHandler(this.cRCommandView_Load);
            this.spcEditor.Panel1.ResumeLayout(false);
            this.spcEditor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion        

        private SaveFileDialog sfdSave;
        private OpenFileDialog ofdOpen;
        private ToolTip tTipExecute;
        private SplitContainer spcEditor;
        public HydroR.Controls.REditor rtCommands;


    }
}
