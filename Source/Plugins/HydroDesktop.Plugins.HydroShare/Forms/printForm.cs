using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace HydroDesktop.Plugins.HydroShare
{
    public partial class printForm : Form
    {
        uploadForm1_geo form_geo = new uploadForm1_geo();
        uploadForm1_other form_other = new uploadForm1_other();
        uploadForm1_time form_time = new uploadForm1_time();

        public printForm(uploadForm1_geo f1)
        {
            InitializeComponent();
            form_geo = f1;

            this.timeStampTB.Text = Convert.ToString(DateTime.Now);
            this.nameTB.Text = form_geo.name;
            this.sourceSubjectTB.Text = form_geo.sSubject;
            this.titleTB.Text = form_geo.title;
            this.emailTB.Text = form_geo.email;
            this.rightsTB.Text = form_geo.rights;
            this.coverageSpatialTB.Text = form_geo.coverageSpatial;
            this.fileTB.Text = form_geo.file;

            
        }

        public printForm(uploadForm1_other f1)
        {
            InitializeComponent();
            form_other = f1;

            this.timeStampTB.Text = Convert.ToString(DateTime.Now);
            this.nameTB.Text = form_geo.name;
            this.sourceSubjectTB.Text = form_geo.sSubject;
            this.titleTB.Text = form_geo.title;
            this.emailTB.Text = form_geo.email;
            this.rightsTB.Text = form_geo.rights;
            this.coverageSpatialTB.Text = form_geo.coverageSpatial;
            this.fileTB.Text = form_geo.file;


        }

        public printForm(uploadForm1_time f1)
        {
            InitializeComponent();
            form_time = f1;

            this.timeStampTB.Text = Convert.ToString(DateTime.Now);
            this.nameTB.Text = form_geo.name;
            this.sourceSubjectTB.Text = form_geo.sSubject;
            this.titleTB.Text = form_geo.title;
            this.emailTB.Text = form_geo.email;
            this.rightsTB.Text = form_geo.rights;
            this.coverageSpatialTB.Text = form_geo.coverageSpatial;
            this.fileTB.Text = form_geo.file;


        }

        private void printButton_Click(object sender, EventArgs e)
        {
            //PrintDocument pd = new PrintDocument();
            //pd.PrintPage += new PrintPageEventHandler(PrintImage);
            //pd.Print();
        }

        void PrintImage(object o, PrintPageEventArgs e)
        {
            int x = SystemInformation.WorkingArea.X;
            int y = SystemInformation.WorkingArea.Y;
            int width = this.Width;
            int height = this.Height;

            Rectangle bounds = new Rectangle(x, y, width, height);

            Bitmap img = new Bitmap(width, height);

            this.DrawToBitmap(img, bounds);
            Point p = new Point(100, 100);
            e.Graphics.DrawImage(img, p);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
           
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
            }
        }

     
    }
}
