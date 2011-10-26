using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EPADelineation
{
    public partial class FmProgress : Form
    {
        public bool _isworking = false;

        public FmProgress()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Updates text in the progress bar dialog
        /// </summary>
        /// <param name="startpt">the comid of the outlet point</param>
        public void updateText(object[] startpt)
        {
            _isworking = true;

            try
            {
                string comid = startpt[0] as string;
                string measure = startpt[1] as string;

                this.lblmessage2.Text = "Sending Request to EPA Delineation Web Services \n for   COMID = " + comid + " \n and MEASURE = " + measure + "...";
                this.lblmessage1.Visible = false;
                this.lblmessage2.Visible = true;
                this.lblmessage3.Visible = false;

                //Update the Label Text
                Application.DoEvents();

                _isworking = false;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.closeForm(); 

                _isworking = false;
            }
        }

        public void updateText()
        {
            _isworking = true;

            try
            {
                this.lblmessage3.Visible = true;
                this.lblmessage1.Visible = false;
                this.lblmessage2.Visible = false;

                Application.DoEvents();

                _isworking = false;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.closeForm(); 
                _isworking = false;
            }
        }

        public void closeForm()
        {
            _isworking = true;
            try
            {
                this.Close();
                _isworking = false;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.closeForm(); 

                _isworking = false;
            }
        }

    }
}
