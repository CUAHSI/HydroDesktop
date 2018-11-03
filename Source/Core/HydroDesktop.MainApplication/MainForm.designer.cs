// ********************************************************************************************************
// HydroDesktop Version 1.8
// Developed for the Consortium of Universities for the Advancement of Hydrologic Science, Inc. (CUAHSI) as a desktop client for the CUAHSI Hydrologic Information System(see www.cuahsi.org).
// Development team at Idaho State University and Brigham Young University lead by Dr.Dan Ames(dan.ames @byu.edu).
// Source code available on GitHub at https://github.com/CUAHSI/HydroDesktop
// Copyright 2012-present, CUAHSI and Dan Ames
//
// Please use the following citation to refer to HydroDesktop in your publications:
// Ames, D.P., Horsburgh, J.S., Cao, Y., Kadlec, J., Whiteaker, T., and Valentine, D., 2012. HydroDesktop: Web Services-Based Software for Hydrologic Data Discovery, Download, Visualization, and Analysis.Environmental Modelling & Software.Vol 37, pp 146-156. http://dx.doi.org/10.1016/j.envsoft.2012.03.013
//
// Development of HydroDesktop was/is supported by the following grants from the National Science Foundation: OCI-1148453, OCI-1148090, EAR-0622374, EPS-0814387, EPS-0919514.
// ********************************************************************************************************
// The contents of this file are subject to the MIT License (MIT)
// you may not use this file except in compliance with the License. You may obtain a copy of the License at
// http://www.hydrodesktop.org/
//
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
// ANY KIND, either expressed or implied. See the License for the specific language governing rights and
// limitations under the License.
//
// The Initial Developers of this Original Code are Dan Ames and Ted Dunsford. Originally created in 2010.
// ********************************************************************************************************

namespace HydroDesktop.MainApplication
{
    /// <summary>
    /// Form
    /// </summary>
    partial class MainForm
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
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 149);
            this.Icon = Properties.Resources.HydroDesktop;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "HydroDesktop";
            this.LocationChanged += new System.EventHandler(this.MainForm_LocationChanged);
            this.ResumeLayout(false);

        }

        #endregion

    }
}