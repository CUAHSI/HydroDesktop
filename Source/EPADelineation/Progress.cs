using System;
using System.Windows.Forms;

namespace EPADelineation
{
    public partial class FmProgress : Form
    {
        /// <summary>
        /// True if the process is still working
        /// </summary>
        public bool _isworking = false;

        /// <summary>
        /// Creates a new instance of the progress dialog form
        /// </summary>
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
        /// <summary>
        /// updates progress status text in the progress form
        /// </summary>
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

        /// <summary>
        /// Closes the progress dialog form
        /// </summary>
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
