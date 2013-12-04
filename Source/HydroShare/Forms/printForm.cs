using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace HydroShare
{
    public partial class printForm : Form
    {
        uploadForm1 form1 = new uploadForm1();

        public printForm(uploadForm1 f1)
        {
            InitializeComponent();
            form1 = f1;

            this.timeStampTB.Text = Convert.ToString(DateTime.Now);
            this.nameTB.Text = form1.name;
            this.sourceSubjectTB.Text = form1.sSubject;
            this.titleTB.Text = form1.title;
            this.emailTB.Text = form1.email;
            this.rightsTB.Text = form1.rights;
            this.coverageSpatialTB.Text = form1.coverageSpatial;
            this.fileTB.Text = form1.file;

            
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
