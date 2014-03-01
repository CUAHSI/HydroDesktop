#region Copyright
///////////////////////////////////////////////////////////
//
//    Copyright (C) 2006 OpenMI Association
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//    or look at URL www.gnu.org/licenses/lgpl.html
//
//    Contact info: 
//      URL: www.openmi.org
//	Email: sourcecode@openmi.org
//	Discussion forum available at www.sourceforge.net
//
//      Coordinator: Roger Moore, CEH Wallingford, Wallingford, Oxon, UK
//
///////////////////////////////////////////////////////////
//
//  Original authors: Jan Curn, DHI - Water & Environment, Prague, Czech Republic
//                    Jan B. Gregersen, DHI - Water & Environment, Horsholm, Denmark
//  Created on:      1. July 2005
//  Version:         1.0.0 
//
//  Modification history:
//  
//
///////////////////////////////////////////////////////////
#endregion
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
	/// <summary>
	/// Summary description for AboutBox.
	/// </summary>
	public class AboutBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.LinkLabel linkWwwOpenMIOrg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkWwwSourceforgeNet;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Creates a new instance of <see cref="AboutBox">AboutBox</see> dialog.
		/// </summary>
		public AboutBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.linkWwwOpenMIOrg = new System.Windows.Forms.LinkLabel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.linkWwwSourceforgeNet = new System.Windows.Forms.LinkLabel();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(136, 436);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(148, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(380, 432);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // linkWwwOpenMIOrg
            // 
            this.linkWwwOpenMIOrg.Location = new System.Drawing.Point(60, 459);
            this.linkWwwOpenMIOrg.Name = "linkWwwOpenMIOrg";
            this.linkWwwOpenMIOrg.Size = new System.Drawing.Size(164, 16);
            this.linkWwwOpenMIOrg.TabIndex = 2;
            this.linkWwwOpenMIOrg.TabStop = true;
            this.linkWwwOpenMIOrg.Text = "http://www.openmi.org";
            this.linkWwwOpenMIOrg.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkWwwOpenMIOrg_LinkClicked);
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonClose.Location = new System.Drawing.Point(413, 522);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(92, 32);
            this.buttonClose.TabIndex = 10;
            this.buttonClose.Text = "Close";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 443);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Contact info:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(20, 459);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 16);
            this.label2.TabIndex = 11;
            this.label2.Text = "URL:";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(20, 491);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(156, 16);
            this.label7.TabIndex = 13;
            this.label7.Text = "Discussion forum available at:";
            // 
            // linkWwwSourceforgeNet
            // 
            this.linkWwwSourceforgeNet.Location = new System.Drawing.Point(180, 491);
            this.linkWwwSourceforgeNet.Name = "linkWwwSourceforgeNet";
            this.linkWwwSourceforgeNet.Size = new System.Drawing.Size(207, 16);
            this.linkWwwSourceforgeNet.TabIndex = 15;
            this.linkWwwSourceforgeNet.TabStop = true;
            this.linkWwwSourceforgeNet.Text = "http://sourceforge.net/projects/openmi/";
            this.linkWwwSourceforgeNet.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkWwwSourceforgeNet_LinkClicked);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(12, 512);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 16);
            this.label9.TabIndex = 17;
            this.label9.Text = "Created on:";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(84, 512);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(92, 16);
            this.label10.TabIndex = 18;
            this.label10.Text = "January 2010";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(84, 532);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 16);
            this.label11.TabIndex = 20;
            this.label11.Text = "2.0";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(12, 532);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(68, 16);
            this.label12.TabIndex = 19;
            this.label12.Text = "Version:";
            // 
            // AboutBox
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(534, 563);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.linkWwwSourceforgeNet);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.linkWwwOpenMIOrg);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "About \"OpenMI Editor 2\" ...";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion

		private void linkWwwOpenMIOrg_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			ProcessStartInfo info = new ProcessStartInfo( linkWwwOpenMIOrg.Text );
			Process.Start( info );
		}

        //private void linkWwwJanCurn_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        //{
        //    ProcessStartInfo info = new ProcessStartInfo( linkWwwJanCurn.Text );
        //    Process.Start( info );		
        //}

		private void linkWwwSourceforgeNet_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			ProcessStartInfo info = new ProcessStartInfo( linkWwwSourceforgeNet.Text );
			Process.Start( info );		
		}

        private void label1_Click(object sender, EventArgs e)
        {

        }
	}
}
